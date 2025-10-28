using Azure;
using CrystalByRiya.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class ExceptionLoggingFilter : IAsyncExceptionFilter
{
    private readonly ILogger<ExceptionLoggingFilter> _logger;
    private readonly ApplicationDbContext _dbContext;

    public ExceptionLoggingFilter(ILogger<ExceptionLoggingFilter> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "An unhandled exception occurred.");

        // Capture page or route information
        var pageName = context.HttpContext.Request.Path.Value;

        // Log to database
        var exceptionLog = new ExceptionLog
        {
            Timestamp = DateTime.UtcNow,
            ExceptionMessage = context.Exception.Message,
            StackTrace = context.Exception.StackTrace,
            Source = context.Exception.Source,
            PageName = pageName // Include page name or route
        };

        _dbContext.ExceptionLogs.Add(exceptionLog);
        await _dbContext.SaveChangesAsync();

        context.Result = new RedirectToPageResult("/Error");
        context.ExceptionHandled = true; // Mark exception as handled

    }
}

