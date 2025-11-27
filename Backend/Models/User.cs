namespace TaskManagement.Models
{
    
    public class User
    {
        public int Id { get; set; }

        // User's email address - will be used for login
        public string Email { get; set; } = string.Empty;

        // Hashed password for security
        public string PasswordHash { get; set; } = string.Empty;

        // User's display name
        public string FullName { get; set; } = string.Empty;

        // When the user was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public ICollection<Project> Projects { get; set; } = new List<Project>();

        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    }
}
