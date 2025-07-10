using TodoWeb.Application.Dtos.CourseStudentDetailModel;
using TodoWeb.Application.Dtos.StudentModel;
using TodoWeb.Domains.Entities;

namespace TodoWeb.Service.Services.Students
{
    public interface IStudentService
    {
        public Task<IEnumerable<StudentViewModel>> GetStudentsAsync(int? studentId);
        public StudentPagingViewModel GetStudents(int? schoolId, string? sortBy, bool isDescending, int? pageSize, int? pageIndex);
        public IEnumerable<StudentViewModel> SearchStudents(string searchTerm);
        public Task<int> PostAsync(StudentViewModel student);
        public Task<int> PutAsync(StudentViewModel student);
        public Task<int> DeleteAsync(int studentID);
    }
}
