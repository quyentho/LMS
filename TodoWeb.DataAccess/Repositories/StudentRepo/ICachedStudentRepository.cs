using System.Linq.Expressions;
using TodoWeb.Domains.Entities;

namespace TodoWeb.DataAccess.Repositories.StudentRepo
{
    public interface ICachedStudentRepository : IStudentRepository
    {
        Task<List<Student>> GetAllStudentsWithCacheAsync(Expression<Func<Student, object>> include);
        void InvalidateCache();
    }
}