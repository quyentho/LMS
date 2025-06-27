using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoWeb.Application.Dtos.CourseStudentDetailModel;
using TodoWeb.Application.Dtos.StudentModel;
using TodoWeb.Application.Extensions;
using TodoWeb.DataAccess.Repositories.StudentRepo;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Service.Services.Students
{
    public class StudentService : IStudentService
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _cachedStudentRepository;

        public StudentService(IApplicationDbContext context, IMapper mapper, IStudentRepository cachedStudentRepository)
        {
            _context = context;
            _mapper = mapper;
            _cachedStudentRepository = cachedStudentRepository;
        }

        public async Task<IEnumerable<StudentViewModel>> GetStudentsAsync(int? studentId)
        {
            var students = await _cachedStudentRepository.GetStudentsAsync(studentId, s => s.School);

            var result = _mapper.Map<List<StudentViewModel>>(students).ToList();
            return result;
        }

        public async Task<IEnumerable<StudentViewModel>> GetStudentsAsync()
        {
            var students = await _cachedStudentRepository.GetAllStudentsWithCacheAsync(student => student.School);
            var result = _mapper.Map<List<StudentViewModel>>(students);
            return result;
        }

        public async Task<StudentPagingViewModel> GetPagedStudentsAsync(int? schoolId, string? sortBy, bool isDescending, int? pageSize, int? pageIndex)
        {
            if (pageSize <= 0 || pageIndex <= 0)
            {
                return new StudentPagingViewModel
                {
                    Students = Enumerable.Empty<StudentViewModel>().ToList(),
                    TotalPages = 0
                };
            }

            var students = await _cachedStudentRepository.GetPagedStudentsAsync(schoolId, sortBy, isDescending, pageSize, pageIndex);

            int totalPage;

            if (pageSize == null)
            {
                totalPage = 1;
            }
            else
            {
                var numberOfStudents = students.Count;
                totalPage = (int)Math.Ceiling((double)numberOfStudents / (int)pageSize);
            }
                
            var data = new StudentPagingViewModel
            {
                Students = _mapper.Map<List<StudentViewModel>>(students),
                TotalPages = totalPage
            };
            return data;
        }

        public IEnumerable<StudentViewModel> SearchStudents(string searchTerm)
        {
            var tokens = searchTerm
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var query = _context.Students
                .Where(student => student.Status != Constants.Enums.Status.Deleted)
                .Include(student => student.School)
                .AsQueryable();

            if (tokens.Length > 1)
            {
                query = query.ApplyRelatedSearch(searchTerm, student => student.Id.ToString(),
                    student => student.FirstName + " " + student.LastName,
                    student => student.Age.ToString(),
                    student => student.School.Name,
                    student => student.Balance.ToString());
            }else if (tokens.Length == 1)
            {
                query = query.ApplySearch(searchTerm, student => student.Id.ToString(),
                    student => student.FirstName + " " + student.LastName,
                    student => student.Age.ToString(),
                    student => student.School.Name,
                    student => student.Balance.ToString());
            }else
            {
                return Enumerable.Empty<StudentViewModel>();
            }

                var result = _mapper.ProjectTo<StudentViewModel>(query).ToList();
            return result;
        }

        public int Post(StudentViewModel student)
        {
            //kiểm tra xem student id có bị trùng hay không
            var dupID = _context.Students.Find(student.Id);
            if (dupID != null || student.Id < 1)
            {
                return -1;
            }
            var name = student.FullName.Split(' ');
            //lấy school nhờ vào school name
            var school = _context.School.FirstOrDefault(s => s.Name.Equals(student.SchoolName));//không dùng where bởi vì tìm ra một list

            if (school == null)
            {
                return -1;
            }

            var data = new Student
            {
                Id = student.Id,
                FirstName = name[0],
                LastName = string.Join(" ", name.Skip(1)),
                SId = school.Id,
                School = school,

            };
            _context.Students.Add(data);
            _context.SaveChanges();
            
            // Invalidate cache when data changes
            _cachedStudentRepository.InvalidateCache();
            
            return data.Id;
        }

        public int Put(StudentViewModel student)
        {
            //tìm student
            var data = _context.Students.Find(student.Id);
            if (data == null || data.Status == Constants.Enums.Status.Deleted)
            {
                return -1;
            }
            var name = student.FullName.Split(' ');
            //kiểm tra xem người dùng có đưa đúng tên school
            var school = _context.School.FirstOrDefault(s => s.Name.Equals(student.SchoolName));//không dùng where bởi vì tìm ra một list
            if (school == null)
            {
                return -1;
            }
            //data.FirstName = name[0];
            //data.LastName = string.Join(" ", name.Skip(1));
            //data.SId = school.Id;
            //data.School = school;
            //data.Balance = student.Balance;
            _mapper.Map(student, data);
            _context.SaveChanges();
            
            // Invalidate cache when data changes
            _cachedStudentRepository.InvalidateCache();
            
            return data.Id;
        }

        public int Delete(int studentID)
        {
            var data = _context.Students.Find(studentID);
            if (data == null)
            {
                return -1;
            }
            _context.Students.Remove(data);
            _context.SaveChanges();
            
            // Invalidate cache when data changes
            _cachedStudentRepository.InvalidateCache();
            
            return data.Id;
        }

        public StudentCourseDetailViewModel GetStudentDetails(int id)
        {
            var query = _context.Students
                .Where(student => student.Status != Constants.Enums.Status.Deleted)
                .Include(student => student.CourseStudent)
                .ThenInclude(cs => cs.Course);

            var student = query.FirstOrDefault(x => x.Id == id);//không dùng where bởi vì trả list
            //excute lúc này luôn, excute khi mình chấm cái gì đó mà kéo dữ liệu từ database thì nó sẽ excute 
            if (student == null)
            {
                return null;
            }
            //projectto dùng khi muốn map một câu query
            //còn map thì dùng khi muốn map một object
            return _mapper.Map<StudentCourseDetailViewModel>(student);
        }
    }
}
