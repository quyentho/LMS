using Microsoft.EntityFrameworkCore;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.DataAccess.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public CourseRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync(int? courseId)
        {
            var query = _dbContext.Course.AsQueryable();
            if (courseId.HasValue)
            {
                query = query.Where(c => c.Id == courseId);

                if (!query.Any())
                {
                    return Enumerable.Empty<Course>();
                }
            }

            return await query.ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            return await _dbContext.Course.FindAsync(courseId);
        }

        public async Task<Course?> GetCourseByNameAsync(string courseName)
        {
            return await _dbContext.Course
                .SingleOrDefaultAsync(course => course.Name == courseName);
        }

        public async Task<int> AddAsync(Course course)
        {
            await _dbContext.Course.AddAsync(course);
            await _dbContext.SaveChangesAsync();

            return course.Id;
        }

        public async Task<int> UpdateAsync(Course course)
        {
            var courseFromDb = await  GetCourseByIdAsync(course.Id);

            if (courseFromDb == null)
            {
                throw new Exception("Course not found.");
            }

            _dbContext.Entry(courseFromDb).CurrentValues.SetValues(course);

            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int courseId)
        {
            var course = await GetCourseByIdAsync(courseId);

            if (course == null)
            {
                throw new Exception("Course not found.");
            }

            _dbContext.Course.Remove(course);

            return await _dbContext.SaveChangesAsync();
        }
    }
}
