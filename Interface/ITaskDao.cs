using TaskManagementSystem.Models;

namespace TaskManagementSystem.Interface
{
    public interface ITaskDao : IDao<TaskEntity>
    {
        Task<List<TaskEntity>> All(string Id);
        void Create(TaskEntity entity);
        void Update(TaskEntity entity);
        void Delete(TaskEntity entity);

        Task<TaskEntity> FindById(string id);
    }
}
