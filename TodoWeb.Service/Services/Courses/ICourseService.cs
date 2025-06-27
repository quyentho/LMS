using TodoWeb.Application.Dtos.CourseModel;
using TodoWeb.Application.Dtos.CourseStudentDetailModel;

namespace TodoWeb.Service.Services.Courses
{
    public interface ICourseService
    {
        public IEnumerable<CourseViewModel> GetCourses(int? courseId);

        public Task<int> Post(PostCourseViewModel course);
        public Task<int> PutAsync(CourseViewModel course);
        public Task<int> DeleteAsync(int courseId);
        Task<int> SoftDeleteAsync(int courseId);
    }
}
