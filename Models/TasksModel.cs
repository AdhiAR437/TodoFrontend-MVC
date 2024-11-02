using System;
using System.ComponentModel.DataAnnotations;

namespace TodoFrontend_MVC.Models
{
    public class TasksModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Task name is required.")]
        public string TaskName { get; set; } = string.Empty;

        public bool Status { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

 
        public DateTime? CompletedDate { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        // Uncomment if you want to include the navigation property
        // [JsonIgnore]
        // public virtual UserModel? User { get; set; }
    }
}
