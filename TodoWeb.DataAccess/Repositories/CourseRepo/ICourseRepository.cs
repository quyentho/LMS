using TodoWeb.Domains.Entities;

namespace TodoWeb.DataAccess.Repositories.CourseRepo
{
    public interface ICourseRepository
    {
        Task<int> AddCourseAsync(Course course);
        Task<Course?> GetCourseByNameAsync(string courseName);
        Task<IEnumerable<Course>> GetCourses(int? courseId);
        
        Task<Course?> GetCourseByIdAsync(int courseId);
        Task<int> UpdateCourseAsync(Course course);
        Task<int> DeleteCourseAsync(int courseId);
    }
}