using CrystalByRiya;
using CrystalByRiya.@class;
using CrystalByRiya.Classes;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using  CrystalByRiya.Classes;
using System.IO.Compression;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<AddToWishlistModel>();
builder.Services.AddScoped<AddToCartItems>();
builder.Services.AddScoped<AwsCredentials>();
builder.Services.AddScoped<AmazonS3>();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;  // Enable compression for HTTPS requests as well
    options.Providers.Add<GzipCompressionProvider>();  // Add GZIP compression
    options.Providers.Add<BrotliCompressionProvider>();  // Add Brotli compression (optional, modern and efficient)
});



// Add Razor Pages with runtime compilation for dynamic changes
builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

// Add PhonePe payment service (scoped for each request)
builder.Services.AddScoped<PhonePePaymentService>();

// Configure Session with idle timeout of 40 minutes
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(40);  // Adjust idle timeout based on your needs
    options.Cookie.HttpOnly = true;  // Ensure the cookie can't be accessed via client-side script
    options.Cookie.IsEssential = true;  // Mark the session cookie as essential
});

// Add caching services
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();


// Configure file upload size limits
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 300 * 1024 * 1024;  // 300 MB upload limit
});

// Add support for MVC Controllers and Views
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();  // Allow runtime compilation for both pages and views

// Enable Antiforgery to prevent CSRF attacks
builder.Services.AddAntiforgery();

// Configure Entity Framework with SQL Server and Connection String
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CrystalByRiyaConnection"))
);

// Register IHttpContextAccessor to access session, cookies, etc. in services or controllers
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
    builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();  
    });
});
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); 
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();  
}

app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseStaticFiles();
/*var options = new RewriteOptions();
options.AddRedirectToHttps();
options.Rules.Add(new RedirectToWwwRule());
app.UseRewriter(options);
*/
app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseResponseCaching();

app.UseAuthentication();               
app.UseAuthorization();

app.UseSession();                     

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Admin") &&
        string.IsNullOrEmpty(context.Session.GetString("Login")))
    {
        context.Response.Redirect("/adminlogin");
        return;
    }

    await next();
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

app.Run();


// Enable Routing and set the correct URL path format
