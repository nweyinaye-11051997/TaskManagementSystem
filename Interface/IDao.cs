using System.Linq.Expressions;

namespace TaskManagementSystem.Interface
{
    public interface IDao<T> where T : class
    {
        void Create(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}
