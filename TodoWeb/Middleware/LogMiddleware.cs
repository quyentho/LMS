﻿namespace TodoWeb.Middleware
{
    //rate limitting
    //200 request / s
    //30s => 10 resquest
    public class LogMiddleware : IMiddleware
    {
        private readonly ILogger<LogMiddleware> _logger;
        public LogMiddleware(ILogger<LogMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An unexpected error occurred.");
            }
        }
    }
}
