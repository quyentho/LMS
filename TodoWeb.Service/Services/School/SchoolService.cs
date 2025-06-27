using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoWeb.Application.Dtos.SchoolModel;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Service.Services.School
{
    public class SchoolService : ISchoolService
    {
        //inject and use IMapper
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public SchoolService(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<IEnumerable<SchoolViewModel>> GetSchools(int? schoolId)
        {
            var query = _context.School
                .Where(school => school.Status != Constants.Enums.Status.Deleted)
                .AsQueryable();

            if (schoolId.HasValue)
            {

                query = query.Where(x => x.Id == schoolId);
            }
            var data = await query.ToListAsync();
            return _mapper.Map<IEnumerable<SchoolViewModel>>(data);
        }

        public int Post(SchoolViewModel school)
        {
            var dupID = _context.School.Find(school.Id);
            if (dupID != null || school.Id < 1)
            {
                return -1;
            }
            var existingSchool = _context.School.FirstOrDefault(s => s.Name.Equals(school.Name)); 

            if (existingSchool != null)
            {
                return -1;
            }

            var data = _mapper.Map<TodoWeb.Domains.Entities.School>(school);
            var state = _context.Entry(data).State;
            _context.School.Add(data);
            state = _context.Entry(data).State;
            _context.SaveChanges();
            state = _context.Entry(data).State;
            data.Address = "123";
            state = _context.Entry(data).State;
            _context.School.Add(data);
            return data.Id;
        }

        public int Put(SchoolViewModel school)
        {
            var data = _context.School.Find(school.Id);

            if (data == null || data.Status == Constants.Enums.Status.Deleted)
            {
                return -1;
            }
            var name = school.Name.Split(' ');

            _mapper.Map(school, data);
            _context.SaveChanges();
            return data.Id;
        }

        public int Delete(int schoolId)
        {
            var data = _context.School.Find(schoolId);
            if (data == null || data.Status == Constants.Enums.Status.Deleted)
            {
                return -1;
            }
            _context.School.Remove(data);
            _context.SaveChanges();
            return data.Id;
        }

        public SchoolStudentViewModel GetSchoolDetail(int schoolId)
        {
            var school = _context.School.Find(schoolId);
            if(school == null || school.Status == Constants.Enums.Status.Deleted)
            {
                return null;
            }
            _context.Entry(school).Collection(x => x.Students).Load();
            return _mapper.Map<SchoolStudentViewModel>(school);
        }
    }
}
