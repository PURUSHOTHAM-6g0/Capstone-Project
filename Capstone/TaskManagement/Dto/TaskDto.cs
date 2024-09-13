namespace TaskManagement.Dto
{
    public class TaskDto
    {
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public string AssignedTo {  get; set; }
    }
}
