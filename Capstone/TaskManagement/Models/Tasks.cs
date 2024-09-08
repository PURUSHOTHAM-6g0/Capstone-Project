using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class Tasks
    {
        [Key] 
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription {  get; set; }

        public TaskStatus Status { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public DateTime DueDate { get; set; }
        public enum TaskStatus
        {
            Todo,
            InProgress,
            Completed
        }
    }
}
