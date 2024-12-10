using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Interface;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Dao
{
    public class TaskDao : Dao<TaskEntity>, ITaskDao
    {
        private readonly AppDbContext _db;
        public TaskDao(AppDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<List<TaskEntity>> All(string Id)
        {

            return await _db.Task
       .Where(b => b.CreateBy.Contains(Id))
        .ToListAsync();
        }
        public void Delete(TaskEntity entity)
        {
            _db.Remove(entity);
            _db.SaveChanges();

        }

        public void Update(TaskEntity entity)
        {

            _db.Update(entity);
            _db.SaveChanges();
        }

        public   void Create(TaskEntity entity)
        {
            
            _db.Task.Add(entity);
            _db.SaveChanges();


        }

        public async Task<TaskEntity> FindById(string Id)
        {
          return  await _db.Task.FindAsync(Id);
        }
    }
}
