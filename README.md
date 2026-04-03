<h1 align="center">
  💎 CrystalByRiya
</h1>

<p align="center">
  A full-stack e-commerce platform for crystal jewelry and crystal-based products — built with ASP.NET Core 9.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet" />
  <img src="https://img.shields.io/badge/ASP.NET_Core-Razor_Pages-blue?style=flat-square" />
  <img src="https://img.shields.io/badge/Database-SQL_Server-CC2927?style=flat-square&logo=microsoftsqlserver" />
  <img src="https://img.shields.io/badge/Cloud-AWS_S3-FF9900?style=flat-square&logo=amazonaws" />
  <img src="https://img.shields.io/badge/Payment-PhonePe-5f259f?style=flat-square" />
  <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" />
</p>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Folder Structure](#-folder-structure)
- [Prerequisites](#-prerequisites)
- [Installation](#-installation)
- [Environment & Configuration](#-environment--configuration)
- [Running the Project](#-running-the-project)
- [Database Setup](#-database-setup)
- [Build & Deployment](#-build--deployment)
- [API Reference](#-api-reference)
- [Authentication](#-authentication)
- [Error Handling & Logging](#-error-handling--logging)
- [Common Issues & Fixes](#-common-issues--fixes)
- [Contributing](#-contributing)
- [License](#-license)
- [Contact](#-contact)

---

## 🔍 Overview

**CrystalByRiya** is a production-grade, monolithic e-commerce web application serving customers who purchase crystal jewelry and wellness products online. It features a **customer-facing storefront**, a **protected admin panel**, **PhonePe payment integration**, **AWS S3 media storage**, and **transactional email** via Gmail SMTP.

- **Live Site:** [https://www.crystalsbyriya.com](https://www.crystalsbyriya.com)
- **Architecture:** Monolith — ASP.NET Core Razor Pages (customer) + MVC (admin + API)
- **Target Users:** End customers + internal store admins

---

## ✨ Features

### Customer Storefront
- 🛍️ Product catalog with categories, subcategories, and filters
- 🔍 Smart search with price range slider
- 📦 Product detail with size variants, add-ons, and materials
- 🛒 Shopping cart with quantity management
- ❤️ Wishlist
- 💳 Checkout with PhonePe payment gateway
- 📧 Order confirmation emails
- 🔒 Forgot / Reset password via email OTP
- ✍️ Blog with comments and FAQs
- 🏷️ Coupon / discount code support
- 📱 Quick-view product modal

### Admin Panel (`/Admin`)
- 📊 Dashboard with order & revenue stats
- 🗂️ Full CRUD — Products, Categories, Subcategories, Blogs, Banners, Gallery
- 📦 Orders management with status tracking
- 👥 Customer management
- 🏷️ Coupon & discount management
- 🌟 Review & testimonial moderation
- 👷 Employee management
- 📢 Announcement system for storefront
- 🧾 Reports section

---

## 🛠️ Tech Stack

| Layer | Technology | Version |
|---|---|---|
| Runtime | .NET | **9.0** |
| Framework | ASP.NET Core (Razor Pages + MVC) | **9.0** |
| ORM — Primary | Entity Framework Core | **9.0.5** |
| ORM — Secondary | Dapper | **2.1.66** |
| Database | Microsoft SQL Server | 2019+ |
| Authentication | ASP.NET Core Cookie Auth | Built-in |
| Cloud Storage | AWS S3 + CloudFront CDN | SDK **4.0.0.4** |
| Payment Gateway | PhonePe | REST API |
| Email | MailKit + MimeKit | **4.12.0** |
| HTTP Client | RestSharp | **112.1.0** |
| JSON | Newtonsoft.Json | **13.0.3** |
| Password Hashing | BCrypt.Net-Next | **4.0.2** |
| Compression | Gzip + Brotli | Built-in |
| Frontend | HTML, CSS, JavaScript, Razor | — |

---

## 📁 Folder Structure

```
CrystalByRiya/
├── Api/                          # Internal AJAX API controllers (15 controllers)
├── Areas/
│   └── Admin/                    # Protected admin panel
│       ├── Pages/                # Admin Razor Pages (CRUD)
│       │   ├── Products/
│       │   ├── Category/
│       │   ├── Orders/
│       │   ├── Blog/
│       │   ├── Coupons/
│       │   ├── Banners/
│       │   ├── Gallery/
│       │   ├── Review/
│       │   ├── Employee/
│       │   ├── Materials/
│       │   ├── Subcategories/
│       │   └── Announcements/
│       └── Shared/               # Admin layout & partials
│
├── class/                        # Business logic & service classes
│   ├── AmazonS3.cs               # S3 upload / delete
│   ├── AwsCredentials.cs         # AWS config (⚠️ move keys to secrets)
│   ├── PhonePePaymentService.cs  # Payment initiation & verification
│   ├── AddToCartItems.cs         # Cart operations
│   ├── AddtoWishlist.cs          # Wishlist operations
│   ├── ExceptionHandlingMiddleware.cs
│   ├── OrderMail.cs              # Email model for orders
│   └── SearchFilter.cs
│
├── Controllers/                  # MVC controllers
│   ├── HomeController.cs
│   └── ReturnController.cs
│
├── Helpers/
│   └── SessionHelper.cs          # Session extension methods
│
├── Models/                       # EF Core entities + DbContext (75 models)
│   ├── ApplicationDbContext.cs   # ← Central DbContext
│   ├── TblProduct.cs
│   ├── TblCart.cs
│   ├── TblOrderId.cs
│   └── ViewModel/
│       └── ChildViewModel.cs
│
├── Pages/                        # Customer-facing Razor Pages
│   ├── Index.cshtml(.cs)         # Homepage
│   ├── productlist.cshtml(.cs)   # Shop / product listing
│   ├── detail.cshtml(.cs)        # Product detail
│   ├── cart.cshtml(.cs)
│   ├── checkout.cshtml(.cs)
│   ├── thankyou.cshtml(.cs)      # Payment confirmation
│   ├── wishlist.cshtml(.cs)
│   ├── blog.cshtml(.cs)
│   ├── blogdetail.cshtml(.cs)
│   ├── dashboard.cshtml(.cs)     # Customer account
│   ├── SearchResult.cshtml(.cs)
│   └── Shared/                   # _Layout.cshtml, partials
│
├── StoredProcedure/              # Dapper result POCOs for SQL SPs
│   ├── ProductBySkuCode.cs
│   ├── SPCategoryWiseProduct.cs
│   └── SpBestSeller.cs
│
├── ViewComponents/
│   └── ProductsViewComponent.cs
│
├── wwwroot/                      # Static public files
│   ├── css/
│   ├── js/
│   ├── fonts/
│   ├── Images/                   # Local section banners
│   └── assetsv2/                 # Third-party UI libraries
│
├── Program.cs                    # Entry point & DI configuration
├── appsettings.json
├── appsettings.Development.json
├── AGENTS.md                     # AI agent guide for this project
└── CrystalByRiya.csproj
```

---

## ✅ Prerequisites

Ensure the following are installed before starting:

| Tool | Minimum Version | Download |
|---|---|---|
| .NET SDK | **9.0** | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| SQL Server | 2019+ | [microsoft.com/sql-server](https://www.microsoft.com/sql-server) |
| SQL Server Management Studio | Any | Optional but recommended |
| Visual Studio / VS Code | Latest | With C# extension |
| Git | Any | [git-scm.com](https://git-scm.com) |

---

## ⚙️ Installation

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/CrystalByRiya.git
cd CrystalByRiya/CrystalByRiya
```

### 2. Restore NuGet Packages

```bash
dotnet restore
```

### 3. Configure the Application

Copy the example config and fill in your values:

```bash
# appsettings.Development.json is already present
# Edit it for your local environment (see Configuration section below)
```

### 4. Set Up the Database

```bash
# Apply EF Core migrations to create the schema
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
# or with hot-reload
dotnet watch run
```

App starts at: `https://localhost:5001` · `http://localhost:5000`

---

## 🔧 Environment & Configuration

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "CrystalByRiyaConnection": "<your-connection-string>"
  },
  "SMTP": {
    "From": "your-email@gmail.com",
    "SmtpServer": "smtp.gmail.com",
    "Port": 465,
    "Username": "your-email@gmail.com",
    "Password": "<gmail-app-password>"
  },
  "PhonePe": {
    "MerchantId": "<phonepe-merchant-id>",
    "MerchantKey": "<phonepe-merchant-key>",
    "BaseUrl": "https://api.phonepe.com/apis/hermes"
  }
}
```

> ⚠️ **Never commit real secrets** to source control. Use User Secrets for local development (see below).

### Local Development — Connection Strings

```json
// Local (Windows Auth)
"CrystalByRiyaConnection": "Server=YOUR_PC_NAME;Database=DrAstrocrystal;Trusted_Connection=True;MultipleActiveResultSets=true;connection timeout=100;TrustServerCertificate=True;"

// Production (SQL Auth)
"CrystalByRiyaConnection": "Server=YOUR_SERVER_IP,1433;Database=CrystalByRiya;User Id=YOUR_USER;password=YOUR_PASSWORD;TrustServerCertificate=True;"
```

> Only **one** connection string should be uncommented at a time.

### Managing Secrets (Recommended)

```bash
# Initialize .NET User Secrets
dotnet user-secrets init

# AWS credentials
dotnet user-secrets set "AwsCredentials:AccessKey" "AKIA..."
dotnet user-secrets set "AwsCredentials:SecretKey" "..."

# SMTP
dotnet user-secrets set "SMTP:Password" "your-gmail-app-password"

# PhonePe
dotnet user-secrets set "PhonePe:MerchantKey" "..."
```

| Setting | Local | Production |
|---|---|---|
| Database | Windows Auth, `DrAstrocrystal` | SQL Auth, `CrystalByRiya` |
| AWS Keys | User Secrets | Environment Variables / AWS IAM Role |
| SMTP Password | User Secrets | Environment Variables |
| PhonePe URL | Sandbox URL | `https://api.phonepe.com/apis/hermes` |

---

## ▶️ Running the Project

```bash
# Development (with auto-reload)
dotnet watch run

# Production mode locally
dotnet run --environment Production

# Specify a custom port
dotnet run --urls "https://localhost:7001;http://localhost:5001"
```

**Default URLs:**

| Environment | URL |
|---|---|
| HTTP | `http://localhost:5000` |
| HTTPS | `https://localhost:5001` |
| Admin Panel | `https://localhost:5001/Admin` |

---

## 🗄️ Database Setup

### EF Core Migrations

```bash
# Add a new migration
dotnet ef migrations add <DescriptiveName>
# e.g.
dotnet ef migrations add AddProductTagsColumn

# Apply all pending migrations
dotnet ef database update

# Rollback to a specific migration
dotnet ef database update <PreviousMigrationName>

# List all migrations
dotnet ef migrations list

# Remove last unapplied migration
dotnet ef migrations remove
```

> **Rule:** Migration names must be descriptive — e.g., `AddCouponExpiryDate`, not `Mig1`.

### Database Names

| Environment | Database |
|---|---|
| Local Development | `DrAstrocrystal` |
| Production (AWS EC2) | `CrystalByRiya` |

### Stored Procedures

- SQL Stored Procedures live **inside SQL Server** (not in project files).
- C# result-mapping POCOs live in `StoredProcedure/`.
- Registered in `ApplicationDbContext.cs` via `.HasNoKey().ToView(null)`.

**Calling a Stored Procedure (Dapper):**

```csharp
using var connection = new SqlConnection(_configuration.GetConnectionString("CrystalByRiyaConnection"));
var result = await connection.QueryAsync<ProductBySkuCode>(
    "GetProductBySkuCode",
    new { PCode = skuCode },
    commandType: CommandType.StoredProcedure
);
```

**Calling a Stored Procedure (EF Core):**

```csharp
var result = await _context.ProductBySkuCodes
    .FromSqlRaw("EXEC GetProductBySkuCode @PCode", new SqlParameter("@PCode", skuCode))
    .ToListAsync();
```

---

## 🚀 Build & Deployment

### Build

```bash
# Debug build
dotnet build

# Release build
dotnet build -c Release
```

### Publish

```bash
# Publish for Windows Server (framework-dependent)
dotnet publish -c Release -o ./publish

# Publish as self-contained (no .NET runtime needed on server)
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish
```

### Deploy to IIS (Windows Server)

1. Publish the app: `dotnet publish -c Release -o ./publish`
2. Copy `./publish/` to the server.
3. Create an IIS Site pointing to the publish folder.
4. Install the **.NET 9 Hosting Bundle** on the server.
5. Set environment variable `ASPNETCORE_ENVIRONMENT=Production` in IIS.
6. Set up HTTPS with an SSL certificate (Let's Encrypt or AWS ACM).

### Deploy to AWS EC2

1. SSH into your EC2 instance.
2. Install .NET 9 Runtime: `sudo apt install dotnet-runtime-9.0`
3. Copy published files via SCP or AWS CodeDeploy.
4. Configure Nginx as a reverse proxy to forward to `localhost:5000`.
5. Use `systemd` to run the app as a service.

---

## 📡 API Reference

All AJAX endpoints are in the `Api/` folder under the `/api/[controller]` route prefix.

### Quick-View Product

```
GET /api/QuickView/GetQuickView?skucode={sku}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "productId": "CBR-001",
    "productName": "Rose Quartz Bracelet",
    "price": 599.00,
    "description": "...",
    "image": ["url1", "url2"],
    "size": ["S:499:url", "M:599:url"],
    "addOn": ["Gift Box:99"],
    "materialName": ["Silver:699:url"]
  }
}
```

### Search Products

```
GET /api/Search?q={query}
```

### Update Cart Quantity

```
POST /api/UpdateCartQty
Content-Type: application/json
RequestVerificationToken: <antiforgery-token>

{ "productId": "CBR-001", "qty": 2 }
```

### Price Slider Filter

```
GET /api/PriceSlider?minPrice={min}&maxPrice={max}&categoryId={id}
```

### Newsletter Subscription

```
POST /api/Newsletter
Content-Type: application/json

{ "email": "user@example.com" }
```

### Check Registration

```
GET /api/CheckRegister?email={email}
```

---

## 🔐 Authentication

### Customer Authentication
- **Scheme:** Cookie-based (`CookieAuthenticationDefaults.AuthenticationScheme`)
- **Role:** `Customer`
- **Login page:** `/shop-account`
- **Cookie:** HttpOnly, Secure, 40-min session idle timeout

### Admin Authentication
- **Scheme:** Cookie-based with Admin role check
- **Role:** `Admin`
- **Login page:** `/adminlogin`
- **Protected by:** `[Authorize(Roles = "Admin")]` on all admin PageModels

### Session (Guest Users)
- Cart and wishlist are stored in **server-side session** before login.
- Session helper: `Helpers/SessionHelper.cs` — `SetObjectAsJson` / `GetObjectFromJson`.
- Session timeout: **40 minutes**.

### PhonePe Payment Flow
1. Checkout page collects billing/shipping details.
2. `PhonePePaymentService.InitiatePaymentAsync()` builds the signed payload and calls the PhonePe API.
3. User is redirected to PhonePe's hosted payment page.
4. After payment, PhonePe redirects to `/thankyou`.
5. `thankyou.cshtml.cs` calls `VerifyPaymentAsync()` to confirm and saves order to `TblPaymentHistory`.

---

## ⚠️ Error Handling & Logging

- **Global middleware:** `ExceptionHandlingMiddleware.cs` catches all unhandled exceptions.
- **Logging:** Exceptions are persisted to the `ExceptionLogs` table in SQL Server (with `Timestamp`, `ExceptionMessage`, `StackTrace`, `Source`, `PageName`).
- **User redirect:** All unhandled errors redirect to `/Error`.
- **Dev mode:** Full stack trace shown via `UseDeveloperExceptionPage()`.

---

## 🐛 Common Issues & Fixes

### ❌ `dotnet ef` not found

```bash
dotnet tool install --global dotnet-ef
# Then verify
dotnet ef --version
```

---

### ❌ SSL Certificate Error on `localhost`

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

---

### ❌ Database connection fails locally

- Ensure SQL Server service is running.
- Check the server name in your connection string matches your machine name.
- Use `Trusted_Connection=True` for Windows Auth on local.
- Ensure `TrustServerCertificate=True` is set.

---

### ❌ Images not loading after upload

- Images are served via **AWS CloudFront**, not `wwwroot`.
- Check that `AwsCredentials.cs` has the correct `bucketName` and `cloudFrontURL`.
- Verify S3 bucket CORS policy allows your domain.
- Verify CloudFront distribution is enabled.

---

### ❌ PhonePe payment fails

- Confirm `MerchantId` matches your PhonePe account exactly.
- Amount is sent in **paise** (multiply ₹ amount × 100).
- `RedirectUrl` and `CallbackUrl` must be HTTPS in production.
- For sandbox testing, use `https://api-preprod.phonepe.com/apis/pg-sandbox/pg/v1/pay`.

---

### ❌ Email not being sent

- In Gmail: Enable 2FA, then generate an **App Password** (not your account password).
- Set `SMTP:Port` to `465` (SSL) or `587` (STARTTLS).
- Confirm `SMTP:Username` and `SMTP:Password` match in `appsettings.json`.

---

### ❌ Session not persisting

- Ensure `app.UseSession()` is called **before** `app.UseAuthorization()` in `Program.cs`.
- Check `options.IdleTimeout` — defaults to `40 minutes`.
- Cookie must be marked `IsEssential = true` for GDPR compliance.

---

## 🤝 Contributing

1. **Fork** the repository
2. **Create** a feature branch: `git checkout -b feature/your-feature-name`
3. **Follow** the coding conventions described in [AGENTS.md](./AGENTS.md)
4. **Commit** with a meaningful message: `git commit -m "feat: add product tag filter"`
5. **Push** to your fork: `git push origin feature/your-feature-name`
6. **Open** a Pull Request against `main`

### Commit Message Convention

```
feat:     New feature
fix:      Bug fix
docs:     Documentation update
refactor: Code refactoring (no functional change)
style:    Formatting / whitespace
chore:    Build/config changes
```

### Code Standards (Read Before Contributing)

- Read `AGENTS.md` fully before making changes.
- Always use `async/await` — never `.Result` or `.Wait()`.
- Business logic goes in `class/` services, not in `.cshtml` views.
- All product/media images must go through `AmazonS3.UploadFileToS3()`.
- Never manually edit files in the `Migrations/` folder.
- Run `dotnet build` and confirm no warnings before submitting a PR.

---

## 📜 License

This project is licensed under the **MIT License**.

```
MIT License

Copyright (c) 2024 CrystalByRiya

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions...
```

See [LICENSE](./LICENSE) for full text.

---

## 📬 Contact

| Role | Name | Contact |
|---|---|---|
| Project Owner | CrystalByRiya | [crystalsbyriya.com](https://www.crystalsbyriya.com) |
| Developer | — | somillohani@gmail.com |

---

<p align="center">
  Made with ❤️ for crystal lovers everywhere 💎
</p>
