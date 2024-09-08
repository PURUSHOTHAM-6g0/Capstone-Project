using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        public string EmployeeId {  get; set; }
        public Employee Employee { get; set; }
        public ICollection<Tasks> Tasks { get; set; }
    }
}
