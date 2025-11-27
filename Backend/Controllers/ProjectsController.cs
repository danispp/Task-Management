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
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        private int GetCurrentUserId()
        {
            // Extract user ID from the JWT token claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var userId = GetCurrentUserId();

            var projects = await _context.Projects
                .Where(p => p.UserId == userId)
                .Include(p => p.Tasks) // Load tasks to count them
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    UserId = p.UserId,
                    TaskCount = p.Tasks.Count
                })
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(projects);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDetailDto>> GetProject(int id)
        {
            var userId = GetCurrentUserId();

            // Find project and include related data
            var project = await _context.Projects
                .Where(p => p.Id == id && p.UserId == userId)
                .Include(p => p.User)
                .Include(p => p.Tasks)
                .ThenInclude(t => t.AssignedToUser) // Load assigned user for each task
                .FirstOrDefaultAsync();

            // Return 404 if project not found or doesn't belong to user
            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            // Map to DTO
            var projectDto = new ProjectDetailDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                User = new UserDto
                {
                    Id = project.User.Id,
                    Email = project.User.Email,
                    FullName = project.User.FullName,
                    CreatedAt = project.User.CreatedAt
                },
                Tasks = project.Tasks.Select(t => new TaskDto
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
                }).ToList()
            };

            return Ok(projectDto);
        }

        
        [HttpPost]
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto createDto)
        {
            var userId = GetCurrentUserId();

            // Create new project
            var project = new Project
            {
                Name = createDto.Name,
                Description = createDto.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Map to DTO
            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                UserId = project.UserId,
                TaskCount = 0
            };

            // Return 201 Created with location header
            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, projectDto);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto updateDto)
        {
            var userId = GetCurrentUserId();

            // Find project
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            // Update properties
            project.Name = updateDto.Name;
            project.Description = updateDto.Description;

            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content - successful update
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var userId = GetCurrentUserId();

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }
    }
}
