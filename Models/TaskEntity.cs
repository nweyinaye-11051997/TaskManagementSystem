using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class TaskEntity
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Priority { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
        public string CreateBy { get; set; }
      
    }
}
