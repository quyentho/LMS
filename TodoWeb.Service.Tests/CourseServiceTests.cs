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
        private Mock<ICourseRepository> _courseRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private CourseService _courseService;

        public CourseServiceTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _mapperMock = new Mock<IMapper>();

            _courseService = new CourseService(_mapperMock.Object, _courseRepositoryMock.Object);
        }

        // method name_condition_expectedBehavior
        [Fact]
        public async Task PostAsync_WithNonExistentCourse_ReturnsCreatedIdAsync()
        {
            // Arrange
            var inputCourse = new PostCourseViewModel
            {
                CourseName = "New Course",
                StartDate = DateTime.Now,
            };

            var expectedCourse = new Course
            {
                Id = 9999,
                Name = inputCourse.CourseName,
                StartDate = inputCourse.StartDate,
            };

            _courseRepositoryMock.Setup(repo => repo.GetCourseByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((Course?)null);

            _mapperMock.Setup(mapper => mapper.Map<Course>(inputCourse))
                .Returns(expectedCourse);

            _courseRepositoryMock.Setup(repo => repo.AddAsync(expectedCourse))
                .ReturnsAsync(expectedCourse.Id);


            // Act
            var result = await _courseService.PostAsync(inputCourse);

            // Assert

            Assert.NotEqual(0, result);
            Assert.Equal(expectedCourse.Id, result);
        }

        [Fact]

        public async Task PostAsync_WithExistingCourse_ThrowsInvalidOperationException()
        {
            const string courseName = "Existing Course";

            // Arrange
            var inputCourse = new PostCourseViewModel
            {
                CourseName = courseName,
                StartDate = DateTime.Now,
            };

            var existingCourse = new Course
            {
                Id = 9999,
                Name = courseName,
                StartDate = inputCourse.StartDate,
            };

            _courseRepositoryMock.Setup(repo => repo.GetCourseByNameAsync(courseName))
                .ReturnsAsync(existingCourse);

            _mapperMock.Setup(mapper => mapper.Map<Course>(inputCourse))
                .Returns(existingCourse);

            _courseRepositoryMock.Setup(repo => repo.AddAsync(existingCourse))
                .ReturnsAsync(existingCourse.Id);

            // Act
            var result = async () => await _courseService.PostAsync(inputCourse);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(result);

            Assert.Equal($"Course with name '{courseName}' already exists.", exception.Message);

            _mapperMock.Verify(m => m.Map<Course>(inputCourse), Times.Never);

            _courseRepositoryMock.Verify(m => m.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task PutAsync_WithValidInput_ReturnsUpdatedCourseId()
        {
            var inputCourse = new CourseViewModel
            {
                CourseId = 9999,
                CourseName = "Existing Course",
                StartDate = DateTime.Now,
            };

            var expectedCourse = new Course
            {
                Id = inputCourse.CourseId,
                Name = "Updated Name",
                StartDate = inputCourse.StartDate,
                Status = Constants.Enums.Status.Finished
            };

            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(inputCourse.CourseId))
                .ReturnsAsync(expectedCourse);

            _mapperMock.Setup(mapper => mapper.Map(inputCourse, expectedCourse))
                .Verifiable();

            _courseRepositoryMock.Setup(repo => repo.UpdateAsync(expectedCourse))
                .Verifiable();

            // Act
            var id = await _courseService.PutAsync(inputCourse);

            // Assert
            Assert.Equal(expectedCourse.Id, id);

            _mapperMock.Verify(mapper => mapper.Map(inputCourse, expectedCourse), Times.Once);
            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(expectedCourse), Times.Once);
        }

        [Theory]
        [MemberData(nameof(NonExistentCourses))]
        public async Task PutAsync_WithNonExistentOrDeletedCourse_ThrowsInvalidOperationException(Course? courseFromDb)
        {
            var inputCourse = new CourseViewModel
            {
                CourseId = 9999,
                CourseName = "NonExistent Course",
                StartDate = DateTime.Now,
            };

            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(inputCourse.CourseId))
                .ReturnsAsync(courseFromDb);

            _mapperMock.Setup(mapper => mapper.Map(inputCourse, courseFromDb));

            _courseRepositoryMock.Setup(repo => repo.UpdateAsync(courseFromDb!));

            //Act, Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _courseService.PutAsync(inputCourse));

            Assert.Equal("Course not found or has been deleted.", exception.Message);

            _mapperMock.Verify(mapper => mapper.Map(inputCourse, courseFromDb), Times.Never);

            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(courseFromDb!), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingCourse_ReturnsDeletedCourseId()
        {
            // Arrange
            var existingCourseId = 9999;
            var existingCourse = new Course
            {
                Id = existingCourseId,
                Name = "Existing Course",
                StartDate = DateTime.Now,
            };

            _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(existingCourseId))
                .ReturnsAsync(existingCourse);

            _courseRepositoryMock.Setup(repo => repo.DeleteAsync(existingCourse))
                .ReturnsAsync(existingCourseId);

            // Act
            var result = await _courseService.DeleteAsync(existingCourseId);

            // Assert
            Assert.Equal(existingCourseId, result);
        }

        public static IEnumerable<object[]> NonExistentCourses = new List<object[]>
        {
            new object[] { null },
            new object[] { new Course { Id = 9999, Name = "NonExistent Course", Status = Constants.Enums.Status.Deleted} }
        };
    }
}