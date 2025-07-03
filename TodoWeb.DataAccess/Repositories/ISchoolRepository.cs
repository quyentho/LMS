using TodoWeb.Domains.Entities;

namespace TodoWeb.DataAccess.Repositories
{
    public interface ISchoolRepository
    {
        Task<School?> GetSchoolByNameAsync(string schoolName);
    }
}