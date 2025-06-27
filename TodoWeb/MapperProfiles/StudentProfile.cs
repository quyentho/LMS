using AutoMapper;
using TodoWeb.Application.Dtos.StudentModel;
using TodoWeb.Domains.Entities;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<Student, StudentViewModel>();
    }
}