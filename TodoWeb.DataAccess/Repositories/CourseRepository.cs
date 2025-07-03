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

        //public async Task<IEnumerable<Course>> GetAllAsync(int? courseId, Expression<Func<Course, object>>? include = null)
        //{
        //    var query = _dbContext.Course.AsQueryable();
        //    if (courseId.HasValue)
        //    {
        //        query = query.Where(c => c.Id == courseId);

        //        if (!query.Any())
        //        {
        //            return Enumerable.Empty<Course>();
        //        }
        //    }

        //    return await query.ToListAsync();
        //}

        //public async Task<Course?> GetByIdAsync(int courseId)
        //{
        //    return await _dbContext.Course.FindAsync(courseId);
        //}

        public async Task<Course?> GetCourseByNameAsync(string courseName)
        {
            return await _dbSet
                .SingleOrDefaultAsync(course => course.Name == courseName);
        }

        //public async Task<int> AddAsync(Course course)
        //{
        //    await _dbContext.Course.AddAsync(course);
        //    await _dbContext.SaveChangesAsync();

        //    return course.Id;
        //}

        //public async Task<int> UpdateAsync(Course course)
        //{
        //    var courseFromDb = await  GetByIdAsync(course.Id);

        //    if (courseFromDb == null)
        //    {
        //        throw new Exception("Course not found.");
        //    }

        //    _dbContext.Entry(courseFromDb).CurrentValues.SetValues(course);

        //    return await _dbContext.SaveChangesAsync();
        //}

        //public async Task<int> DeleteAsync(int courseId)
        //{
        //    var course = await GetByIdAsync(courseId);

        //    if (course == null)
        //    {
        //        throw new Exception("Course not found.");
        //    }

        //    _dbContext.Course.Remove(course);

        //    return await _dbContext.SaveChangesAsync();
        //}
    }
}
