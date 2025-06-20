using TodoWeb.Domains.Entities;

namespace TodoWeb.Infrastructures.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        // Generic repository access
        IGenericRepository<T> Repository<T>() where T : class, IBaseEntity;

        // Specific repository properties for commonly used entities
        IGenericRepository<ToDo> ToDoRepository { get; }
        IGenericRepository<Student> StudentRepository { get; }
        IGenericRepository<Course> CourseRepository { get; }
        IGenericRepository<CourseStudent> CourseStudentRepository { get; }
        IGenericRepository<School> SchoolRepository { get; }
        IGenericRepository<Grade> GradeRepository { get; }
        IGenericRepository<Question> QuestionRepository { get; }
        IGenericRepository<Exam> ExamRepository { get; }
        IGenericRepository<ExamQuestion> ExamQuestionRepository { get; }
        IGenericRepository<ExamSubmission> ExamSubmissionRepository { get; }
        IGenericRepository<ExamSubmissionDetail> ExamSubmissionDetailRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<RefreshToken> RefreshTokenRepository { get; }
        IGenericRepository<AuditLog> AuditLogRepository { get; }

        // Save operations
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        // Transaction support
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
