using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Data;
using TaskManagement.DTOs;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var userId = GetCurrentUserId();

            var tasks = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .Where(t => t.Project.UserId == userId)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    DueDate = t.DueDate,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    AssignedToUser = t.AssignedToUser != null ? new UserDto
                    {
                        Id = t.AssignedToUser.Id,
                        Email = t.AssignedToUser.Email,
                        FullName = t.AssignedToUser.FullName,
                        CreatedAt = t.AssignedToUser.CreatedAt
                    } : null
                })
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return Ok(tasks);
        }

        
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByProject(int projectId)
        {
            var userId = GetCurrentUserId();

            // Verify project belongs to user
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.AssignedToUser)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    DueDate = t.DueDate,
                    ProjectId = t.ProjectId,
                    ProjectName = project.Name,
                    AssignedToUser = t.AssignedToUser != null ? new UserDto
                    {
                        Id = t.AssignedToUser.Id,
                        Email = t.AssignedToUser.Email,
                        FullName = t.AssignedToUser.FullName,
                        CreatedAt = t.AssignedToUser.CreatedAt
                    } : null
                })
                .ToListAsync();

            return Ok(tasks);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var userId = GetCurrentUserId();

            var task = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedToUser)
                .Where(t => t.Id == id && t.Project.UserId == userId)
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            var taskDto = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssignedToUser = task.AssignedToUser != null ? new UserDto
                {
                    Id = task.AssignedToUser.Id,
                    Email = task.AssignedToUser.Email,
                    FullName = task.AssignedToUser.FullName,
                    CreatedAt = task.AssignedToUser.CreatedAt
                } : null
            };

            return Ok(taskDto);
        }

        
        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createDto)
        {
            var userId = GetCurrentUserId();

            // Verify project belongs to user
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == createDto.ProjectId && p.UserId == userId);

            if (project == null)
            {
                return BadRequest(new { message = "Invalid project" });
            }

            // If assigned to user, verify user exists
            if (createDto.AssignedToUserId.HasValue)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == createDto.AssignedToUserId.Value);
                if (!userExists)
                {
                    return BadRequest(new { message = "Assigned user not found" });
                }
            }

            var task = new TaskItem
            {
                Title = createDto.Title,
                Description = createDto.Description,
                ProjectId = createDto.ProjectId,
                Priority = createDto.Priority,
                DueDate = createDto.DueDate,
                AssignedToUserId = createDto.AssignedToUserId,
                Status = Models.TaskStatus.Todo,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Load related data for response
            await _context.Entry(task).Reference(t => t.Project).LoadAsync();
            if (task.AssignedToUserId.HasValue)
            {
                await _context.Entry(task).Reference(t => t.AssignedToUser).LoadAsync();
            }

            var taskDto = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                ProjectId = task.ProjectId,
                ProjectName = task.Project.Name,
                AssignedToUser = task.AssignedToUser != null ? new UserDto
                {
                    Id = task.AssignedToUser.Id,
                    Email = task.AssignedToUser.Email,
                    FullName = task.AssignedToUser.FullName,
                    CreatedAt = task.AssignedToUser.CreatedAt
                } : null
            };

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, taskDto);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateDto)
        {
            var userId = GetCurrentUserId();

            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == userId);

            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            // If assigned to user, verify user exists
            if (updateDto.AssignedToUserId.HasValue)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == updateDto.AssignedToUserId.Value);
                if (!userExists)
                {
                    return BadRequest(new { message = "Assigned user not found" });
                }
            }

            task.Title = updateDto.Title;
            task.Description = updateDto.Description;
            task.Status = updateDto.Status;
            task.Priority = updateDto.Priority;
            task.DueDate = updateDto.DueDate;
            task.AssignedToUserId = updateDto.AssignedToUserId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();

            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == userId);

            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] Models.TaskStatus status)
        {
            var userId = GetCurrentUserId();

            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.UserId == userId);

            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            task.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
