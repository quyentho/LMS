using System.Net.Http;
using System.IO;
using TodoWeb.Application.Dtos.CourseModel;
using TodoWeb.Domains.Entities;
using TodoWeb.DataAccess.Repositories;

namespace TodoWeb.Service.Services.Examples
{
    /// <summary>
    /// Examples of UNTESTABLE code patterns that are difficult to mock and unit test.
    /// These are anti-patterns that should be avoided in production code.
    /// </summary>
    public class UntestableCodeExamples
    {
        private readonly ICourseRepository _courseRepository;

        public UntestableCodeExamples(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        // 1. STATIC DEPENDENCIES - Cannot be mocked
        public string CreateCourseWithCurrentTime(string courseName)
        {
            // DateTime.Now is static and cannot be mocked
            var currentTime = DateTime.Now;
            var course = new Course
            {
                Name = courseName,
                StartDate = currentTime,
                CreateAt = currentTime
            };

            return $"Course '{courseName}' created at {currentTime:yyyy-MM-dd HH:mm:ss}";
        }

        // 2. FILE SYSTEM DEPENDENCIES - Hard to test without actual files
        public async Task<string> SaveCourseToFileAsync(Course course)
        {
            // Direct file system access - requires actual files to test
            var fileName = $"course_{course.Id}.txt";
            var filePath = Path.Combine(Environment.CurrentDirectory, fileName);
            
            await File.WriteAllTextAsync(filePath, $"{course.Name} - {course.StartDate}");
            
            return filePath;
        }

        // 3. CONSOLE OUTPUT - Cannot be easily verified in tests
        public void PrintCourseDetails(Course course)
        {
            // Console.WriteLine cannot be easily captured in unit tests
            Console.WriteLine($"Course ID: {course.Id}");
            Console.WriteLine($"Course Name: {course.Name}");
            Console.WriteLine($"Start Date: {course.StartDate}");
        }

        // 4. RANDOM NUMBER GENERATION - Non-deterministic results
        public string GenerateCourseCode()
        {
            // Random is not deterministic, making tests unreliable
            var random = new Random();
            var code = "";
            for (int i = 0; i < 6; i++)
            {
                code += (char)('A' + random.Next(26));
            }
            return code;
        }

        // 5. HTTP CLIENT INSTANTIATION - Creates actual HTTP calls
        public async Task<string> FetchCourseDataFromApiAsync(int courseId)
        {
            // HttpClient instantiated directly - will make actual HTTP calls
            using var client = new HttpClient();
            var response = await client.GetAsync($"https://api.example.com/courses/{courseId}");
            return await response.Content.ReadAsStringAsync();
        }

        // 6. THREAD.SLEEP - Makes tests slow and unreliable
        public async Task ProcessCourseWithDelayAsync(Course course)
        {
            Console.WriteLine($"Processing course: {course.Name}");
            
            // Thread.Sleep makes tests slow and unreliable
            Thread.Sleep(5000); // 5 seconds delay
            
            Console.WriteLine($"Finished processing: {course.Name}");
        }

        // 7. ENVIRONMENT VARIABLES - Depends on system configuration
        public string GetCourseStoragePath()
        {
            // Environment variables make tests dependent on system configuration
            var basePath = Environment.GetEnvironmentVariable("COURSE_STORAGE_PATH");
            return basePath ?? "C:\\DefaultPath";
        }

        // 8. GUID GENERATION - Non-deterministic
        public string CreateUniqueStudentId()
        {
            // Guid.NewGuid() is non-deterministic
            return Guid.NewGuid().ToString();
        }

        // 9. COMPLEX STATIC CALLS - Hard to isolate
        public bool ValidateCourseName(string courseName)
        {
            // Complex static method calls are hard to mock
            return !string.IsNullOrEmpty(courseName) && 
                   courseName.Length > 3 && 
                   File.Exists(Path.Combine(Environment.CurrentDirectory, "validation.txt"));
        }

        // 10. SEALED CLASSES - Cannot be mocked (like DateTime, File, etc.)
        public async Task<Course> CreateCourseWithValidationAsync(string courseName)
        {
            // Multiple untestable dependencies combined
            var course = new Course
            {
                Name = courseName,
                StartDate = DateTime.Now.AddDays(7), // Static DateTime
                CreateAt = DateTime.UtcNow,
                Id = Math.Abs(Guid.NewGuid().GetHashCode()) // Non-deterministic ID
            };

            // File system dependency
            var logPath = Path.Combine(Environment.CurrentDirectory, "course_log.txt");
            await File.AppendAllTextAsync(logPath, $"Course created: {course.Name} at {DateTime.Now}\n");

            // Console output
            Console.WriteLine($"New course created: {course.Name}");

            return course;
        }

        // 11. EXCEPTION HANDLING WITH STATIC DEPENDENCIES
        public async Task<string> ProcessCourseWithLoggingAsync(Course course)
        {
            try
            {
                // Some business logic
                await Task.Delay(100);
                return $"Processed: {course.Name}";
            }
            catch (Exception ex)
            {
                // Logging to file system - untestable
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error: {ex.Message}";
                await File.AppendAllTextAsync("error_log.txt", logMessage + "\n");
                throw;
            }
        }

        // 12. MIXED BUSINESS LOGIC WITH INFRASTRUCTURE
        public async Task<bool> EnrollStudentInCourseAsync(int studentId, int courseId)
        {
            // Business logic mixed with infrastructure concerns
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                // Logging directly to console
                Console.WriteLine($"Course {courseId} not found");
                return false;
            }

            // Creating HTTP client directly
            using var httpClient = new HttpClient();
            var studentResponse = await httpClient.GetAsync($"https://api.example.com/students/{studentId}");
            
            if (!studentResponse.IsSuccessStatusCode)
            {
                // Writing to file system
                await File.AppendAllTextAsync("enrollment_errors.txt", 
                    $"Student {studentId} not found at {DateTime.Now}\n");
                return false;
            }

            // Random enrollment number
            var enrollmentNumber = new Random().Next(100000, 999999);
            
            Console.WriteLine($"Student {studentId} enrolled in course {courseId} with number {enrollmentNumber}");
            return true;
        }
    }
}