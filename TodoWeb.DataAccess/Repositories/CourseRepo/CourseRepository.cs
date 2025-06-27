using Microsoft.EntityFrameworkCore;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.DataAccess.Repositories.CourseRepo
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CourseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Course>> GetCourses(int? courseId)
        {
            var query = _dbContext.Course.AsQueryable();
            if (courseId.HasValue)
            {
                query = query.Where(course => course.Id == courseId);

                if (query.Count() == 0)
                {
                    return await Task.FromResult(Enumerable.Empty<Course>());
                }
            }
            return await query.ToListAsync();
        }

        public async Task<Course?> GetCourseByNameAsync(string courseName)
        {
            return await _dbContext.Course.FirstOrDefaultAsync(course => course.Name == courseName);
        }

        public Course? GetCourseById(int courseId)
        {
            return _dbContext.Course.FirstOrDefault(course => course.Id == courseId);
        }

        public async Task<int> AddCourseAsync(Course course)
        {
            await _dbContext.Course.AddAsync(course);
            await _dbContext.SaveChangesAsync();
            return course.Id;
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId)
        {
            return await _dbContext.Course.FindAsync(courseId);
        }

        public async Task<int> UpdateCourseAsync(Course course)
        {
            var existingCourse = await _dbContext.Course.FindAsync(course.Id);
            if (existingCourse == null)
            {
                return -1; // Course not found
            }
            _dbContext.Entry(existingCourse).CurrentValues.SetValues(course);
            await _dbContext.SaveChangesAsync();
            return existingCourse.Id;
        }

        public async Task<int> DeleteCourseAsync(int courseId)
        {
            var course = await _dbContext.Course.FindAsync(courseId);
            if (course == null)
            {
                return -1; // Course not found
            }
            _dbContext.Course.Remove(course);
            await _dbContext.SaveChangesAsync();
            return course.Id;
        }
    }
}
