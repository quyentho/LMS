using System.Linq.Expressions;
using TodoWeb.Domains.Entities;

namespace TodoWeb.DataAccess.Repositories.StudentRepo
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetPagedStudentsAsync(int? schoolId, string? sortBy, bool isDescending, int? pageSize, int? pageIndex);
        Task<List<Student>> GetStudentsAsync(int? studentId, Expression<Func<Student, object>> include);
    }
}
