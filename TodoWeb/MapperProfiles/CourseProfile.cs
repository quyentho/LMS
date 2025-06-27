using AutoMapper;
using TodoWeb.Application.Dtos.CourseModel;
using TodoWeb.Domains.Entities;

public class CourseProfile : Profile
{
    public CourseProfile()
    {
        CreateMap<Course, CourseViewModel>();
    }
}