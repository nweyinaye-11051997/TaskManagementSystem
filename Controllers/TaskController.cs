using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.ApplicationService;
using TaskManagementSystem.Models;
using TaskManagementSystem.ViewModel;

namespace TaskManagementSystem.Controllers
{
   
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            
            _taskService = taskService;

        }

        public async Task<IActionResult> List(string searchString,string Status ,string Priority, DateTime? DueDate, string currentFilter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var list = await _taskService.All(userId);
            if (!string.IsNullOrEmpty(searchString))
            {
                list = list.Where(t => t.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                                       t.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            
            if (!string.IsNullOrEmpty(Status))
            {
                list = list.Where(t => t.Status.Equals(Status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            
            if (!string.IsNullOrEmpty(Priority))
            {
                list = list.Where(t => t.Priority.Equals(Priority, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (DueDate.HasValue)
            {
                list = list.Where(t => t.StartDate == DueDate.Value.Date || t.EndDate == DueDate.Value.Date).ToList();
            }
            return View(list);
        }

        public  IActionResult Create()
        {
            return View();   
        }

        [HttpPost]
        public IActionResult Create(CreateTaskVM view)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           
            var obj = new TaskEntity
            {
                Id = Guid.NewGuid().ToString(),
                Title = view.Title,
                Description = view.Description,
                Priority = view.Priority,
                StartDate = view.StartDate,
                EndDate = view.EndDate,
                Status = view.Status,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                CreateBy = userId

            };
             _taskService.Create(obj);
            return RedirectToAction("List", "Task");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TaskEntity view)
        {

            var obj = await _taskService.FindById(view.Id);

            if (obj is not null)
            {

                obj.Title = view.Title;
                obj.Description = view.Description;
                obj.Priority = view.Priority;
                obj.StartDate = view.StartDate;
                obj.EndDate = view.EndDate;
                obj.Status = view.Status;
                obj.UpdateDate = DateTime.Now;
                _taskService.Update(obj);
            }
            return RedirectToAction("List", "Task");
        }

        public async Task<IActionResult> Edit(string Id)
        {
            var obj = await _taskService.FindById(Id);
            return View(obj);
        }

        public  IActionResult Cancel() { 
            return RedirectToAction("List", "Task");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(TaskEntity view)
        {
            try
            {
                var obj = await _taskService.FindById(view.Id);
                if (obj is not null)
                {
                    _taskService.Delete(obj);
                }
            }
            catch (DataException/* dex */)
            {
                return RedirectToAction("Edit", "Task");
            }
            return RedirectToAction("List", "Task");
        }
    }
}
