using System.Linq.Expressions;
using TodoWeb.Domains.Entities;

namespace TodoWeb.DataAccess.Repositories.StudentRepo
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetStudentAsync(int? studentId, Expression<Func<Student, object>> include);
    }
}
