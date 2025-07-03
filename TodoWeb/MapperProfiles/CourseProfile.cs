using AutoMapper;
using TodoWeb.Application.Dtos.CourseModel;
using TodoWeb.Domains.Entities;

namespace TodoWeb.MapperProfiles
{
    public class CourseProfile: Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseViewModel>();
        }
    }
}
