using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Interface;

namespace TaskManagementSystem.Dao
{
    public class Dao<T> : IDao<T> where T : class
    {
        private readonly AppDbContext _db;
        internal DbSet<T> dbSet;
        public Dao(AppDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public void Create(T entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
