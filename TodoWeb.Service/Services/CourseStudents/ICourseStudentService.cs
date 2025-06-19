using TodoWeb.Application.Dtos.CourseStudentModel;

namespace TodoWeb.Service.Services.CourseStudents
{
    public interface ICourseStudentService
    {
        public int PostCourseStudent(PostCourseStudentViewModel courseStudentViewModel);
    }
}
