using TodoWeb.Application.Dtos.StudentModel;

namespace TodoWeb.Service.Services.Students
{
    public interface IStudentServiceWithRepository
    {
        // Methods from original IStudentService
        IEnumerable<StudentViewModel> GetStudent(int? studentId);
        IEnumerable<StudentViewModel> GetStudents();

        // Enhanced async methods with repository pattern
        Task<int> PostStudentAsync(StudentCreateViewModel studentCreateViewModel);
        Task<bool> UpdateStudentAsync(int id, StudentCreateViewModel studentUpdateViewModel);
        Task<bool> SoftDeleteStudentAsync(int id);
        Task<StudentViewModel?> GetStudentByIdAsync(int id);
        Task<IEnumerable<StudentViewModel>> SearchStudentsAsync(string searchTerm);

        // Advanced repository pattern features
        Task<bool> TransferStudentToNewSchoolAsync(int studentId, int newSchoolId);
    }
}
