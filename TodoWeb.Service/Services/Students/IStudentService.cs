using TodoWeb.Application.Dtos.CourseStudentDetailModel;
using TodoWeb.Application.Dtos.StudentModel;
using TodoWeb.Domains.Entities;

namespace TodoWeb.Service.Services.Students
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentViewModel>> GetStudentsAsync(int? studentId);
        Task<IEnumerable<StudentViewModel>> GetStudentsAsync();
        public StudentCourseDetailViewModel GetStudentDetails(int id);
        public IEnumerable<StudentViewModel> SearchStudents(string searchTerm);
        public int Post(StudentViewModel student);
        public int Put(StudentViewModel student);
        public int Delete(int studentID);
        Task<StudentPagingViewModel> GetPagedStudentsAsync(int? schoolId, string? sortBy, bool isDescending, int? pageSize, int? pageIndex);
    }
}
