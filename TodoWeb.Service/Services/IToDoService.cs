using Microsoft.EntityFrameworkCore;
using TodoWeb.Application.Dtos.ToDoModel;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Service.Services
{
    public interface IToDoService
    {
        int Post(ToDoViewModel toDo);

        Guid Generate();
    }

    public class ToDoService : IToDoService
    {

        private readonly IApplicationDbContext _dbContext;
        public ToDoService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Guid Generate()
        {
            return Guid.NewGuid();
        }

        public int Post(ToDoViewModel toDo)
        {
            var data = new ToDo
            {
                Description = toDo.Description,
            };
            _dbContext.ToDos.Add(data);//add lúc này chỉ lưu trên memmory thôi, chúng ta phải sử dụng savechange để lưu xuống database
            _dbContext.SaveChanges();
            return data.Id;
        }
    }
}
