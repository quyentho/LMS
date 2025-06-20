using TodoWeb.Application.Dtos.ToDoModel;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures.Repositories;

namespace TodoWeb.Service.Services
{
    // Updated interface with additional methods
    public interface IToDoServiceWithRepository
    {
        Task<int> PostAsync(ToDoViewModel toDo);
        int Post(ToDoViewModel toDo);
        Task<ToDo?> GetByIdAsync(int id);
        Task<IEnumerable<ToDo>> GetAllAsync();
        Task<IEnumerable<ToDo>> GetAllIncompleteAsync();
        Task<bool> UpdateAsync(int id, ToDoViewModel toDo);
        Task<bool> DeleteAsync(int id);
        Task<bool> CompleteAsync(int id);
        Guid Generate();
    }

    // Updated service using UnitOfWork and Repository pattern
    public class ToDoServiceWithRepository : IToDoServiceWithRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToDoServiceWithRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Guid Generate()
        {
            return Guid.NewGuid();
        }

        public async Task<int> PostAsync(ToDoViewModel toDo)
        {
            var data = new ToDo
            {
                Description = toDo.Description,
                IsCompleted = false
            };

            await _unitOfWork.ToDoRepository.AddAsync(data);
            await _unitOfWork.SaveChangesAsync();
            return data.Id;
        }

        public int Post(ToDoViewModel toDo)
        {
            var data = new ToDo
            {
                Description = toDo.Description,
                IsCompleted = false
            };

            _unitOfWork.ToDoRepository.Add(data);
            _unitOfWork.SaveChanges();
            return data.Id;
        }

        public async Task<ToDo?> GetByIdAsync(int id)
        {
            return await _unitOfWork.ToDoRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ToDo>> GetAllAsync()
        {
            return await _unitOfWork.ToDoRepository.GetAllAsync();
        }

        public async Task<IEnumerable<ToDo>> GetAllIncompleteAsync()
        {
            return await _unitOfWork.ToDoRepository.FindAsync(x => !x.IsCompleted);
        }

        public async Task<bool> UpdateAsync(int id, ToDoViewModel toDo)
        {
            var existingToDo = await _unitOfWork.ToDoRepository.GetByIdAsync(id);
            if (existingToDo == null)
                return false;

            existingToDo.Description = toDo.Description;

            _unitOfWork.ToDoRepository.Update(existingToDo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingToDo = await _unitOfWork.ToDoRepository.GetByIdAsync(id);
            if (existingToDo == null)
                return false;

            _unitOfWork.ToDoRepository.Remove(existingToDo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteAsync(int id)
        {
            var existingToDo = await _unitOfWork.ToDoRepository.GetByIdAsync(id);
            if (existingToDo == null)
                return false;

            existingToDo.IsCompleted = true;
            _unitOfWork.ToDoRepository.Update(existingToDo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
