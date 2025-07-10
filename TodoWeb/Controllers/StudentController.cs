using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TodoWeb.ActionFilters;
using TodoWeb.Application.Dtos.CourseStudentDetailModel;
using TodoWeb.Application.Dtos.StudentModel;
using TodoWeb.Domains.Entities;
using TodoWeb.Service.Services.Students;

namespace TodoWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(LogFilter), Arguments = [LogLevel.Warning])]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }
        [HttpGet("/search")]
        public IActionResult SearchStudents([FromQuery] string search_query)
        {
            var result = _studentService.SearchStudents(search_query);
            if (result.IsNullOrEmpty())
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpGet("/AllStudents")]
        public async Task<IActionResult> GetAllStudents()
        {
            var result = await _studentService.GetStudentsAsync(null);
            if (result.IsNullOrEmpty())
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpGet("{studentId}")]
        public IActionResult GetStudent(int studentId)
        {
            var result = _studentService.GetStudentsAsync(studentId);
            return Ok(result);
        }

        [HttpGet("/Students")]
        public IActionResult GetStudents(
            [FromQuery] int? schoolId,
            [FromQuery] string? sortBy,
            [FromQuery] bool desc,
            [FromQuery] int? pageSize,
            [FromQuery] int? pageIndex)

        {
            var result = _studentService.GetStudents(schoolId, sortBy, desc, pageSize, pageIndex);
            if (result.TotalPages == 0 || result.Students.IsNullOrEmpty())
            {
                return NotFound();
            }
            return Ok(result);

        }

        [HttpGet("/StudentDetails/{id}")]
        public StudentCourseDetailViewModel GetStudentDetails(int id)
        {
            return new StudentCourseDetailViewModel();
            //return _studentService.GetStudentDetails(id);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(StudentViewModel student)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return CreatedAtAction(nameof(GetStudent), await _studentService.PostAsync(student));
        }
        [HttpPut]
        public async Task<IActionResult> Put(StudentViewModel student)
        {
            await _studentService.PutAsync(student);
            return NoContent();
        }

        [HttpDelete]
        public async Task<int> DeleteAsync(int studentID)
        {
            return await _studentService.DeleteAsync(studentID);
        }
    }
}
