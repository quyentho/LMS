using AutoMapper;
using Moq;
using TodoWeb.Application.Dtos.CourseModel;
using TodoWeb.DataAccess.Repositories;
using TodoWeb.Domains.Entities;
using TodoWeb.Service.Services.Courses;

namespace TodoWeb.Service.Tests
{
    public class CourseServiceTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<ICourseRepository> _courseRepositoryMock;
        private CourseService _courseService;

        public CourseServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _courseService = new CourseService(_mapperMock.Object, _courseRepositoryMock.Object);
        }

        [Fact]
        public async Task PostAsync_WithANewCourseName_ReturnsCreatedIdAsync()
        {
            // Arrange

            _courseRepositoryMock.Setup(repo => repo.GetCourseByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Course?)null);

            var expectedCourse = new Course
            {
                Id = 1,
                Name = "New Course",
                StartDate = DateTime.UtcNow
            };

            var newCourse = new PostCourseViewModel
            {
                CourseName = "New Course",
                StartDate = DateTime.UtcNow
            };

            _mapperMock.Setup(x => x.Map<Course>(newCourse))
                .Returns(expectedCourse);

            _courseRepositoryMock.Setup(repo => repo.AddAsync(expectedCourse))
                .ReturnsAsync(expectedCourse.Id);

            // Act
            var id = await _courseService.PostAsync(newCourse);

            // Assert
            Assert.NotEqual(0, id);
            Assert.Equal(expectedCourse.Id, id);
        }

        [Fact]
        public async Task PostAsync_WithAnExistingCourseName_ThrowsInvalidOperationException()
        {
            // Arrange
            var existingCourseName = "Existing Course";

            var inputCourse = new PostCourseViewModel
            {
                CourseName = existingCourseName,
                StartDate = DateTime.UtcNow
            };

            var existingCourse = new Course
            {
                Id = 1,
                Name = existingCourseName,
                StartDate = DateTime.UtcNow
            };

            _courseRepositoryMock.Setup(repo => repo.GetCourseByNameAsync(existingCourseName))
                .ReturnsAsync(existingCourse);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _courseService.PostAsync(inputCourse)
            );

            Assert.Equal($"Course with name '{existingCourseName}' already exists.", exception.Message);
        }

        [Fact]
        public async Task PutAsync_WithValidCourseInput_ReturnsUpdatedCourseId()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var inputCourse = new CourseViewModel
            {
                CourseId = 1,
                CourseName = "Updated Course",
                StartDate = startDate
            };

            var oldCourse = new Course
            {
                Id = 1,
                Name = "Old Course",
                StartDate = startDate,
                Status = Constants.Enums.Status.NotStarted
            };

            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(inputCourse.CourseId))
                .ReturnsAsync(oldCourse);

            _mapperMock.Setup(x => x.Map(inputCourse, oldCourse));

            _courseRepositoryMock.Setup(repo => repo.UpdateAsync(oldCourse));

            // Act

            var updatedId = await _courseService.PutAsync(inputCourse);

            // Assert

            Assert.Equal(oldCourse.Id, updatedId);

            _mapperMock.Verify(x => x.Map(inputCourse, oldCourse), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(oldCourse), Times.Once);
        }

        [Theory]
        [MemberData(nameof(NonExistentCourses))]
        public async Task PutAsync_WithNonExistentOrDeletedCourseId_ThrowsInvalidOperationException(Course? course)
        {
            var inputCourse = new CourseViewModel
            {
                CourseId = 99,
                CourseName = "Non-existent Course",
                StartDate = DateTime.UtcNow
            };

            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(inputCourse.CourseId))
                .ReturnsAsync(course);

            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _courseService.PutAsync(inputCourse));

            Assert.Equal("Course not found or has been deleted.", exception.Message);
        }

        public static IEnumerable<object[]> NonExistentCourses => new List<object[]>
        {
            new object[] { null },
            new object[] { new Course { Id = 99, Status = Constants.Enums.Status.Deleted } }
        };
    }
}