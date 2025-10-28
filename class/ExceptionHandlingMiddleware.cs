using CrystalByRiya.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");

            var pageName = _httpContextAccessor?.HttpContext?.Request?.Path.Value ?? "Unknown";

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (dbContext != null)
                {
                    var exceptionLog = new ExceptionLog
                    {
                        Timestamp = DateTime.UtcNow,
                        ExceptionMessage = ex.Message,
                        StackTrace = ex.StackTrace,
                        Source = ex.Source,
                        PageName = pageName
                    };

                    dbContext.ExceptionLogs.Add(exceptionLog);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    _logger.LogError("ApplicationDbContext could not be resolved.");
                }
            }

            context.Response.Redirect("~/Error");
        }
    }
}
        

