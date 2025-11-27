namespace TaskManagement.Models
{
    
    public class Project
    {
        // Primary Key
        public int Id { get; set; }

        // Project name
        public string Name { get; set; } = string.Empty;

        // Project description
        public string? Description { get; set; }

        // When the project was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public int UserId { get; set; }

        
        public User User { get; set; } = null!;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
