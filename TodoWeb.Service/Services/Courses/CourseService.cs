using AutoMapper;
using TodoWeb.Application.Dtos.CourseModel;
using TodoWeb.DataAccess.Repositories;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Service.Services.Courses
{
    public class CourseService : ICourseService
    {
        //thêm thuộc tính IApplicationDbContext vào class, và khỏi tạo giá trị thông qua constructer để 
        //từ đó class có phiên làm việc với cơ sở dữ liệu cho riêng mình

        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICourseRepository _courseRepository;

        public CourseService(IApplicationDbContext context, IMapper mapper, ICourseRepository courseRepository)
        {
            _context = context;
            _mapper = mapper;
            _courseRepository = courseRepository;
        }
        public IEnumerable<CourseViewModel> GetCourses(int? courseId)
        {
            var courses = _courseRepository.GetCourses(courseId);

            var result = _mapper.Map<List<CourseViewModel>>(courses);

            return result;
        }

        public async Task<int> Post(PostCourseViewModel course)
        {
            var dupCourseName = await _courseRepository.GetCourseByNameAsync(course.CourseName);

            if (dupCourseName != null) return -1;

            var data = _mapper.Map<Course>(course);

            var id = await _courseRepository.AddCourseAsync(data);

            return id;
        }

        public async Task<int> PutAsync(CourseViewModel course)//src
        {
            var oldCourse = await _courseRepository.GetCourseByIdAsync(course.CourseId);
            if (oldCourse == null || oldCourse.Status == Constants.Enums.Status.Deleted)
            {
                return -1;
            }

            _mapper.Map(course, oldCourse);

            var id = await _courseRepository.UpdateCourseAsync(oldCourse);

            return id;
        }

        public async Task<int> DeleteAsync(int courseId)
        {
            return await _courseRepository.DeleteCourseAsync(courseId);
        }

    }
}
