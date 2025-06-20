using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using TodoWeb.Application.Dtos.StudentModel;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures.Repositories;
using TodoWeb.Constants.Enums;

namespace TodoWeb.Service.Services.Students
{    // Updated StudentService using Repository Pattern
    public class StudentServiceWithRepository : IStudentServiceWithRepository
    {
        private const string STUDENT_KEY = "StudentKey";
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public StudentServiceWithRepository(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public IEnumerable<StudentViewModel> GetStudent(int? studentId)
        {
            // Using repository pattern with query builder
            var query = _unitOfWork.StudentRepository.Query(
                student => student.Status != Status.Deleted,
                student => student.School  // Include School
            );

            if (studentId.HasValue)
            {
                query = query.Where(x => x.Id == studentId);
            }

            var result = _mapper.ProjectTo<StudentViewModel>(query).ToList();
            return result;
        }

        public IEnumerable<StudentViewModel> GetStudents()
        {
            var data = _cache.GetOrCreate(STUDENT_KEY, entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(30);
                return GetAllStudent();
            });
            return data;
        }

        private IEnumerable<StudentViewModel> GetAllStudent()
        {
            // Using repository pattern
            var students = _unitOfWork.StudentRepository.Query(
                student => student.Status != Status.Deleted,
                student => student.School
            ).ToList();

            return _mapper.Map<IEnumerable<StudentViewModel>>(students);
        }

        public async Task<int> PostStudentAsync(StudentCreateViewModel studentCreateViewModel)
        {
            // Validate if school exists
            var school = await _unitOfWork.SchoolRepository.FirstOrDefaultAsync(
                s => s.Id == studentCreateViewModel.SchoolId && s.Status != Status.Deleted
            );

            if (school == null)
                throw new ArgumentException("School not found or has been deleted");

            var newStudent = _mapper.Map<Student>(studentCreateViewModel);
            newStudent.Status = Status.Unverified;

            await _unitOfWork.StudentRepository.AddAsync(newStudent);
            await _unitOfWork.SaveChangesAsync();

            return newStudent.Id;
        }

        public async Task<bool> UpdateStudentAsync(int id, StudentCreateViewModel studentUpdateViewModel)
        {
            var existingStudent = await _unitOfWork.StudentRepository.GetByIdAsync(id);
            if (existingStudent == null || existingStudent.Status == Status.Deleted)
                return false;

            // Validate if new school exists (if school is being changed)
            if (studentUpdateViewModel.SchoolId != existingStudent.School.Id)
            {
                var school = await _unitOfWork.SchoolRepository.FirstOrDefaultAsync(
                    s => s.Id == studentUpdateViewModel.SchoolId && s.Status != Status.Deleted
                );

                if (school == null)
                    throw new ArgumentException("School not found or has been deleted");
            }

            // Update student properties
            _mapper.Map(studentUpdateViewModel, existingStudent);

            _unitOfWork.StudentRepository.Update(existingStudent);
            await _unitOfWork.SaveChangesAsync();

            // Clear cache
            _cache.Remove(STUDENT_KEY);

            return true;
        }

        public async Task<bool> SoftDeleteStudentAsync(int id)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(id);
            if (student == null || student.Status == Status.Deleted)
                return false;

            // Using the generic repository's soft delete method
            _unitOfWork.StudentRepository.SoftDeleteById(id);
            await _unitOfWork.SaveChangesAsync();

            // Clear cache
            _cache.Remove(STUDENT_KEY);

            return true;
        }

        public async Task<StudentViewModel?> GetStudentByIdAsync(int id)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(id, s => s.School);
            if (student == null || student.Status == Status.Deleted)
                return null;

            return _mapper.Map<StudentViewModel>(student);
        }

        public async Task<IEnumerable<StudentViewModel>> SearchStudentsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllStudentsAsync();

            var students = await _unitOfWork.StudentRepository.FindAsync(
                s => s.Status != Status.Deleted &&
                     (s.FirstName.Contains(searchTerm) ||
                      s.LastName.Contains(searchTerm) ||
                      s.School.Name.Contains(searchTerm)),
                s => s.School
            );

            return _mapper.Map<IEnumerable<StudentViewModel>>(students);
        }

        private async Task<IEnumerable<StudentViewModel>> GetAllStudentsAsync()
        {
            var students = await _unitOfWork.StudentRepository.GetAllAsync(s => s.School);
            var activeStudents = students.Where(s => s.Status != Status.Deleted);
            return _mapper.Map<IEnumerable<StudentViewModel>>(activeStudents);
        }

        // Example of using transactions
        public async Task<bool> TransferStudentToNewSchoolAsync(int studentId, int newSchoolId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
                if (student == null || student.Status == Status.Deleted)
                    return false;

                var newSchool = await _unitOfWork.SchoolRepository.GetByIdAsync(newSchoolId);
                if (newSchool == null || newSchool.Status == Status.Deleted)
                    return false;

                // Update student's school
                student.School.Id = newSchoolId;
                _unitOfWork.StudentRepository.Update(student);

                // You could add additional business logic here
                // For example, updating grades, course enrollments, etc.

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Clear cache
                _cache.Remove(STUDENT_KEY);

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
