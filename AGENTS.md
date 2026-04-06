# AGENTS.md - CrystalByRiya Project Guide

Purpose: this file is the working guide for anyone editing this repository. It is based on the code that is actually present in the project as of April 6, 2026, not just the intended architecture.

Read this file before making changes.

---

## 1. Project Snapshot

CrystalByRiya is a monolithic ASP.NET Core 9 web application for a crystal and jewelry storefront branded as **Dr. Astro Crystals** / **Crystals By Riya**.

It contains:

- Customer-facing Razor Pages under `Pages/`
- A large admin area under `Areas/Admin/Pages/`
- JSON/AJAX endpoints under `Api/`
- EF Core models in `Models/`
- A mix of EF Core, raw SQL, and stored-procedure-backed query models
- Session-based customer and admin login flows
- AWS S3 / CloudFront media upload helpers
- PhonePe payment integration (production credentials hardcoded)
- SMTP email sending with MailKit (currently commented out in registration)

**Current scale verified by directory scan:**

| Area | Count |
|------|-------|
| Customer page files (`Pages/`, `.cshtml` + `.cshtml.cs` pairs) | 55 top-level files under `Pages/` |
| Admin page files (`Areas/Admin/Pages/`) | 227 files across 24 subdirectories |
| API controllers (`Api/`) | 15 |
| MVC controllers (`Controllers/`) | 2 (`HomeController.cs`, `ReturnController.cs`) |
| Service/helper classes (`class/`) | 17 |
| EF model/result files (`Models/`) | 76 files + 1 ViewModel subfolder |
| StoredProcedure result classes (`StoredProcedure/`) | 4 |
| ViewComponents | 1 (`ProductsViewComponent.cs` registered as `"Category"`) |
| Helpers | 1 (`SessionHelper.cs`) |

The category ViewComponent also has its Razor view under `Pages/Components/Category/Category.cshtml(.cs)`. That folder is easy to miss because it sits outside the usual `Pages/Shared/Components/` convention.

---

## 2. Tech Stack

Verified from `CrystalByRiya.csproj` and source:

- .NET 9 (`net9.0`)
- ASP.NET Core Razor Pages + MVC controllers
- Entity Framework Core 9.0.5 (`Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Tools`)
- Microsoft.Data.SqlClient 6.0.2
- SQL Server (local dev: `DESKTOP-OI7KVCP`, database: `DrAstrocrystal`)
- Dapper 2.1.66 (referenced but most flows use EF Core `FromSqlRaw`)
- AWSSDK.S3 4.0.0.4
- MailKit 4.12.0
- MimeKit 4.12.0
- Newtonsoft.Json 13.0.3
- RestSharp 112.1.0
- BCrypt.Net-Next 4.0.2
- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation 9.0.5
- Microsoft.VisualStudio.Web.CodeGeneration.Design 9.0.0

**Build status:**

- `dotnet build` succeeds when no running process is locking `bin\Debug\net9.0\CrystalByRiya.dll`
- Build emits many nullable/analyzer warnings
- Known `MimeKit` 4.12.0 vulnerability warning (`NU1902`)
- `using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;` is imported in `myaccount.cshtml.cs` but not needed; it is a leftover import

---

## 3. What The App Really Does Today

### 3.1 Startup and Pipeline

Main startup is in `Program.cs`.

**Configured services (in registration order):**

```csharp
builder.Services.AddScoped<AddToWishlistModel>();
builder.Services.AddScoped<AddToCartItems>();
builder.Services.AddScoped<AwsCredentials>();
builder.Services.AddScoped<AmazonS3>();
builder.Services.AddResponseCompression(...)   // Gzip + Brotli, HTTPS enabled
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddScoped<PhonePePaymentService>();
builder.Services.AddSession(...)               // IdleTimeout = 40 mins, HttpOnly, IsEssential
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.Configure<FormOptions>(...)   // 300 MB multipart limit
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddAntiforgery();
builder.Services.AddDbContextPool<ApplicationDbContext>(...) // CrystalByRiyaConnection
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(...)                  // AllowAnyOrigin / AllowAnyHeader / AllowAnyMethod
```

**Configured middleware (in pipeline order):**

```
UseExceptionHandler("/Error")   [non-dev only]
UseHsts()                       [non-dev only]
UseDeveloperExceptionPage()     [dev only]
UseHttpsRedirection()
UseResponseCompression()
UseStaticFiles()
// UseRewriter() is COMMENTED OUT (RedirectToWwwRule + HTTPS redirect)
UseRouting()
UseCors()
UseResponseCaching()
UseAuthentication()             ← called but NO AddAuthentication() registered
UseAuthorization()
UseSession()
UseEndpoints → MapControllers() + MapRazorPages()
```

**Critical reality checks:**

- `UseAuthentication()` is called but there is no matching `AddAuthentication(...)` in `Program.cs`
- The real login system is session-based, not framework cookie-auth-based
- `ExceptionHandlingMiddleware.cs` exists in `class/` but is **NOT** registered in `Program.cs`
- `ExceptionLoggingFilter.cs` exists in `class/` but is **NOT** wired globally
- `RedirectToWwwRule.cs` exists at the project root but its `UseRewriter()` call is commented out
- `using CrystalByRiya.Classes;` appears twice in `Program.cs` (duplicate using, harmless)

---

### 3.2 Customer Authentication

Customer login and registration are handled in:

- `Pages/myaccount.cshtml.cs` — registration (`OnPost`) and login (`OnPostLoginAsync`)
- `Pages/checkout.cshtml.cs` — inline login (`OnPostLoginAsync`) during checkout
- `Pages/shop-account.cshtml.cs` — account management, billing pre-population
- `Pages/dashboard.cshtml.cs` — order history, profile, logout

**Actual session keys set on login/registration:**

| Key | Value |
|-----|-------|
| `UserEmail` | user's email address |
| `UserName` | user's display name |
| `Password` | plaintext password ⚠️ security risk |
| `Phone` | user's phone number |

**Login flow detail (`myaccount.cshtml.cs`):**

- Registration: checks duplicate email/phone → inserts `Register` row → sets session → redirects to `/cart` (or `redirectUrl` if provided)
- Welcome email code is currently **commented out** in `OnPost`
- Login (`OnPostLoginAsync`): matches `TblRegisters` by email + password (plaintext) → sets session → redirects to `/cart` or `redirectUrl`
- Password reset: separate email-token flow in `ForgetPassword.cshtml.cs` / `ResetPassword.cshtml.cs`; tokens stored in `PasswordResetTokens`

Logout is session removal, implemented in `dashboard.cshtml.cs`.

---

### 3.3 Admin Authentication

Admin login is in `Pages/adminlogin.cshtml.cs`.

- Credentials are checked against `TblAdmin` using `EmailId` + `Password`
- On success, session key `Login` is set to the admin email
- `TblAdminLogin` still exists in the model, but the active page login does not use it
- `TblEmployee` is still present for admin/staff data elsewhere in the app, but it is not the current admin-login source of truth
- Admin access guarded by checking `HttpContext.Session.GetString("Login")` per-page
- Admin layout also contains client-side redirect to `/adminlogin`

**Reality check:** admin protection is manual/session-based. No consistent `[Authorize]` pattern.

---

### 3.4 Cart and Wishlist

**Core logic files:**

- `class/AddToCartItems.cs` — cart add/buy-now helper (namespace: `CrystalByRiya.@class`)
- `class/AddtoWishlist.cs` — wishlist add helper
- `class/AddToWishlists.cs` — wishlist DTO/model class (**do not confuse** the two)
- `Pages/wishlist.cshtml(.cs)` — wishlist page that loads session data first, then falls back to `TblWishlist` for logged-in users using a projected query
- `Pages/dashboard.cshtml(.cs)` — dashboard wishlist tab that serves wishlist data through `?handler=Wishlist` and removes items through `?handler=RemoveFromWishlist`

**Session keys used:**

| Key | Type | Purpose |
|-----|------|---------|
| `cart` | JSON `List<Item>` | session cart items |
| `wishlist` | JSON `List<Item>` | session wishlist items |
| `buynowItem` | JSON `List<ChildskuCode>` | buy-now temporary item |
| `productid` | string | last added product SKU |
| `quantity` | int | last added quantity |
| `childsku` | string | last child SKU code |
| `AppliedCoupon` | `"True"` / absent | coupon applied flag |
| `Discount` | int | coupon discount percentage |

**Cart flow in `cart.cshtml.cs` (`CartModel`):**

- `OnGet()`:
  - If user logged in AND session cart empty → load from `TblCarts`
  - If user logged in AND session cart has items → reload from `TblCarts` (DB always wins)
  - If user not logged in → keep session cart as-is
  - Loads active `CouponCodes` for display
  - Calls `CalculateTotals()` — shipping is calculated after coupon discount, and is ₹80 if the discounted subtotal is below ₹3000, else free
- `OnPostCouponCode(string coupon_code)` — validates coupon, sets `AppliedCoupon` + `Discount` in session
- `OnPostUpdateQuantity(string productID, int delta, string size)` — increments/decrements quantity, clamps the minimum to 1, and updates both session and `TblCarts`
- `OnGetDelete(string id, string size, string addon, string material)` — removes from `TblCarts` AND session; must match all four fields exactly

**Known cart edge cases:**

- `NA` vs empty string/null mismatch between session and DB can prevent delete from clearing the DB row → item reappears on refresh
- `wwwroot/js/main.js` binds `.remove` class for mini-cart; if main cart rows reuse `.remove`, `OnGetDelete` is intercepted by JS and never fires
- DB always overrides session for logged-in users on every `OnGet` → any session-only edits are lost on page refresh
- `ChildskuCode` class is defined inline in `cart.cshtml.cs` (not shared with `AddToCartItems.cs` which has its own copy)
- Wishlist remove buttons need their own class names or handler wiring; reusing `.remove` on wishlist/dashboard markup can get intercepted by shared JS and stop the Razor handler from firing

**Add to cart (`AddToCartItems.cs`):**

- Calls SP `GetProductBySkuCode @PCode` via `FromSqlRaw` to resolve product details
- `NA` is stored for missing size/material/addon
- For logged-in users: inserts or increments `TblCarts` row
- Updates session JSON under key `cart`

---

### 3.5 Checkout and Payment

**Checkout flow files:**

- `Pages/checkout.cshtml.cs` — main active checkout page model
- `Pages/thankyou.cshtml.cs` — payment verification and order completion
- `Pages/Index1.cshtml.cs` — legacy/demo PhonePe flow (treat as legacy)
- `class/PhonePeCredientials.cs` — static credentials (hardcoded, production values)
- `class/PhonePePaymentService.cs` — exists but checkout does NOT use it for the active flow
- `Controllers/HomeController.cs` + `Controllers/ReturnController.cs` — older payment experiments (legacy)

**Checkout page model detail (`checkoutModel`):**

- Constructor takes both `ApplicationDbContext` and `PhonePePaymentService` (but service not used in main flow)
- `OnGet(bool isBuyNow)` — loads cart or buy-now items from session; pre-fills `TblBillingDetails` / `TblShippingDetails` for logged-in users
- `OnPostLoginAsync` — duplicate login handler (also exists on `myaccount`)
- `OnPostPlaceOrder(...)`:
  1. Validates all billing fields manually (not via `ModelState.IsValid`)
  2. Calculates subtotal/shipping/discount from session
  3. Calls `GenerateOrderIdForPayment(...)` → uses SP `SpOrderId` to get a `DAC`-prefixed order ID
  4. Stores order ID in session keys: `PhonePeTransactionId`, `PreGeneratedOrderId`
  5. Manually assembles PhonePe payload (base64 + SHA256 + `X-VERIFY` header)
  6. Posts to `https://api.phonepe.com/apis/hermes/pg/v1/pay`
  7. Redirects browser to PhonePe payment URL
  8. `OnGetVerifyPaymentAsync` calls `PhonePePaymentService.VerifyPaymentAsync(...)`, but that is separate from the manual payment initiation path

**PhonePe credentials (hardcoded in `class/PhonePeCredientials.cs`):**

| Field | Value |
|-------|-------|
| `Merchantid` | `CRYSTALSONLINE` |
| `SaltKey` | `66222d9f-cfd6-41fd-8b83-d495f887a63e` |
| `saltIndex` | `1` |
| `RedirectUrl` | `https://www.crystalsbyriya.com/thankyou` |
| `CallbackUrl` | `https://www.crystalsbyriya.com/api/response` |
| `PostUrl` | `https://api.phonepe.com/apis/hermes/pg/v1/pay` |

**Important session keys in checkout flow:**

| Key | Purpose |
|-----|---------|
| `isBuyNow` | `"True"` / `"False"` |
| `Phone` | customer phone |
| `Comment` | order notes |
| `PaymentMethod` | `"cod"` or online |
| `Email` | customer email for order |
| `BillingDetails` | JSON of `TblBillingDetail` |
| `ShippingDetails` | JSON of `TblShippingDetail` |
| `PhonePeTransactionId` | generated order ID |
| `PreGeneratedOrderId` | same order ID, for thank you page |
| `IsShipToDifferentAddress` | string bool |

**Shipping thresholds (hardcoded in both checkout and cart):**

- Effective subtotal after coupon discount below ₹3000 → ₹80 shipping
- Effective subtotal after coupon discount at or above ₹3000 → free shipping

**Cart vs checkout subtotal mismatch:** the two pages calculate totals independently; totals can differ unless logic is aligned.

**Country/state data:** hardcoded dictionary in `checkout.cshtml.cs` for India only, with a fixed list of Indian states and union territories.

---

### 3.6 Dynamic Features and Banners

#### 3.6.1 ProductList Page Banners
- `Pages/productlist.cshtml(.cs)` fetches category banners from `CategoryBanners` table (`TblCategoryBanner` DbSet → `CategoryBanner` model)
- Uses `CategoryId` foreign key to link banners to categories
- Banner images served via CloudFront CDN
- Gracefully handles missing banners (null-safe)

#### 3.6.2 Category Navigation (ViewComponent)
- `ViewComponents/ProductsViewComponent.cs` — registered as `"Category"` (namespace: `Viraj.ViewComponents`)
- Called in `_Layout.cshtml` as `@await Component.InvokeAsync("Category")`
- Fetches all `TblCategory` rows
- Passes `ChildViewModel` (which holds `List<TblCategory>`) to view `"Category"`
- The Razor view for that component lives in `Pages/Components/Category/Category.cshtml`
- `ChildViewModel` is in `Models/ViewModel/ChildViewModel.cs`; only contains `Categories` property (no `CategoryBanners` despite earlier docs)
- Navigation renders via the three-column-menu CSS class: `.three-column-menu`

#### 3.6.3 Layout Session Logic
`Pages/Shared/_Layout.cshtml` does more than layout rendering:

- Injects `ApplicationDbContext` directly via `@inject`
- Injects `IHttpContextAccessor` directly
- Reads `"cart"` session key → deserializes `List<Item>` → counts `cartCount`
- Reads `"wishlist"` session key → counts `wishlistCount`
- Reads `"UserEmail"` and `"UserName"` session keys for header account links
- Calls `_context.TblCategory.FirstOrDefault()` → uses result as `companyInfo` for top-bar marquee text and meta description
- Top-bar marquee uses `companyInfo.Description` (fallback hardcoded)
- Category dropdown uses three-column grid layout via `.three-column-menu`
- Mobile toolbar bottom bar links: Shop, Search, Account (links to `/adminlogin` ⚠️ should be customer account), Wishlist, Cart

**Warning:** moving logic out of the layout must be done carefully — it is tightly coupled to session and DB.

#### 3.6.4 Dashboard Wishlist Tab

- `Pages/dashboard.cshtml` currently contains page-local JavaScript for the wishlist tab instead of relying on the external `DashboardPage.js` bundle for wishlist rendering
- The wishlist tab calls `GET /dashboard?handler=Wishlist` and re-renders rows client-side
- Remove actions post to `POST /dashboard?handler=RemoveFromWishlist`
- The dashboard wishlist endpoint prefers session data first, then falls back to `TblWishlist` when the session is empty and `UserEmail` is present
- Keep the anti-forgery token available to the dashboard script if the remove flow is changed

---

### 3.7 Complete User Flow: Index to Checkout

**Homepage (`Pages/Index.cshtml(.cs)`):**
- Dynamic: featured products, banners, blogs, announcements, Instagram feed
- `OnPostRegister()` — creates account, sets session, redirects
- `OnPostAddToWishlistAsync()` — adds to wishlist with login redirect
- Redirects users with cart to `/cart` after registration

**ProductList (`Pages/productlist.cshtml(.cs)`):**
- Category banners from `TblCategoryBanner`
- Dynamic sorting/filtering with session state
- `OnPostAddToCart` / `OnPostAddToWishlistAsync` handlers
- Database-level pagination

**Product Detail (`Pages/detail.cshtml(.cs)`):**
- Dynamic gallery from `ImageGallery` model
- Size/material variants from backend
- `OnPostAddToCart()` and `OnPostAddToWishlistAsync()` handlers
- Buy Now: direct checkout with `buynow=true`

**Cart (`Pages/cart.cshtml(.cs)`):**
- Session/DB sync (see §3.4 for full logic)
- `OnPostCouponCode` / `OnPostUpdateQuantity` / `OnGetDelete`
- Delete path: `?handler=Delete&id=...&size=...&addon=...&material=...`

**Checkout (`Pages/checkout.cshtml(.cs)`):**
- Manual field validation, PhonePe redirect (see §3.5)

**Thank You (`Pages/thankyou.cshtml(.cs)`):**
- Verifies PhonePe transaction status
- Writes order to `TblBillingDetails`, `TblShippingDetails`, `TblCustomerOrderDetails`
- Sends order confirmation email
- Clears session order keys

---

## 4. Folder Guide

Use this structure when adding code.

### Core Folders

| Folder | Purpose |
|--------|---------|
| `Pages/` | Customer-facing Razor Pages (`.cshtml` + `.cshtml.cs` pairs) |
| `Areas/Admin/Pages/` | Admin CRUD pages, uses `~/Areas/Admin/Shared/_Layout.cshtml` |
| `Api/` | JSON/AJAX API controllers |
| `Controllers/` | Legacy MVC controllers (payment/redirect experiments) |
| `Models/` | EF Core entities, view models, table-mapped classes |
| `Models/ViewModel/` | Composite view models (currently only `ChildViewModel.cs`) |
| `StoredProcedure/` | Result classes for SP queries |
| `class/` | Business logic, DTO-like classes, services |
| `Helpers/` | Session helper extensions (`SessionHelper.cs`) |
| `ViewComponents/` | Razor view components (`ProductsViewComponent.cs`) |
| `Pages/Components/` | Razor view files for the category ViewComponent (`Pages/Components/Category/`) |
| `Pages/Components/NewFolder/` | Empty placeholder folder tracked in the project file |
| `wwwroot/` | Static assets |
| `wwwroot/ProductImage/` | Empty/tracked placeholder folder in the project file |
| `Scripts/` | Empty / utility scripts folder |
| `NewFolder/` | Empty placeholder folder |

### Important Layout and Shared Files

- `Pages/Shared/_Layout.cshtml` — customer layout (1418 lines, contains DB + session logic)
- `Pages/Shared/_Layout.cshtml.css` — scoped CSS for layout
- `Pages/Shared/_ValidationScriptsPartial.cshtml`
- `Areas/Admin/Shared/_Layout.cshtml` — admin layout
- `Pages/Components/Category/Category.cshtml(.cs)` — category ViewComponent view pair
- `Pages/_ViewImports.cshtml`
- `Pages/_ViewStart.cshtml`
- `Areas/Admin/_ViewStart.cshtml`

### Admin Subdirectories (verified)

```
Areas/Admin/Pages/
  AddOns/          (10 files: Create/Delete/Details/Edit/Index)
  AddToCart/
  Announcements/
  Banners/         (10 files)
  Blog/            (10 files)
  BlogsFaq/
  Category/        (10 files)
  Comments/
  Coupon/          (10 files)
  Employee/        (10 files)
  Gallery/         (10 files)
  Index.cshtml(.cs)
  Instagrams/
  Materials/       (11 files — includes an Edit.razor placeholder)
  Orders/          (4 files: BillingDetails + OrderDetails)
  Page/            (empty placeholder, tracked in .csproj)
  ProductFaqs/
  Products/        (18 files — includes CategoryAndSubCategory CRUD + Sizes)
  Productsizes/    (10 files)
  Relatedproducts/
  Reports/         (2 files: Index only)
  Review/          (10 files)
  ReviewGalleries/
  Subcategories/   (10 files)
  Wishlist/
```

---

## 5. Data Layer

### 5.1 DbContext

`Models/ApplicationDbContext.cs` is the central `DbContext`.

**All DbSets (verified from source):**

| DbSet Property | Entity Type | Notes |
|---------------|-------------|-------|
| `TblCategory` | `TblCategory` | categories |
| `TblSubcategory` | `Subcategory` | subcategories |
| `TblProducts` | `Product` | products |
| `TblBlogs` | `Blogs` | blog posts |
| `TblBanners` | `Banner` | home banners |
| `TblInstagram` | `Instagram` | Instagram feed |
| `TblBlogsFaq` | `BlogFaq` | blog FAQs |
| `TblRegisters` | `Register` | customer accounts |
| `TblAnnouncement` | `Announcement` | announcements |
| `TblProductFaq` | `ProductFaq` | product FAQs |
| `TblImageGalleries` | `ImageGallery` | product image galleries |
| `TblProductSizes` | `ProductSizes` | product size variants |
| `TblReviewGallery` | `ReviewGallery` | review images |
| `TblReviews` | `TblReviews` | product reviews |
| `TblCategoryWiseProduct` | `CategoryWiseProduct` | category↔product mapping |
| `TblOrderIds` | `TblOrderId` | order ID records |
| `TblBillingDetails` | `TblBillingDetail` | billing info |
| `TblShippingDetails` | `TblShippingDetail` | shipping info |
| `CouponCodes` | `CouponCodes` | coupons |
| `TblCustomerOrderDetails` | `TblCustomerOrderDetails` | order line items |
| `TblAdmin` | `TblAdmin` | active admin login source of truth (`EmailId` + `Password`) |
| `TblAdminLogin` | `AdminLogin` | legacy/alternate admin login table, not used by the active login page |
| `TblMailCredentials` | `MailCredentials` | SMTP credentials |
| `TblIntentionMaster` | `IntentionMaster` | crystal intentions |
| `TblCategoryBanner` | `CategoryBanner` | category banner images |
| `ExceptionLogs` | `ExceptionLog` | exception logging |
| `TblEmployee` | `Employee` | admin users |
| `Materials` | `Material` | product materials |
| `TblContactUs` | `ContactUs` | contact form submissions |
| `TblNewsLetters` | `NewsLetter` | newsletter subscribers |
| `TblCommentReply` | `CommentReply` | blog comments |
| `TblWishlist` | `AddingWishlist` | wishlist items |
| `TblAddOn` | `AddOn` | product add-ons |
| `RelatedProducts` | `RelatedProduct` | related products |
| `TblCarts` | `Cart` | cart items |
| `PasswordResetTokens` | `PasswordResetToken` | password reset tokens |

**Stored procedure DbSets (HasNoKey + ToView(null)):**

| Property | SP Result Class | SP Name |
|----------|---------------|---------|
| `ProductBySkuCodes` | `ProductBySkuCode` | `GetProductBySkuCode` |
| `SpBestSeller` | `BestSeller` | `SpBestseller` |
| `SPCategoryWiseProduct` | `SPCategoryWiseProduct` | `SPCategoryWiseProduct` |
| `SPSubCategoryWiseProduct` | `SPSubCategoryWiseProduct` | `SPSubCategoryWiseProduct` |

Other helpers:
- `DetachAllEntities()` — detaches Added/Modified/Deleted entities

### 5.2 Entity Naming is Mixed

This repo has both simplified model names and `Tbl*` names:

- `Product` (simplified) vs `TblProduct` (table-style) — both exist in `Models/`
- `Register` vs `TblRegister`
- `Cart` vs `TblCart`
- `Blogs` vs `TblBlog`
- `Banner` vs `TblBanner`

Do not assume naming consistency. Read the actual model and DbSet property before editing queries.

### 5.3 Stored Procedure Usage Pattern

```csharp
// Typical EF Core SP call
var skuParam = new SqlParameter("@PCode", SkuCode);
var results = await _context.ProductBySkuCodes
    .FromSqlRaw("EXEC GetProductBySkuCode @PCode", skuParam)
    .ToListAsync();
```

- Always use parameterized SQL
- If you add a new SP: add result class in `StoredProcedure/`, register in `ApplicationDbContext.cs` with `.HasNoKey().ToView(null)`

---

## 6. Services and Helpers

### `class/AddToCartItems.cs`
- Namespace: `CrystalByRiya.@class`
- Scoped service injected via `ILogger<AddToCartItems>` + `IHttpContextAccessor`
- `OnPostAddToCarts(...)` — calls `GetProductBySkuCode` SP, writes to `TblCarts`, updates session
- `ChildskuCode` is redefined here AND in `cart.cshtml.cs` — they are separate classes

### `class/AddtoWishlist.cs` vs `class/AddToWishlists.cs`
- `AddtoWishlist.cs` — wishlist helper service (the logic)
- `AddToWishlists.cs` — wishlist DTO/model class
- Do not confuse the two. The naming is almost identical.

### `class/AmazonS3.cs`
- Handles upload/delete via `AwsCredentials`
- Uploads return the generated file name (not a full URL)
- Callers prepend `cloudFrontURL` from `AwsCredentials`

### `class/AwsCredentials.cs`
- Contains hardcoded: AWS access key, secret key, bucket name, folder names, CloudFront base URL
- **Critical:** credentials are in source. Do not duplicate. Move to secrets long-term.

### `class/PhonePeCredientials.cs`
- Static class with hardcoded production PhonePe credentials
- Merchant ID: `CRYSTALSONLINE`
- The previous commented-out block (`/*...*/`) is still visible above the active block
- **Critical:** production salt key is in source

### `class/PhonePePaymentService.cs`
- Exists, registered as scoped in `Program.cs`
- `checkout.cshtml.cs` constructor accepts it but the active `OnPostPlaceOrder` path does NOT use it — it manually builds the request inline
- If changing payment behavior, inspect BOTH the service and the page model

### `class/PhonePePaymentResponse.cs`
- DTO for PhonePe API responses

### `class/ExceptionHandlingMiddleware.cs`
- Full implementation: logs to `ExceptionLogs`, redirects to `~/Error`
- **NOT registered in `Program.cs`** — not active

### `class/ExceptionLoggingFilter.cs`
- Action filter implementation
- **NOT registered globally in `Program.cs`** — not active

### `class/OrderMail.cs`
- Mail helper for order confirmation emails

### `class/BlogQuickView.cs`, `class/QuickViews.cs`
- DTOs for quick-view API responses

### `class/CombinedReviews.cs`
- DTO combining review data

### `class/Exist.cs`
- Utility class (check if item exists)

### `class/Item.cs`
- Cart/wishlist item DTO used throughout session JSON

### `class/SearchFilter.cs`
- Search filter DTO

### `Helpers/SessionHelper.cs`
- Namespace: `Astaberry.Helpers` ← **legacy namespace, not `CrystalByRiya.Helpers`**
- `SetObjectAsJson(ISession, string, object)` — JSON serialize to session
- `GetObjectFromJson<T>(ISession, string)` — JSON deserialize from session
- Used throughout: `using Astaberry.Helpers;`

### `ViewComponents/ProductsViewComponent.cs`
- Namespace: `Viraj.ViewComponents` ← legacy namespace
- Registered as `[ViewComponent(Name = "Category")]`
- Fetches all categories, passes `ChildViewModel` to view named `"Category"`

### `RedirectToWwwRule.cs`
- Implements `IRule` for www redirect
- Its registration in `Program.cs` is commented out

---

## 7. Representative Feature Map

### Storefront Pages

**Core Shopping Flow:**

| Page | File(s) | Description |
|------|---------|-------------|
| Homepage | `Pages/Index.cshtml(.cs)` | Featured products, banners, blogs, announcements, Instagram |
| Product List | `Pages/productlist.cshtml(.cs)` | Category listing with banners, sort/filter, pagination |
| Product Detail | `Pages/detail.cshtml(.cs)` | Gallery, variants, reviews, add-to-cart/wishlist |
| Cart | `Pages/cart.cshtml(.cs)` | Session/DB cart, coupon, quantity update, delete |
| Checkout | `Pages/checkout.cshtml(.cs)` | Billing form, PhonePe payment |
| Thank You | `Pages/thankyou.cshtml(.cs)` | Payment verification, order completion |

**Account Management:**

| Page | File(s) | Description |
|------|---------|-------------|
| My Account | `Pages/myaccount.cshtml(.cs)` | Registration + login |
| Dashboard | `Pages/dashboard.cshtml(.cs)` | Order history, profile, logout |
| Shop Account | `Pages/shop-account.cshtml(.cs)` | Account management, billing pre-fill |
| Forget Password | `Pages/ForgetPassword.cshtml(.cs)` | Password reset request |
| Reset Password | `Pages/ResetPassword.cshtml(.cs)` | Token verification + new password |

**Content Pages:**

| Page | File(s) | Description |
|------|---------|-------------|
| Wishlist | `Pages/wishlist.cshtml(.cs)` | Wishlist management |
| Blog List | `Pages/blog.cshtml(.cs)` | Blog listing |
| Blog Detail | `Pages/blogdetail.cshtml(.cs)` | Post with comments |
| Contact | `Pages/contactus.cshtml(.cs)` | Contact form |
| Search | `Pages/SearchResult.cshtml(.cs)` | Product search results |

**Informational:**

| Page | File | Description |
|------|------|-------------|
| About Us | `Pages/AboutUs.cshtml(.cs)` | Company info |
| Shipping | `Pages/shipping.cshtml(.cs)` | Shipping policies |
| FAQ | `Pages/faq.cshtml` | Frequently asked questions (no `.cs` file) |
| Exchange | `Pages/exchange.cshtml` | Exchange/return policy (no `.cs` file) |
| Privacy | `Pages/Privacy.cshtml` | Privacy policy (no `.cs` file) |
| Terms | `Pages/termandconditions.cshtml(.cs)` | Terms and conditions |
| Rewards | `Pages/rewards.cshtml(.cs)` | Rewards information |
| Admin Login | `Pages/adminlogin.cshtml(.cs)` | Admin login (customer-facing path) |

**System/Legacy:**

| Page | File(s) | Description |
|------|---------|-------------|
| Error | `Pages/Error.cshtml(.cs)` | Primary error page |
| Error1 | `Pages/Error1.cshtml(.cs)` | Secondary error page |
| Index1 | `Pages/Index1.cshtml(.cs)` | Legacy PhonePe demo flow |
| Category | `Pages/category.cshtml(.cs)` | Legacy category page (kept, not linked prominently) |

### Admin Sections

```
Areas/Admin/Pages/
├── Index.cshtml(.cs)          ← admin dashboard
├── Products/                  ← full CRUD + CategoryAndSubCategory + Sizes
├── Category/                  ← full CRUD
├── Subcategories/             ← full CRUD
├── Banners/                   ← full CRUD
├── Blog/                      ← full CRUD
├── BlogsFaq/
├── Coupon/                    ← full CRUD
├── Gallery/                   ← full CRUD
├── Review/                    ← full CRUD
├── ReviewGalleries/
├── Announcements/
├── ProductFaqs/
├── Instagrams/
├── Relatedproducts/
├── Reports/                   ← Index only
├── AddOns/                    ← full CRUD
├── Materials/                 ← full CRUD + Edit.razor placeholder
├── Employee/                  ← full CRUD
├── Comments/
├── Wishlist/
├── AddToCart/
├── Orders/                    ← BillingDetails + OrderDetails
├── Productsizes/              ← full CRUD
└── Page/                      ← empty placeholder
```

### API Controllers (`Api/`)

| Controller | Purpose |
|-----------|---------|
| `BlogQuickViewController.cs` | Blog quick-view data |
| `CheckRegisterController.cs` | Check if email already registered |
| `GetInfoController.cs` | General info lookup |
| `ImageGalleryApiController.cs` | Product image gallery data |
| `NewsletterController.cs` | Newsletter subscription |
| `PriceSliderController.cs` | Price range filter data |
| `ProductsApiController.cs` | Product data |
| `ProductSizesApiController.cs` | Product size data |
| `QuickViewController.cs` | Product quick-view |
| `RatingController.cs` | Product ratings |
| `ReviewController.cs` | Product reviews |
| `SearchController.cs` | Product search |
| `UpdateCartQtyController.cs` | Cart quantity update (AJAX) |
| `WishlistUpdateQtyController.cs` | Wishlist quantity update (AJAX) |
| `YoursOrdersController.cs` | Customer order history |

---

## 8. Frontend Notes

### 8.1 Asset Locations

| Path | Contents |
|------|----------|
| `wwwroot/css/` | Bootstrap, Swiper, Animate, styles.css |
| `wwwroot/js/` | main.js, third-party scripts |
| `wwwroot/assetsv2/` | Theme asset bundle |
| `wwwroot/Admin/` | Admin panel assets |
| `wwwroot/images/` | Local images (logo, products, etc.) |
| `wwwroot/fonts/` | Web fonts |
| `wwwroot/icon/` | Icon fonts (icomoon) |
| `wwwroot/forms/` | Brevo/Sendinblue form embed assets |
| `wwwroot/Image1/` | Tracked empty folder |
| `wwwroot/Xml/` | Tracked empty folder |
| `wwwroot/s/` | Additional asset folder |

### 8.2 Key CSS Classes to Know

- `.three-column-menu` — category nav dropdown grid (defined in `styles.css`)
- `.remove` — mini-cart remove button (bound in `main.js`); **do not reuse on cart page delete links**
- `.preload`, `.preload-container` — page preloader overlay

### 8.3 Storefront UI Notes

- `_Layout.cshtml` is 1418 lines and stateful — treat carefully
- `Pages/productlist.cshtml` renders the category banner inside the page-title hero area; CSS lives in `wwwroot/css/styles.css`
- `Pages/cart.cshtml` delete link must use a class distinct from `.remove` (which `main.js` intercepts); the current page uses `cart_remove`, which is the safe pattern to follow
- Before touching cart/wishlist remove buttons, read `wwwroot/js/main.js`
- The mobile toolbar "Account" link goes to `/adminlogin` — this appears to be a bug/leftover that should route to `/myaccount`
- Quick-view modal in `_Layout.cshtml` still has placeholder/hardcoded product images (not dynamic)
- Footer social links: Facebook and Instagram point to production URLs; X and TikTok are still `href="#"` placeholders

---

## 9. Configuration Files

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "CrystalByRiyaConnection": "Server=DESKTOP-OI7KVCP;Database=DrAstrocrystal;..."
  },
  "SMTP": {
    "From": "somillohani@gmail.com",
    "SmtpServer": "smtp.gmail.com",
    "Port": 465,
    "Username": "somillohani@gmail.com",
    "Password": ""     ← empty, email broken
  }
}
```

Three connection strings present in the file:
1. Active: local Windows auth to `DrAstrocrystal` on `DESKTOP-OI7KVCP`
2. Commented: `EC2AMAZ-BB1P0T9` SQL Express instance
3. Commented: production `3.137.93.229` SQL auth string with credentials in clear text

**Warning:** the commented production SQL auth string contains credentials in the file.

### `appsettings.Development.json`

Minimal, just Logging override.

---

## 10. Current Coding Reality

This project does not follow one clean architectural style. Optimize for safe, local improvements.

**Observed patterns:**

- Session-heavy state management throughout
- Sync DB calls in some page models (e.g., `checkout.cshtml.cs` `OnGet` uses `.FirstOrDefault()` not `await`)
- Business logic lives in page models AND in services under `class/`
- `AddToCartItems` is a scoped service but acts like a page model helper
- Direct `HttpClient` creation in `checkout.cshtml.cs` (not `IHttpClientFactory`)
- `CartModel.OnPostUpdateQuantity` uses `delta`, not an absolute quantity value; the UI increments and decrements from the current value
- Mixed naming conventions (`TblXxx` vs simplified)
- Mixed namespace conventions (`CrystalByRiya.*`, `Astaberry.Helpers`, `Viraj.ViewComponents`)
- Broad try/catch blocks with swallowed exceptions
- Nullable reference warnings common across the repo
- Registration welcome email code is commented out in `myaccount.cshtml.cs`
- `Production()` method in `checkout.cshtml.cs` is dead code (hardcoded placeholder strings)
- Duplicate `ChildskuCode` class in both `cart.cshtml.cs` and `AddToCartItems.cs`
- `checkout.cshtml.cs` only exposes India as a country choice; the state list is hardcoded for India only
- Shipping is calculated after coupon discount in both cart and checkout when a coupon is active
- Wishlist behavior is split between a session-first page (`Pages/wishlist.cshtml`) and a dashboard AJAX tab (`Pages/dashboard.cshtml`); keep both in sync when changing wishlist storage or rendering

**When contributing:**

- Match surrounding conventions unless intentionally cleaning up
- Prefer improving the touched area rather than introducing a second parallel pattern
- If a flow depends on session, trace ALL related session keys before changing it

---

## 11. Rules For Safe Changes

### Do

- Read both `.cshtml` and `.cshtml.cs` for any page you change
- Inspect `Program.cs` before changing startup-sensitive behavior
- Check session keys before changing account, cart, wishlist, checkout, or admin flows
- Trace stored procedure mappings before changing product data flows
- Inspect `wwwroot/js/main.js` before changing cart, wishlist, compare, or mini-cart remove interactions
- Preserve existing route/query conventions used by frontend scripts
- Use async database APIs where practical in new code (`await`, `FirstOrDefaultAsync`, etc.)
- Keep anti-forgery in mind for POST forms and AJAX
- Verify with `dotnet build` after meaningful changes
- Check whether a feature already has a legacy controller or alternate page-model path before refactoring only one half
- For wishlist changes, inspect both `Pages/wishlist.cshtml(.cs)` and `Pages/dashboard.cshtml(.cs)` because they use different render paths for the same data

### Do Not

- Assume ASP.NET Core auth is configured — `UseAuthentication()` is registered but `AddAuthentication()` is not
- Assume `ExceptionHandlingMiddleware` is active in the pipeline
- Assume `ExceptionLoggingFilter` is active globally
- Assume all media comes from local `wwwroot` — images are on CloudFront/S3
- Assume `PhonePePaymentService` is the only payment code path — checkout builds its own request
- Hard-delete or rewrite admin/session behavior without tracing dependent pages
- Manually edit EF migration history unless explicitly required
- Reuse the `.remove` CSS class on cart page delete links
- Assume the `ChildskuCode` class in `cart.cshtml.cs` and the one in `AddToCartItems.cs` are the same type
- Assume checkout has a multi-country selector; it is hardcoded to India only

---

## 12. Known Risks and Mismatches

| Risk | Location |
|------|----------|
| Hardcoded AWS credentials | `class/AwsCredentials.cs` |
| Hardcoded PhonePe production secrets (salt key, merchant ID) | `class/PhonePeCredientials.cs` |
| Plaintext password stored in session | `myaccount.cshtml.cs`, `checkout.cshtml.cs` |
| Commented production SQL auth string with credentials | `appsettings.json` |
| Empty SMTP password in config → email broken | `appsettings.json` → `SMTP.Password` |
| `UseAuthentication()` without `AddAuthentication()` | `Program.cs` |
| Exception middleware exists but not in pipeline | `class/ExceptionHandlingMiddleware.cs` |
| Exception logging filter not wired globally | `class/ExceptionLoggingFilter.cs` |
| Frontend layout contains DB access + session parsing | `Pages/Shared/_Layout.cshtml` |
| Admin auth is session/manual only, and the active login page reads `TblAdmin` | `Pages/adminlogin.cshtml.cs` + all admin pages |
| `.remove` JS handler intercepts cart delete links | `wwwroot/js/main.js` |
| `AddtoWishlist.cs` vs `AddToWishlists.cs` naming confusion | `class/` directory |
| `ChildskuCode` class duplicated | `cart.cshtml.cs` and `AddToCartItems.cs` |
| Dead `Production()` method with placeholder strings | `checkout.cshtml.cs` |
| Welcome email commented out | `myaccount.cshtml.cs::OnPost` |
| Quick-view modal uses static placeholder product images | `_Layout.cshtml` |
| Mobile toolbar "Account" links to `/adminlogin` | `_Layout.cshtml` toolbar |
| Legacy namespace `Astaberry.Helpers` still in use | `Helpers/SessionHelper.cs` |
| Legacy namespace `Viraj.ViewComponents` still in use | `ViewComponents/ProductsViewComponent.cs` |
| `MimeKit` 4.12.0 known vulnerability (`NU1902`) | `CrystalByRiya.csproj` |
| Shipping threshold hardcoded independently in cart AND checkout | `cart.cshtml.cs`, `checkout.cshtml.cs` |
| Subtotal formula not shared between cart and checkout | `cart.cshtml.cs`, `checkout.cshtml.cs` |
| Wishlist is split between session-first listing and dashboard AJAX rendering | `Pages/wishlist.cshtml(.cs)`, `Pages/dashboard.cshtml(.cs)` |
| Placeholder folders tracked in the project file (`Pages/Components/NewFolder`, `wwwroot/ProductImage`) | `CrystalByRiya.csproj` |

---

## 13. Working Commands

Run from the project root (`CrystalByRiya/`):

```powershell
# Restore, build, run
dotnet restore
dotnet build
dotnet run
dotnet watch run

# EF Core migrations
dotnet ef migrations list
dotnet ef migrations add <DescriptiveName>
dotnet ef database update
dotnet ef migrations remove
```

---

## 14. Quick File Reference

| File | Purpose |
|------|---------|
| `Program.cs` | Startup and service registration |
| `appsettings.json` | Connection strings, SMTP config |
| `CrystalByRiya.csproj` | Package references and folder tracking |
| `RedirectToWwwRule.cs` | www redirect rule (currently disabled) |
| `Models/ApplicationDbContext.cs` | EF Core context, all DbSets, SP mappings |
| `Models/ViewModel/ChildViewModel.cs` | Category nav view model |
| `Pages/Shared/_Layout.cshtml` | Customer layout + session/DB-driven header |
| `Areas/Admin/Shared/_Layout.cshtml` | Admin layout + session gate |
| `Pages/_ViewImports.cshtml` | Global namespace imports |
| `Pages/Index.cshtml(.cs)` | Homepage |
| `Pages/myaccount.cshtml.cs` | Customer registration + login |
| `Pages/adminlogin.cshtml.cs` | Admin login (`TblAdmin`, `EmailId` + `Password`) |
| `Pages/ForgetPassword.cshtml.cs` | Password reset email/token request |
| `Pages/ResetPassword.cshtml.cs` | Password reset completion |
| `Pages/cart.cshtml.cs` | Cart display, coupon, delete, quantity update |
| `Pages/checkout.cshtml.cs` | Checkout form + PhonePe request creation |
| `Pages/thankyou.cshtml.cs` | Payment verification, order persistence, email |
| `Pages/dashboard.cshtml.cs` | Customer dashboard, logout |
| `Pages/productlist.cshtml.cs` | Product listing with banners |
| `Pages/detail.cshtml.cs` | Product detail page |
| `Pages/Components/Category/Category.cshtml(.cs)` | Razor view used by the category ViewComponent |
| `Pages/Index1.cshtml.cs` | Legacy PhonePe demo (treat as legacy) |
| `class/AddToCartItems.cs` | Cart + buy-now add helper |
| `class/AddtoWishlist.cs` | Wishlist add helper |
| `class/AddToWishlists.cs` | Wishlist DTO/model class |
| `class/Item.cs` | Session cart/wishlist item DTO |
| `class/AmazonS3.cs` | S3 upload/delete operations |
| `class/AwsCredentials.cs` | Hardcoded AWS config |
| `class/PhonePeCredientials.cs` | Hardcoded PhonePe production config |
| `class/PhonePePaymentService.cs` | Payment service (not used by active checkout path) |
| `class/PhonePePaymentResponse.cs` | PhonePe API response DTO |
| `class/OrderMail.cs` | Order confirmation email helper |
| `class/ExceptionHandlingMiddleware.cs` | Exception middleware (not wired in pipeline) |
| `class/ExceptionLoggingFilter.cs` | Exception filter (not wired globally) |
| `Helpers/SessionHelper.cs` | JSON session helpers (namespace: `Astaberry.Helpers`) |
| `ViewComponents/ProductsViewComponent.cs` | Category nav ViewComponent |
| `StoredProcedure/ProductBySkuCode.cs` | SP result class for `GetProductBySkuCode` |
| `StoredProcedure/SPCategoryWiseProduct.cs` | SP result class |
| `StoredProcedure/SPSubCategoryWiseProduct.cs` | SP result class |
| `StoredProcedure/SpBestSeller.cs` | SP result class for `SpBestseller` |
| `Controllers/HomeController.cs` | Legacy payment/redirect controller |
| `Controllers/ReturnController.cs` | Legacy payment status controller |

---

## 15. Editing Guidance For Future Agents

If you need to understand a feature quickly:

1. Find the page or API entry point in the Feature Map (§7)
2. Read the matching `.cshtml.cs` (or controller)
3. Search for all related session keys used in that file
4. Check whether product data comes from tables (`TblProducts`) or stored procedures (`ProductBySkuCodes`)
5. Inspect `_Layout.cshtml` if behavior involves header, footer, cart count, wishlist count, or category nav
6. Check `wwwroot/js/main.js` if touching any remove/cart/wishlist/compare button classes

If you need to add a new admin section:

1. Create folder under `Areas/Admin/Pages/<SectionName>/`
2. Add standard CRUD pages: `Index`, `Create`, `Edit`, `Delete`, `Details`
3. Add or reuse the model in `Models/`
4. Register DbSet in `ApplicationDbContext.cs` if new entity
5. Add navigation link in `Areas/Admin/Shared/_Layout.cshtml`

If you need to document or refactor:

- Document current behavior first
- Call out mismatches between intended and actual architecture
- Avoid claiming framework auth, service-layer separation, or secret management that the code does not actually implement
- Maintain dynamic data integration patterns already established
- Do not introduce a second pattern for something that already works one way (e.g., don't add `IHttpClientFactory` to half the payment flows)

---

**Last verified against source:** April 6, 2026
