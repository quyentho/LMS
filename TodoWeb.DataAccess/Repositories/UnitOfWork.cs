using Microsoft.EntityFrameworkCore.Storage;
using TodoWeb.Domains.Entities;

namespace TodoWeb.Infrastructures.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        // Specific repository backing fields
        private IGenericRepository<ToDo>? _toDoRepository;
        private IGenericRepository<Student>? _studentRepository;
        private IGenericRepository<Course>? _courseRepository;
        private IGenericRepository<CourseStudent>? _courseStudentRepository;
        private IGenericRepository<School>? _schoolRepository;
        private IGenericRepository<Grade>? _gradeRepository;
        private IGenericRepository<Question>? _questionRepository;
        private IGenericRepository<Exam>? _examRepository;
        private IGenericRepository<ExamQuestion>? _examQuestionRepository;
        private IGenericRepository<ExamSubmission>? _examSubmissionRepository;
        private IGenericRepository<ExamSubmissionDetail>? _examSubmissionDetailRepository;
        private IGenericRepository<User>? _userRepository;
        private IGenericRepository<RefreshToken>? _refreshTokenRepository;
        private IGenericRepository<AuditLog>? _auditLogRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        #region Generic Repository Access

        public IGenericRepository<T> Repository<T>() where T : class, IBaseEntity
        {
            var type = typeof(T);

            if (_repositories.ContainsKey(type))
            {
                return (IGenericRepository<T>)_repositories[type];
            }

            var repository = new GenericRepository<T>(_context);
            _repositories.Add(type, repository);
            return repository;
        }

        #endregion

        #region Specific Repository Properties

        public IGenericRepository<ToDo> ToDoRepository
        {
            get { return _toDoRepository ??= Repository<ToDo>(); }
        }

        public IGenericRepository<Student> StudentRepository
        {
            get { return _studentRepository ??= Repository<Student>(); }
        }

        public IGenericRepository<Course> CourseRepository
        {
            get { return _courseRepository ??= Repository<Course>(); }
        }

        public IGenericRepository<CourseStudent> CourseStudentRepository
        {
            get { return _courseStudentRepository ??= Repository<CourseStudent>(); }
        }

        public IGenericRepository<School> SchoolRepository
        {
            get { return _schoolRepository ??= Repository<School>(); }
        }

        public IGenericRepository<Grade> GradeRepository
        {
            get { return _gradeRepository ??= Repository<Grade>(); }
        }

        public IGenericRepository<Question> QuestionRepository
        {
            get { return _questionRepository ??= Repository<Question>(); }
        }

        public IGenericRepository<Exam> ExamRepository
        {
            get { return _examRepository ??= Repository<Exam>(); }
        }

        public IGenericRepository<ExamQuestion> ExamQuestionRepository
        {
            get { return _examQuestionRepository ??= Repository<ExamQuestion>(); }
        }

        public IGenericRepository<ExamSubmission> ExamSubmissionRepository
        {
            get { return _examSubmissionRepository ??= Repository<ExamSubmission>(); }
        }

        public IGenericRepository<ExamSubmissionDetail> ExamSubmissionDetailRepository
        {
            get { return _examSubmissionDetailRepository ??= Repository<ExamSubmissionDetail>(); }
        }

        public IGenericRepository<User> UserRepository
        {
            get { return _userRepository ??= Repository<User>(); }
        }

        public IGenericRepository<RefreshToken> RefreshTokenRepository
        {
            get { return _refreshTokenRepository ??= Repository<RefreshToken>(); }
        }

        public IGenericRepository<AuditLog> AuditLogRepository
        {
            get { return _auditLogRepository ??= Repository<AuditLog>(); }
        }

        #endregion

        #region Save Operations

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Transaction Support

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        #endregion

        #region Dispose Pattern

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }

        #endregion
    }
}
