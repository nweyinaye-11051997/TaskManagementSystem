using TaskManagementSystem.Models;

namespace TaskManagementSystem.ApplicationService
{
    public interface ITaskService
    {
        Task<List<TaskEntity>> All(string Id);
        void Create(TaskEntity entity);
        void Delete(TaskEntity entity);
        void Update(TaskEntity entity);
        Task<TaskEntity> FindById(string Id);
    }
}
