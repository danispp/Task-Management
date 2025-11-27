namespace TaskManagement.Models
{
    
    public class TaskItem
    {
        // Primary Key
        public int Id { get; set; }

        // Task title
        public string Title { get; set; } = string.Empty;

        // Task description
        public string? Description { get; set; }

        // Current status of the task
        public TaskStatus Status { get; set; } = TaskStatus.Todo;

        // Task priority level
        public Priority Priority { get; set; } = Priority.Medium;

        // When the task was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional due date
        public DateTime? DueDate { get; set; }

        public int ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public int? AssignedToUserId { get; set; }

        public User? AssignedToUser { get; set; }
    }

    
    public enum TaskStatus
    {
        Todo = 0,
        InProgress = 1,
        Done = 2
    }

    
    public enum Priority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
}
