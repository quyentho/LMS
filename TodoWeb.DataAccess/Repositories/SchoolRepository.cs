using Microsoft.EntityFrameworkCore;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.DataAccess.Repositories
{
    public class SchoolRepository : GenericRepository<School>, ISchoolRepository
    {
        public SchoolRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<School?> GetSchoolByNameAsync(string schoolName)
        {
            return await _dbSet
                .SingleOrDefaultAsync(school => school.Name == schoolName);
        }
    }
}
