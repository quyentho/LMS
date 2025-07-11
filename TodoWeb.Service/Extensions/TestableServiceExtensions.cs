using Microsoft.Extensions.DependencyInjection;
using TodoWeb.Service.Services.Abstractions;
using TodoWeb.Service.Services.Implementations;
using TodoWeb.Service.Services.Examples;

namespace TodoWeb.Extensions
{
    /// <summary>
    /// Extension methods for registering testable service dependencies
    /// </summary>
    public static class TestableServiceExtensions
    {
        /// <summary>
        /// Registers all testable service abstractions with their default implementations
        /// </summary>
        public static void AddTestableServices(this IServiceCollection services)
        {
            // Register service abstractions with their default implementations
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IConsoleService, ConsoleService>();
            services.AddScoped<IRandomService, RandomService>();
            services.AddScoped<IGuidService, GuidService>();
            services.AddScoped<IEnvironmentService, EnvironmentService>();
            services.AddScoped<IDelayService, DelayService>();
            
            // Register logger service with its dependencies
            services.AddScoped<ILoggerService, LoggerService>();
            
            // Register HTTP service with HttpClient
            services.AddHttpClient<IHttpService, HttpService>();
            
            // Register the example services
            services.AddScoped<TestableCodeExamples>();
            services.AddScoped<UntestableCodeExamples>();
        }

        /// <summary>
        /// Registers testable services for testing environment with specific configurations
        /// </summary>
        public static void AddTestableServicesForTesting(this IServiceCollection services)
        {
            // In testing, you might want to register specific implementations
            // or use the default ones - this gives you flexibility
            services.AddTestableServices();
            
            // You could also register test-specific implementations here
            // For example, a FileService that works with in-memory storage
            // services.AddScoped<IFileService, InMemoryFileService>();
        }
    }
}