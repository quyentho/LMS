using System.Net.Http;
using TodoWeb.Application.Dtos.CourseModel;
using TodoWeb.Domains.Entities;
using TodoWeb.DataAccess.Repositories;
using TodoWeb.Service.Services.Abstractions;

namespace TodoWeb.Service.Services.Examples
{
    /// <summary>
    /// Examples of TESTABLE code patterns using dependency injection and abstractions.
    /// These are the refactored versions of the untestable examples.
    /// </summary>
    public class TestableCodeExamples
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IFileService _fileService;
        private readonly IConsoleService _consoleService;
        private readonly IRandomService _randomService;
        private readonly IGuidService _guidService;
        private readonly IEnvironmentService _environmentService;
        private readonly IHttpService _httpService;
        private readonly ILoggerService _loggerService;
        private readonly IDelayService _delayService;

        public TestableCodeExamples(
            ICourseRepository courseRepository,
            IDateTimeProvider dateTimeProvider,
            IFileService fileService,
            IConsoleService consoleService,
            IRandomService randomService,
            IGuidService guidService,
            IEnvironmentService environmentService,
            IHttpService httpService,
            ILoggerService loggerService,
            IDelayService delayService)
        {
            _courseRepository = courseRepository;
            _dateTimeProvider = dateTimeProvider;
            _fileService = fileService;
            _consoleService = consoleService;
            _randomService = randomService;
            _guidService = guidService;
            _environmentService = environmentService;
            _httpService = httpService;
            _loggerService = loggerService;
            _delayService = delayService;
        }

        // 1. TESTABLE TIME DEPENDENCIES - Can be mocked
        public string CreateCourseWithCurrentTime(string courseName)
        {
            // IDateTimeProvider can be mocked in tests
            var currentTime = _dateTimeProvider.Now;
            var course = new Course
            {
                Name = courseName,
                StartDate = currentTime,
                CreateAt = currentTime
            };

            return $"Course '{courseName}' created at {currentTime:yyyy-MM-dd HH:mm:ss}";
        }

        // 2. TESTABLE FILE SYSTEM DEPENDENCIES - Can be mocked
        public async Task<string> SaveCourseToFileAsync(Course course)
        {
            // IFileService can be mocked in tests
            var fileName = $"course_{course.Id}.txt";
            var filePath = _fileService.Combine(_environmentService.CurrentDirectory, fileName);
            
            await _fileService.WriteAllTextAsync(filePath, $"{course.Name} - {course.StartDate}");
            
            return filePath;
        }

        // 3. TESTABLE CONSOLE OUTPUT - Can be mocked and verified
        public void PrintCourseDetails(Course course)
        {
            // IConsoleService can be mocked and verified in tests
            _consoleService.WriteLine($"Course ID: {course.Id}");
            _consoleService.WriteLine($"Course Name: {course.Name}");
            _consoleService.WriteLine($"Start Date: {course.StartDate}");
        }

        // 4. TESTABLE RANDOM NUMBER GENERATION - Deterministic in tests
        public string GenerateCourseCode()
        {
            // IRandomService can be mocked to return predictable values
            return _randomService.GenerateString(6);
        }

        // 5. TESTABLE HTTP CLIENT - Can be mocked
        public async Task<string> FetchCourseDataFromApiAsync(int courseId)
        {
            // IHttpService can be mocked in tests
            return await _httpService.GetAsync($"https://api.example.com/courses/{courseId}");
        }

        // 6. TESTABLE DELAY - Can be mocked to avoid slow tests
        public async Task ProcessCourseWithDelayAsync(Course course)
        {
            _consoleService.WriteLine($"Processing course: {course.Name}");
            
            // IDelayService can be mocked to make tests fast
            await _delayService.DelayAsync(5000);
            
            _consoleService.WriteLine($"Finished processing: {course.Name}");
        }

        // 7. TESTABLE ENVIRONMENT VARIABLES - Can be mocked
        public string GetCourseStoragePath()
        {
            // IEnvironmentService can be mocked in tests
            var basePath = _environmentService.GetEnvironmentVariable("COURSE_STORAGE_PATH");
            return !string.IsNullOrEmpty(basePath) ? basePath : "C:\\DefaultPath";
        }

        // 8. TESTABLE GUID GENERATION - Can be mocked
        public string CreateUniqueStudentId()
        {
            // IGuidService can be mocked to return predictable values
            return _guidService.NewGuidString();
        }

        // 9. TESTABLE FILE VALIDATION - All dependencies can be mocked
        public bool ValidateCourseName(string courseName)
        {
            // All dependencies are now mockable
            return !string.IsNullOrEmpty(courseName) && 
                   courseName.Length > 3 && 
                   _fileService.Exists(_fileService.Combine(_environmentService.CurrentDirectory, "validation.txt"));
        }

        // 10. TESTABLE COMPLEX OPERATIONS - All dependencies injected
        public async Task<Course> CreateCourseWithValidationAsync(string courseName)
        {
            // All dependencies are now mockable and testable
            var course = new Course
            {
                Name = courseName,
                StartDate = _dateTimeProvider.Now.AddDays(7),
                CreateAt = _dateTimeProvider.UtcNow,
                Id = Math.Abs(_guidService.NewGuid().GetHashCode())
            };

            // File logging through abstraction
            var logPath = _fileService.Combine(_environmentService.CurrentDirectory, "course_log.txt");
            await _loggerService.LogToFileAsync($"Course created: {course.Name}", logPath);

            // Console output through abstraction
            _consoleService.WriteLine($"New course created: {course.Name}");

            return course;
        }

        // 11. TESTABLE EXCEPTION HANDLING - Logging abstracted
        public async Task<string> ProcessCourseWithLoggingAsync(Course course)
        {
            try
            {
                // Business logic that can be tested
                await _delayService.DelayAsync(100);
                return $"Processed: {course.Name}";
            }
            catch (Exception ex)
            {
                // Logging through abstraction - can be mocked and verified
                await _loggerService.LogToFileAsync($"Error: {ex.Message}", "error_log.txt");
                throw;
            }
        }

        // 12. TESTABLE BUSINESS LOGIC - Separated concerns
        public async Task<bool> EnrollStudentInCourseAsync(int studentId, int courseId)
        {
            // Business logic cleanly separated from infrastructure
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                _loggerService.LogInformation($"Course {courseId} not found");
                return false;
            }

            // HTTP call through abstraction
            var studentResponse = await _httpService.GetResponseAsync($"https://api.example.com/students/{studentId}");
            
            if (!studentResponse.IsSuccessStatusCode)
            {
                await _loggerService.LogToFileAsync($"Student {studentId} not found", "enrollment_errors.txt");
                return false;
            }

            // Random number through abstraction
            var enrollmentNumber = _randomService.Next(100000, 999999);
            
            _consoleService.WriteLine($"Student {studentId} enrolled in course {courseId} with number {enrollmentNumber}");
            return true;
        }

        // BONUS: Method demonstrating testable validation with business rules
        public async Task<ValidationResult> ValidateCourseCreationAsync(string courseName, DateTime startDate)
        {
            var result = new ValidationResult();

            // Business rule validation
            if (string.IsNullOrWhiteSpace(courseName))
            {
                result.AddError("Course name cannot be empty");
                return result;
            }

            if (startDate <= _dateTimeProvider.Now)
            {
                result.AddError("Course start date must be in the future");
                return result;
            }

            // Check for duplicate names
            var existingCourse = await _courseRepository.GetCourseByNameAsync(courseName);
            if (existingCourse != null)
            {
                result.AddError($"Course with name '{courseName}' already exists");
                return result;
            }

            // Validate configuration file exists
            var configPath = _fileService.Combine(_environmentService.CurrentDirectory, "course_config.json");
            if (!_fileService.Exists(configPath))
            {
                result.AddError("Course configuration file not found");
                return result;
            }

            result.IsValid = true;
            return result;
        }
    }

    /// <summary>
    /// Simple validation result class for demonstration
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();

        public void AddError(string error)
        {
            Errors.Add(error);
            IsValid = false;
        }
    }
}