using AutoMapper;
using TodoWeb.Application.Dtos.CourseModel;
using TodoWeb.DataAccess.Repositories;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Service.Services.Courses
{
    public class CourseService : ICourseService
    {
        private readonly IMapper _mapper;
        private readonly ICourseRepository _courseRepository;

        public CourseService(IApplicationDbContext context, IMapper mapper, ICourseRepository courseRepository)
        {
            _mapper = mapper;
            _courseRepository = courseRepository;
        }
        public async Task<IEnumerable<CourseViewModel>> GetCoursesAsync(int? courseId)
        {
            var courses = await _courseRepository.GetAllAsync(courseId);

            return _mapper.Map<List<CourseViewModel>>(courses);
        }

        public async Task<int> Post(PostCourseViewModel course)
        {
            var dupCourseName = await _courseRepository
                .GetCourseByNameAsync(course.CourseName);

            var data = _mapper.Map<Course>(course);

            return await _courseRepository.AddAsync(data);
        }

        public async Task<int> PutAsync(CourseViewModel courseViewModel)//src
        {
            var oldCourse = await _courseRepository.GetByIdAsync(courseViewModel.CourseId);

            if (oldCourse == null || oldCourse.Status == Constants.Enums.Status.Deleted)
            {
                throw new InvalidOperationException("Course not found or has been deleted.");
            }

            var dupCourseName = await _courseRepository.GetCourseByNameAsync(oldCourse.Name);

            _mapper.Map(courseViewModel, oldCourse);

            await _courseRepository.UpdateAsync(oldCourse);

            return oldCourse.Id;
        }

        public async Task<int> DeleteAsync(int courseId)
        {
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new InvalidOperationException($"Course with ID {courseId} does not exist.");
            }

            return await _courseRepository.DeleteAsync(course);
        }
    }
}
