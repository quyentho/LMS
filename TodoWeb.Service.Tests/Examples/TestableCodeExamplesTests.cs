using AutoMapper;
using Moq;
using System.Net;
using TodoWeb.DataAccess.Repositories;
using TodoWeb.Domains.Entities;
using TodoWeb.Service.Services.Abstractions;
using TodoWeb.Service.Services.Examples;

namespace TodoWeb.Service.Tests.Examples
{
    /// <summary>
    /// Example unit tests demonstrating how the refactored testable code can be tested
    /// </summary>
    public class TestableCodeExamplesTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IConsoleService> _consoleServiceMock;
        private readonly Mock<IRandomService> _randomServiceMock;
        private readonly Mock<IGuidService> _guidServiceMock;
        private readonly Mock<IEnvironmentService> _environmentServiceMock;
        private readonly Mock<IHttpService> _httpServiceMock;
        private readonly Mock<ILoggerService> _loggerServiceMock;
        private readonly Mock<IDelayService> _delayServiceMock;
        private readonly TestableCodeExamples _testableService;

        public TestableCodeExamplesTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _fileServiceMock = new Mock<IFileService>();
            _consoleServiceMock = new Mock<IConsoleService>();
            _randomServiceMock = new Mock<IRandomService>();
            _guidServiceMock = new Mock<IGuidService>();
            _environmentServiceMock = new Mock<IEnvironmentService>();
            _httpServiceMock = new Mock<IHttpService>();
            _loggerServiceMock = new Mock<ILoggerService>();
            _delayServiceMock = new Mock<IDelayService>();

            _testableService = new TestableCodeExamples(
                _courseRepositoryMock.Object,
                _dateTimeProviderMock.Object,
                _fileServiceMock.Object,
                _consoleServiceMock.Object,
                _randomServiceMock.Object,
                _guidServiceMock.Object,
                _environmentServiceMock.Object,
                _httpServiceMock.Object,
                _loggerServiceMock.Object,
                _delayServiceMock.Object
            );
        }

        [Fact]
        public void CreateCourseWithCurrentTime_ShouldReturnFormattedString()
        {
            // Arrange
            var fixedDateTime = new DateTime(2024, 1, 15, 10, 30, 0);
            _dateTimeProviderMock.Setup(x => x.Now).Returns(fixedDateTime);

            // Act
            var result = _testableService.CreateCourseWithCurrentTime("Test Course");

            // Assert
            Assert.Equal("Course 'Test Course' created at 2024-01-15 10:30:00", result);
        }

        [Fact]
        public async Task SaveCourseToFileAsync_ShouldSaveToCorrectPath()
        {
            // Arrange
            var course = new Course { Id = 1, Name = "Test Course", StartDate = DateTime.Now };
            var currentDir = "C:\\TestDir";
            var expectedPath = "C:\\TestDir\\course_1.txt";
            var expectedContent = $"{course.Name} - {course.StartDate}";

            _environmentServiceMock.Setup(x => x.CurrentDirectory).Returns(currentDir);
            _fileServiceMock.Setup(x => x.Combine(currentDir, "course_1.txt")).Returns(expectedPath);

            // Act
            var result = await _testableService.SaveCourseToFileAsync(course);

            // Assert
            Assert.Equal(expectedPath, result);
            _fileServiceMock.Verify(x => x.WriteAllTextAsync(expectedPath, expectedContent), Times.Once);
        }

        [Fact]
        public void PrintCourseDetails_ShouldCallConsoleWriteLine()
        {
            // Arrange
            var course = new Course { Id = 1, Name = "Test Course", StartDate = new DateTime(2024, 1, 15) };

            // Act
            _testableService.PrintCourseDetails(course);

            // Assert
            _consoleServiceMock.Verify(x => x.WriteLine("Course ID: 1"), Times.Once);
            _consoleServiceMock.Verify(x => x.WriteLine("Course Name: Test Course"), Times.Once);
            _consoleServiceMock.Verify(x => x.WriteLine("Start Date: 1/15/2024 12:00:00 AM"), Times.Once);
        }

        [Fact]
        public void GenerateCourseCode_ShouldReturnPredictableValue()
        {
            // Arrange
            var expectedCode = "ABC123";
            _randomServiceMock.Setup(x => x.GenerateString(6)).Returns(expectedCode);

            // Act
            var result = _testableService.GenerateCourseCode();

            // Assert
            Assert.Equal(expectedCode, result);
        }

        [Fact]
        public async Task FetchCourseDataFromApiAsync_ShouldReturnApiResponse()
        {
            // Arrange
            var courseId = 1;
            var expectedResponse = "{'id': 1, 'name': 'Test Course'}";
            _httpServiceMock.Setup(x => x.GetAsync("https://api.example.com/courses/1"))
                           .ReturnsAsync(expectedResponse);

            // Act
            var result = await _testableService.FetchCourseDataFromApiAsync(courseId);

            // Assert
            Assert.Equal(expectedResponse, result);
        }

        [Fact]
        public async Task ProcessCourseWithDelayAsync_ShouldCallDelayService()
        {
            // Arrange
            var course = new Course { Name = "Test Course" };

            // Act
            await _testableService.ProcessCourseWithDelayAsync(course);

            // Assert
            _delayServiceMock.Verify(x => x.DelayAsync(5000), Times.Once);
            _consoleServiceMock.Verify(x => x.WriteLine("Processing course: Test Course"), Times.Once);
            _consoleServiceMock.Verify(x => x.WriteLine("Finished processing: Test Course"), Times.Once);
        }

        [Fact]
        public void GetCourseStoragePath_WithEnvironmentVariable_ShouldReturnEnvironmentPath()
        {
            // Arrange
            var expectedPath = "D:\\CustomPath";
            _environmentServiceMock.Setup(x => x.GetEnvironmentVariable("COURSE_STORAGE_PATH"))
                                  .Returns(expectedPath);

            // Act
            var result = _testableService.GetCourseStoragePath();

            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public void GetCourseStoragePath_WithoutEnvironmentVariable_ShouldReturnDefaultPath()
        {
            // Arrange
            _environmentServiceMock.Setup(x => x.GetEnvironmentVariable("COURSE_STORAGE_PATH"))
                                  .Returns(string.Empty);

            // Act
            var result = _testableService.GetCourseStoragePath();

            // Assert
            Assert.Equal("C:\\DefaultPath", result);
        }

        [Fact]
        public void CreateUniqueStudentId_ShouldReturnPredictableGuid()
        {
            // Arrange
            var expectedGuid = "12345678-1234-1234-1234-123456789012";
            _guidServiceMock.Setup(x => x.NewGuidString()).Returns(expectedGuid);

            // Act
            var result = _testableService.CreateUniqueStudentId();

            // Assert
            Assert.Equal(expectedGuid, result);
        }

        [Fact]
        public void ValidateCourseName_WithValidNameAndExistingFile_ShouldReturnTrue()
        {
            // Arrange
            var courseName = "Valid Course Name";
            var currentDir = "C:\\TestDir";
            var validationPath = "C:\\TestDir\\validation.txt";

            _environmentServiceMock.Setup(x => x.CurrentDirectory).Returns(currentDir);
            _fileServiceMock.Setup(x => x.Combine(currentDir, "validation.txt")).Returns(validationPath);
            _fileServiceMock.Setup(x => x.Exists(validationPath)).Returns(true);

            // Act
            var result = _testableService.ValidateCourseName(courseName);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("ab")]
        [InlineData("abc")]
        public void ValidateCourseName_WithInvalidName_ShouldReturnFalse(string courseName)
        {
            // Arrange
            _fileServiceMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

            // Act
            var result = _testableService.ValidateCourseName(courseName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ProcessCourseWithLoggingAsync_WhenExceptionThrown_ShouldLogError()
        {
            // Arrange
            var course = new Course { Name = "Test Course" };
            var exception = new InvalidOperationException("Test exception");
            
            _delayServiceMock.Setup(x => x.DelayAsync(100)).ThrowsAsync(exception);

            // Act & Assert
            var thrownException = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _testableService.ProcessCourseWithLoggingAsync(course));

            Assert.Equal("Test exception", thrownException.Message);
            _loggerServiceMock.Verify(x => x.LogToFileAsync("Error: Test exception", "error_log.txt"), Times.Once);
        }

        [Fact]
        public async Task EnrollStudentInCourseAsync_WithValidData_ShouldReturnTrue()
        {
            // Arrange
            var studentId = 1;
            var courseId = 1;
            var course = new Course { Id = courseId, Name = "Test Course" };
            var expectedEnrollmentNumber = 123456;
            
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);

            _courseRepositoryMock.Setup(x => x.GetByIdAsync(courseId)).ReturnsAsync(course);
            _httpServiceMock.Setup(x => x.GetResponseAsync($"https://api.example.com/students/{studentId}"))
                           .ReturnsAsync(httpResponse);
            _randomServiceMock.Setup(x => x.Next(100000, 999999)).Returns(expectedEnrollmentNumber);

            // Act
            var result = await _testableService.EnrollStudentInCourseAsync(studentId, courseId);

            // Assert
            Assert.True(result);
            _consoleServiceMock.Verify(x => x.WriteLine($"Student {studentId} enrolled in course {courseId} with number {expectedEnrollmentNumber}"), Times.Once);
        }

        [Fact]
        public async Task ValidateCourseCreationAsync_WithValidData_ShouldReturnValidResult()
        {
            // Arrange
            var courseName = "Valid Course";
            var startDate = DateTime.Now.AddDays(7);
            var currentDir = "C:\\TestDir";
            var configPath = "C:\\TestDir\\course_config.json";

            _dateTimeProviderMock.Setup(x => x.Now).Returns(DateTime.Now);
            _courseRepositoryMock.Setup(x => x.GetCourseByNameAsync(courseName))
                               .ReturnsAsync((Course?)null);
            _environmentServiceMock.Setup(x => x.CurrentDirectory).Returns(currentDir);
            _fileServiceMock.Setup(x => x.Combine(currentDir, "course_config.json")).Returns(configPath);
            _fileServiceMock.Setup(x => x.Exists(configPath)).Returns(true);

            // Act
            var result = await _testableService.ValidateCourseCreationAsync(courseName, startDate);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task ValidateCourseCreationAsync_WithDuplicateName_ShouldReturnInvalidResult()
        {
            // Arrange
            var courseName = "Duplicate Course";
            var startDate = DateTime.Now.AddDays(7);
            var existingCourse = new Course { Name = courseName };

            _dateTimeProviderMock.Setup(x => x.Now).Returns(DateTime.Now);
            _courseRepositoryMock.Setup(x => x.GetCourseByNameAsync(courseName))
                               .ReturnsAsync(existingCourse);

            // Act
            var result = await _testableService.ValidateCourseCreationAsync(courseName, startDate);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Course with name 'Duplicate Course' already exists", result.Errors);
        }
    }
}