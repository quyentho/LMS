using TodoWeb.Domains.Entities;

namespace TodoWeb.DataAccess.Repositories
{
    public interface ICourseRepository
    {
        Task<int> AddAsync(Course course);
        Task<int> DeleteAsync(int courseId);
        Task<Course?> GetCourseByIdAsync(int courseId);
        Task<Course?> GetCourseByNameAsync(string courseName);
        Task<IEnumerable<Course>> GetCoursesAsync(int? courseId);
        Task<int> UpdateAsync(Course course);
    }
}
