using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.DataAccess.Repositories
{
    public class SchoolRepository: ISchoolRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public SchoolRepository(IApplicationDbContext applicationDbContext)
        {
            this._dbContext = applicationDbContext;
        }
        public async Task<List<School>> GetSchoolsAsync(int? schoolId, Expression<Func<School, object>>? include = null)
        {
            var schoolQuery = _dbContext.School.Where(s => s.Status != Constants.Enums.Status.Deleted);

            if (schoolId.HasValue)
            {
                schoolQuery = schoolQuery.Where(s => s.Id == schoolId.Value);
            }

            if (include != null)
            {
                schoolQuery = schoolQuery.Include(include);
            }

            return await schoolQuery.ToListAsync();
        }

        public async Task<School?> GetSchoolByNameAsync(string schoolName)
        {
            return await _dbContext.School
                .FirstOrDefaultAsync(s => s.Name == schoolName && s.Status != Constants.Enums.Status.Deleted);
        }
    }
}
