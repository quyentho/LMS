using System.Runtime.CompilerServices;
using TodoWeb.Application.Dtos.GuidModel;
using TodoWeb.Service.Services;
using TodoWeb.Service.Services.CacheService;
using TodoWeb.Service.Services.ExamSubmissions;
using TodoWeb.Service.Services.Users;
using TodoWeb.Service.Services.Courses;
using TodoWeb.Service.Services.Questions;
using TodoWeb.Service.Services.School;
using TodoWeb.Service.Services.ExamSubmissionDetails;
using TodoWeb.Service.Services.CourseStudents;
using TodoWeb.Service.Services.Students;
using TodoWeb.Service.Services.Exams;
using TodoWeb.Service.Services.Grade;
using TodoWeb.Service.Services.ExamQuestions;
using TodoWeb.Service.Services.Users.FacebookService;
using TodoWeb.Service.Services.Users.GoogleService;
using TodoWeb.MapperProfiles;
using TodoWeb.Middleware;
using TodoWeb.DataAccess.Repositories.StudentRepo;
using TodoWeb.DataAccess.Repositories.CourseRepo;

namespace TodoWeb.Extensions
{
    public static class AddDependencyInjection
    {

        public static void AddService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IStudentRepository, StudentRepository>();
            serviceCollection.AddScoped<ICourseRepository, CourseRepository>();


            serviceCollection.AddScoped<IToDoService, ToDoService>();
            serviceCollection.AddSingleton<ISingletonGenerator, SingltonGenerator>();
            serviceCollection.AddScoped<IStudentService, StudentService>();
            serviceCollection.AddScoped<ISchoolService, SchoolService>();
            serviceCollection.AddSingleton<GuidData>();
            serviceCollection.AddScoped<ICourseService, CourseService>();
            serviceCollection.AddScoped<IGradeService, GradeService>();
            serviceCollection.AddScoped<ICourseStudentService, CourseStudentService>();
            serviceCollection.AddScoped<IQuestionService, QuestionService>();
            serviceCollection.AddScoped<IExamService, ExamService>();
            serviceCollection.AddScoped<IExamQuestionService, ExamQuestionService>();
            serviceCollection.AddScoped<IExamSubmissionDetailsService, ExamSubmissionDetailsService>();
            serviceCollection.AddScoped<IExamSubbmissionService, ExamSubbmissionService>();
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddAutoMapper(typeof(CourseProfile));
            serviceCollection.AddAutoMapper(typeof(ToDoProfile));
            serviceCollection.AddAutoMapper(typeof(ExamProfile));
            serviceCollection.AddAutoMapper(typeof(ExamQuestionProfile));
            serviceCollection.AddAutoMapper(typeof(ExamSubmissionProfile));
            serviceCollection.AddAutoMapper(typeof(ExamSubmissionDetailsProfile));
            serviceCollection.AddAutoMapper(typeof(UserProfile));
            serviceCollection.AddSingleton<LogMiddleware>();
            serviceCollection.AddSingleton<RateLimitMiddleware>();
            serviceCollection.AddSingleton<RevokeCheckMiddleware>();
            //serviceCollection.AddSingleton<LogFilter>();
            serviceCollection.AddSingleton<ICacheService, CacheService>();
            serviceCollection.AddSingleton<IGoogleCredentialService, GoogleCredentialService>();
            serviceCollection.AddSingleton<IFacebookCredentialService, FacebookCredentialService>();

        }
    }
}
