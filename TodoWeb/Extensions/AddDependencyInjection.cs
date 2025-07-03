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
using TodoWeb.DataAccess.Repositories;
using TodoWeb.Domains.Entities;
using Microsoft.Extensions.Caching.Memory;
using TodoWeb.Infrastructures;

namespace TodoWeb.Extensions
{
    public static class AddDependencyInjection
    {

        public static void AddService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISchoolRepository, SchoolRepository>();
            serviceCollection.AddScoped<IGenericRepository<Student>, CachedRepository<Student>>(serviceProvider =>
            {
                return new CachedRepository<Student>(
                    serviceProvider.GetRequiredService<IMemoryCache>(),
                    ActivatorUtilities.CreateInstance<StudentRepository>(serviceProvider)
                );
            });

            serviceCollection.AddScoped<IGenericRepository<Course>, CachedRepository<Course>>(serviceProvider =>
            {
                return new CachedRepository<Course>(
                    serviceProvider.GetRequiredService<IMemoryCache>(),
                    ActivatorUtilities.CreateInstance<CourseRepository>(serviceProvider)
                );
            });

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
