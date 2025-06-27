using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoWeb.Application.Dtos.CourseModel;
using TodoWeb.Application.Dtos.CourseStudentDetailModel;
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
        public CourseService(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IEnumerable<CourseViewModel> GetCourses(int? courseId)
        {

            var query = _context.Course.AsQueryable();
            if (courseId.HasValue)
            {
                query = query.Where(course => course.Id == courseId);
                if (query.Count() == 0) return null;
            }
            var result = _mapper.ProjectTo<CourseViewModel>(query).ToList();

            return result;
        }

        public async Task<int> Post(PostCourseViewModel course)
        {
            var dupCourseName = await _context.Course.FirstOrDefaultAsync(c => c.Name == course.CourseName);

            if (dupCourseName != null) return -1;


            var data = _mapper.Map<Course>(course);
            _context.Course.Add(data);

            await _context.SaveChangesAsync();
            return data.Id;
        }

        public int Put(CourseViewModel course)//src
        {
            var oldCourse = _context.Course.Find(course.CourseId);
            if (oldCourse == null || oldCourse.Status == Constants.Enums.Status.Deleted)
            {
                return -1;
            }

            var dupCourseName = _context.Course.FirstOrDefault(c => c.Name == course.CourseName);
            if (dupCourseName != null) return -1;

            _mapper.Map(course, oldCourse);

            _context.SaveChanges();
            return oldCourse.Id;
        }

        public int Delete(int courseId)
        {
            var oldCourse = _context.Course.Find(courseId);
            if (oldCourse == null)
            {
                return -1;
            }
            _context.Course.Remove(oldCourse);
            _context.SaveChanges();
            return oldCourse.Id;
        }

        public IEnumerable<CourseStudentDetailViewModel> GetCoursesDetail(int? courseId)
        {
            var query = _context.Course.AsQueryable();

            if (courseId.HasValue)
            {
                query = query.Where(course => course.Id == courseId);
                if (query.Count() == 0) return null;
            }

            query = query.Where(course => course.Status != Constants.Enums.Status.Deleted)
                .Include(course => course.CourseStudent)
                .ThenInclude(courseStudent => courseStudent.Student);

            return _mapper.ProjectTo<CourseStudentDetailViewModel>(query);
        }
    }
}
