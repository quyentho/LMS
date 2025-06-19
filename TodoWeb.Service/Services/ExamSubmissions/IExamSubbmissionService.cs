using TodoWeb.Application.Dtos.ExamSubmissionsModel;

namespace TodoWeb.Service.Services.ExamSubmissions
{
    public interface IExamSubbmissionService
    {
        public int CreateStudentExamSubmission(StudentExamSubmissionCreateModel newStudentExamSubmission);
    }
}
