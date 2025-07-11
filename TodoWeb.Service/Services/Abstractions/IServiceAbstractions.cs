namespace TodoWeb.Service.Services.Abstractions
{
    /// <summary>
    /// Abstraction for DateTime operations to make code testable
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
        DateTime Today { get; }
    }

    /// <summary>
    /// Abstraction for file system operations
    /// </summary>
    public interface IFileService
    {
        Task<string> ReadAllTextAsync(string path);
        Task WriteAllTextAsync(string path, string content);
        Task AppendAllTextAsync(string path, string content);
        bool Exists(string path);
        string Combine(params string[] paths);
    }

    /// <summary>
    /// Abstraction for console operations
    /// </summary>
    public interface IConsoleService
    {
        void WriteLine(string message);
        void Write(string message);
        string ReadLine();
    }

    /// <summary>
    /// Abstraction for random number generation
    /// </summary>
    public interface IRandomService
    {
        int Next(int minValue, int maxValue);
        int Next(int maxValue);
        string GenerateString(int length);
    }

    /// <summary>
    /// Abstraction for GUID generation
    /// </summary>
    public interface IGuidService
    {
        Guid NewGuid();
        string NewGuidString();
    }

    /// <summary>
    /// Abstraction for environment operations
    /// </summary>
    public interface IEnvironmentService
    {
        string GetEnvironmentVariable(string variable);
        string CurrentDirectory { get; }
    }

    /// <summary>
    /// Abstraction for HTTP operations
    /// </summary>
    public interface IHttpService
    {
        Task<string> GetAsync(string url);
        Task<HttpResponseMessage> GetResponseAsync(string url);
    }

    /// <summary>
    /// Abstraction for logging operations
    /// </summary>
    public interface ILoggerService
    {
        void LogInformation(string message);
        void LogError(string message);
        void LogWarning(string message);
        Task LogToFileAsync(string message, string fileName);
    }

    /// <summary>
    /// Abstraction for delay operations
    /// </summary>
    public interface IDelayService
    {
        Task DelayAsync(int milliseconds);
        Task DelayAsync(TimeSpan timeSpan);
    }
}