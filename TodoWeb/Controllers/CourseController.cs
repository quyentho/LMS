using Microsoft.AspNetCore.Mvc;
using TodoWeb.ActionFilters;
using TodoWeb.Application.Dtos.CourseModel;
using TodoWeb.Application.Dtos.CourseStudentDetailModel;
using TodoWeb.Service.Services.Courses;
using TodoWeb.Service.Services.CourseStudents;

namespace TodoWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(LogFilter), Arguments = [LogLevel.Warning])]
    //typefilter tạo ra mỗi instance của LogFilter, mỗi lần gọi vào thì nó sẽ tạo ra một instance mới của LogFilter (giống với scope)
    //dùng khi muốn truyền tham số vào trong constructor của filter
    [AuditFilter]
    public class CourseController : Controller
    {
        //một instance của courseservice
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseController> _logger;
        private readonly ICourseStudentService _courseStudentService;

        public CourseController(ICourseService courseService, ILogger<CourseController> logger, ICourseStudentService courseStudentService)
        {
            _courseService = courseService;
            _logger = logger;
            this._courseStudentService = courseStudentService;
        }

        [TypeFilter(typeof(CacheFilter), Arguments = [10])]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseAsync(int id)
        {
            _logger.LogInformation($"Get Course with id: {id}");
            if(id == 10)
            {
                _logger.LogWarning($"Warning: {id}");
            }
            if(id <= 0)
            {
                _logger.LogError($"Error: Id can't less 0");
                throw new Exception("Id can't less 0");
            }

            var course = await _courseService.GetCoursesAsync(id);

            if (course == null || !course.Any())
            {
                _logger.LogWarning($"Course with id: {id} not found");
                return NotFound();
            }

            return Ok(course.Single());
        }


        [HttpGet]
        public async Task<IEnumerable<CourseViewModel>> GetAllCourseAsync()//int? courseId//có giá trị hoặc là null
        {
            return await _courseService.GetCoursesAsync(null);
        }
        
        [HttpGet("Detail/{id}")]
        public IEnumerable<CourseStudentDetailViewModel> GetCourseDetails(int id)
        {
            return _courseStudentService.GetCoursesDetail(id);
        }

        [HttpGet("Detail")]
        public IEnumerable<CourseStudentDetailViewModel> GetAllCourseDetails()
        {
            return _courseStudentService.GetCoursesDetail(null);
        }


        [HttpPost]
        public async Task<IActionResult> Post(PostCourseViewModel course)
        {
            try
            {
                return Ok(await _courseService.PostAsync(course));
            }
            catch (Exception ex)
            {
                //log
                throw new Exception(ex.Message);
            }
        }

        [HttpPut]
        public async Task<int> PutAsync(CourseViewModel course)
        {
            return await _courseService.PutAsync(course);
        }

        [HttpDelete]
        public async Task<int> DeleteAsync(int id)
        {
            return await _courseService.DeleteAsync(id);
        }
    }
}
