using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.DataAccess.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext dbContext): base(dbContext)
        {
        }

        public async Task<Course?> GetCourseByNameAsync(string courseName)
        {
            return await _dbSet
                .SingleOrDefaultAsync(course => course.Name == courseName);
        }
    }
}
