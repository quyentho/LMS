using System.Linq.Expressions;
using TodoWeb.Domains.Entities;

namespace TodoWeb.DataAccess.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetStudentsAsync(int? studentId, Expression<Func<Student, object>>? include);
    }
}
