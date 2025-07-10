using TodoWeb.Domains.Entities;

namespace TodoWeb.DataAccess.Repositories
{
    public interface ISchoolRepository : IGenericRepository<School>
    {
        Task<School?> GetSchoolByNameAsync(string schoolName);
    }
}
