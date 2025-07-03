using TodoWeb.Application.Dtos.CourseStudentDetailModel;
using TodoWeb.Application.Dtos.StudentModel;
using TodoWeb.Domains.Entities;

namespace TodoWeb.Service.Services.Students
{
    public interface IStudentService
    {
        public Task<StudentViewModel> GetStudentAsync(int studentId);
        public Task<IEnumerable<StudentViewModel>> GetStudentsAsync();
        public StudentCourseDetailViewModel GetStudentDetails(int id);
        public StudentPagingViewModel GetStudents(int? schoolId, string? sortBy, bool isDescending, int? pageSize, int? pageIndex);
        public IEnumerable<StudentViewModel> SearchStudents(string searchTerm);
        public Task<int> PostAsync(StudentViewModel student);
        public int Put(StudentViewModel student);
        public int Delete(int studentID);
    }
}
