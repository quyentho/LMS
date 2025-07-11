using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using TodoWeb.Application.Dtos.CourseStudentDetailModel;
using TodoWeb.Application.Dtos.StudentModel;
using TodoWeb.Application.Extensions;
using TodoWeb.DataAccess.Repositories;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Service.Services.Students
{


    public class StudentService : IStudentService
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly ISchoolRepository _schoolRepository;

        public StudentService(IApplicationDbContext context, IMapper mapper, IMemoryCache cache, IGenericRepository<Student> studentRepository, ISchoolRepository schoolRepository)
        {
            _context = context;
            _mapper = mapper;
            _cache = cache;
            this._studentRepository = studentRepository;
            this._schoolRepository = schoolRepository;
        }
        public async Task<IEnumerable<StudentViewModel>> GetStudentsAsync(int? studentId)
        {
            var students = await _studentRepository.GetAllAsync(studentId, s => s.School);

            var result = _mapper.Map<List<StudentViewModel>>(students);
            return result;
        }
     
        public StudentPagingViewModel GetStudents(int? schoolId, string? sortBy, bool isDescending, int? pageSize, int? pageIndex)
        {
            if (pageSize <= 0 || pageIndex <= 0)
            {
                return new StudentPagingViewModel
                {
                    Students = Enumerable.Empty<StudentViewModel>().ToList(),
                    TotalPages = 0
                };
            }
            var query = _context.Students
                .Where(student => student.Status != Constants.Enums.Status.Deleted)
                .Include(student => student.School)
                .AsQueryable();

            if (schoolId.HasValue)
            {
                query = query.Where(x => x.School.Id == schoolId);//add theem ddk 
            }


            // Map sortBy string into a list of Expression selectors  
            Expression<Func<Student, object>>[] sortSelectors;
            if (sortBy.IsNullOrEmpty())
            {
                sortSelectors = [];
            }else
            {
                sortSelectors = sortBy.ToLower()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(f => (Expression<Func<Student, object>>)(f.Trim() switch
                    {
                        "id" => student => student.Id,
                        "age" => student => student.Age,
                        "fullname" => student => student.FirstName + " " + student.LastName,
                        "schoolname" => student => student.School.Name,
                        "balance" => student => student.Balance,
                        _ => student => student.Id
                    })).ToArray();
            }

            if(pageSize.HasValue && pageIndex == null)
            {
                pageIndex = 1;
            }
            if (pageIndex.HasValue && pageSize == null)
            {
                pageSize = 5;
            }

            query = query
                .ApplySort(isDescending, sortSelectors)
                .ApplyPaging(pageIndex, pageSize)
                .AsQueryable();
            int totalPage;
            if(pageSize == null)
            {
                totalPage = 1;
            }else
            {
                var numberOfStudents = _context.Students.Count();
                totalPage = (int)Math.Ceiling((double)numberOfStudents / (int)pageSize);
            }
                
            var data = new StudentPagingViewModel
            {
                Students = _mapper.ProjectTo<StudentViewModel>(query).ToList(),
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

        public async Task<int> PostAsync(StudentViewModel student)
        {
            //kiểm tra xem student id có bị trùng hay không
            var dupID = await _studentRepository.GetByIdAsync(student.Id);
            if (dupID != null)
            {
                throw new InvalidOperationException($"Student ID: {student.Id} already exists or is invalid.");
            }

            var name = student.FullName.Split(' ');
            //lấy school nhờ vào school name
            var school = await _schoolRepository.GetSchoolByNameAsync(student.SchoolName);//không dùng where bởi vì tìm ra một list

            if (school == null)
            {
                throw new InvalidOperationException($"School with name {student.SchoolName} does not exist.");
            }

            var data = new Student
            {
                Id = student.Id,
                FirstName = name[0],
                LastName = string.Join(" ", name.Skip(1)),
                SId = school.Id,
                School = school,

            };

            return await _schoolRepository.AddAsync(school);
        }

        public async Task<int> PutAsync(StudentViewModel student)
        {
            //tìm student
            var data = _context.Students.Find(student.Id);
            if (data == null || data.Status == Constants.Enums.Status.Deleted)
            {
                return -1;
            }
            var name = student.FullName.Split(' ');
            //kiểm tra xem người dùng có đưa đúng tên school
            var school = _schoolRepository.GetSchoolByNameAsync(student.SchoolName);

            if (school == null)
            {
                throw new InvalidOperationException($"School with name {student.SchoolName} does not exist.");
            }

            _mapper.Map(student, data);
            await _studentRepository.UpdateAsync(data);
            return data.Id;
        }

        public async Task<int> DeleteAsync(int studentID)
        {
            var data = await _studentRepository.GetByIdAsync(studentID);
            if (data == null)
            {
                throw new InvalidOperationException($"Student with ID {studentID} does not exist.");
            }

            return await _studentRepository.DeleteAsync(data);
        }
    }
}
