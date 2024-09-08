namespace TaskManagement.Models
{
    public class Employee
    {
        public string EmployeeId {  get; set; }
        public string EmployeeName { get; set; }
        public string TimeZone {  get; set; }
        public ICollection<Project> Projects { get; set; }
    }
}
