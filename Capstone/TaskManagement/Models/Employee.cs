using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class Employee
    {
        [Key]
        public string EmployeeId {  get; set; }
        public string EmployeeName { get; set; }
        public string TimeZone {  get; set; }
        public ICollection<EmployeeProject> EmployeeProjects { get; set; }
    }
}
