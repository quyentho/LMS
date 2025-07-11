using TodoWeb.Application.Dtos.CourseModel;
using TodoWeb.Application.Dtos.CourseStudentDetailModel;

namespace TodoWeb.Service.Services.Courses
{
    public interface ICourseService
    {
        public Task<IEnumerable<CourseViewModel>> GetCoursesAsync(int? courseId);
        public Task<int> PostAsync(PostCourseViewModel course);
        public Task<int> PutAsync(CourseViewModel course);
        public Task<int> DeleteAsync(int courseId);
    }
}
