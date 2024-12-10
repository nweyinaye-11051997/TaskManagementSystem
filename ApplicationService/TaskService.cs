
using TaskManagementSystem.Interface;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.ApplicationService
{
    public class TaskService : ITaskService
    {
        private readonly ITaskDao _taskDao;

        public TaskService( ITaskDao taskDao)
        {
            _taskDao = taskDao;
        }

        public async Task<List<TaskEntity>> All(string Id)
        {
            return await _taskDao.All(Id);

        }

        public async Task<TaskEntity> FindById(string Id)
        {
            return await _taskDao.FindById(Id);
        }
        public  void Create(TaskEntity entity)
        {
            _taskDao.Create(entity);
        }

        public void Delete(TaskEntity entity)
        {
            _taskDao.Delete(entity);
        }

        public void Update(TaskEntity entity)
        {
            _taskDao.Update(entity);
        }
    }
}
