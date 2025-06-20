using Microsoft.AspNetCore.Mvc;
using TodoWeb.Application.Dtos.StudentModel;
using TodoWeb.Service.Services.Students;

namespace TodoWeb.Controllers
{
    /// <summary>
    /// Example controller showing how to use the new IStudentServiceWithRepository
    /// This demonstrates the enhanced repository pattern functionality
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StudentRepositoryController : ControllerBase
    {
        private readonly IStudentServiceWithRepository _studentService;

        public StudentRepositoryController(IStudentServiceWithRepository studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Get students with optional filtering by ID
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<StudentViewModel>> GetStudents([FromQuery] int? studentId = null)
        {
            var students = _studentService.GetStudent(studentId);
            return Ok(students);
        }

        /// <summary>
        /// Get all students (cached version)
        /// </summary>
        [HttpGet("cached")]
        public ActionResult<IEnumerable<StudentViewModel>> GetStudentsCached()
        {
            var students = _studentService.GetStudents();
            return Ok(students);
        }

        /// <summary>
        /// Get a specific student by ID with async repository pattern
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentViewModel>> GetStudentByIdAsync(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound($"Student with ID {id} not found");

            return Ok(student);
        }

        /// <summary>
        /// Search students by term (firstName, lastName, or school name)
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<StudentViewModel>>> SearchStudentsAsync([FromQuery] string searchTerm)
        {
            var students = await _studentService.SearchStudentsAsync(searchTerm);
            return Ok(students);
        }

        /// <summary>
        /// Create a new student using async repository pattern
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> CreateStudentAsync([FromBody] StudentCreateViewModel studentCreateViewModel)
        {
            try
            {
                var studentId = await _studentService.PostStudentAsync(studentCreateViewModel);
                return CreatedAtAction(nameof(GetStudentByIdAsync), new { id = studentId }, studentId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing student
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStudentAsync(int id, [FromBody] StudentCreateViewModel studentUpdateViewModel)
        {
            try
            {
                var success = await _studentService.UpdateStudentAsync(id, studentUpdateViewModel);
                if (!success)
                    return NotFound($"Student with ID {id} not found");

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Soft delete a student (marks as deleted, doesn't remove from database)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStudentAsync(int id)
        {
            var success = await _studentService.SoftDeleteStudentAsync(id);
            if (!success)
                return NotFound($"Student with ID {id} not found");

            return NoContent();
        }

        /// <summary>
        /// Transfer a student to a new school using transaction support
        /// This demonstrates the advanced repository pattern features
        /// </summary>
        [HttpPost("{studentId}/transfer/{newSchoolId}")]
        public async Task<ActionResult> TransferStudentToNewSchoolAsync(int studentId, int newSchoolId)
        {
            try
            {
                var success = await _studentService.TransferStudentToNewSchoolAsync(studentId, newSchoolId);
                if (!success)
                    return NotFound($"Student with ID {studentId} or School with ID {newSchoolId} not found");

                return Ok(new { message = "Student transferred successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during the transfer", error = ex.Message });
            }
        }
    }
}
