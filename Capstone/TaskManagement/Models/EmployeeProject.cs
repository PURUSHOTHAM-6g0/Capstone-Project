namespace TaskManagement.Models
{
    public class EmployeeProject
    {
        public string  EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int? TaskId { get; set; }
        public Tasks Task { get; set; }
    }
}
