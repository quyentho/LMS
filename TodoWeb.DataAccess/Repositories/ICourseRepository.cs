using TodoWeb.Domains.Entities;

namespace TodoWeb.DataAccess.Repositories
{
    public interface ICourseRepository: IGenericRepository<Course>
    {
        Task<Course?> GetCourseByNameAsync(string courseName);
    }
}
