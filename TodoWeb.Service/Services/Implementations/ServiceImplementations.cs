using TodoWeb.Service.Services.Abstractions;

namespace TodoWeb.Service.Services.Implementations
{
    /// <summary>
    /// Default implementation of IDateTimeProvider
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Today => DateTime.Today;
    }

    /// <summary>
    /// Default implementation of IFileService
    /// </summary>
    public class FileService : IFileService
    {
        public Task<string> ReadAllTextAsync(string path) => File.ReadAllTextAsync(path);
        public Task WriteAllTextAsync(string path, string content) => File.WriteAllTextAsync(path, content);
        public Task AppendAllTextAsync(string path, string content) => File.AppendAllTextAsync(path, content);
        public bool Exists(string path) => File.Exists(path);
        public string Combine(params string[] paths) => Path.Combine(paths);
    }

    /// <summary>
    /// Default implementation of IConsoleService
    /// </summary>
    public class ConsoleService : IConsoleService
    {
        public void WriteLine(string message) => Console.WriteLine(message);
        public void Write(string message) => Console.Write(message);
        public string ReadLine() => Console.ReadLine() ?? string.Empty;
    }

    /// <summary>
    /// Default implementation of IRandomService
    /// </summary>
    public class RandomService : IRandomService
    {
        private readonly Random _random = new();

        public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);
        public int Next(int maxValue) => _random.Next(maxValue);
        
        public string GenerateString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }

    /// <summary>
    /// Default implementation of IGuidService
    /// </summary>
    public class GuidService : IGuidService
    {
        public Guid NewGuid() => Guid.NewGuid();
        public string NewGuidString() => Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Default implementation of IEnvironmentService
    /// </summary>
    public class EnvironmentService : IEnvironmentService
    {
        public string GetEnvironmentVariable(string variable) => Environment.GetEnvironmentVariable(variable) ?? string.Empty;
        public string CurrentDirectory => Environment.CurrentDirectory;
    }

    /// <summary>
    /// Default implementation of IHttpService
    /// </summary>
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAsync(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        public Task<HttpResponseMessage> GetResponseAsync(string url)
        {
            return _httpClient.GetAsync(url);
        }
    }

    /// <summary>
    /// Default implementation of ILoggerService
    /// </summary>
    public class LoggerService : ILoggerService
    {
        private readonly IFileService _fileService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public LoggerService(IFileService fileService, IDateTimeProvider dateTimeProvider)
        {
            _fileService = fileService;
            _dateTimeProvider = dateTimeProvider;
        }

        public void LogInformation(string message) => Console.WriteLine($"[INFO] {message}");
        public void LogError(string message) => Console.WriteLine($"[ERROR] {message}");
        public void LogWarning(string message) => Console.WriteLine($"[WARNING] {message}");

        public async Task LogToFileAsync(string message, string fileName)
        {
            var timestamp = _dateTimeProvider.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logMessage = $"[{timestamp}] {message}";
            await _fileService.AppendAllTextAsync(fileName, logMessage + Environment.NewLine);
        }
    }

    /// <summary>
    /// Default implementation of IDelayService
    /// </summary>
    public class DelayService : IDelayService
    {
        public Task DelayAsync(int milliseconds) => Task.Delay(milliseconds);
        public Task DelayAsync(TimeSpan timeSpan) => Task.Delay(timeSpan);
    }
}