using EventProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskController : ControllerBase
{
    private readonly TaskService taskService;
    private readonly UserRepository userRepository;

    public TaskController(TaskService taskService, UserRepository userRepository)
    {
        this.taskService = taskService;
        this.userRepository = userRepository;
    }
    
    [HttpPost("add")]
    public async Task<IActionResult> AddTask([FromBody]string description)
    {
        var taskEntity = await taskService.Create(description);
        return Ok(taskEntity);
    }
    
    [HttpPost("reassign")]
    public async Task<IActionResult> ReassignTasks()
    {
        var userRole = GetUserRole();
        if(userRole == null)
            return Unauthorized("There is no claim role");
        
        if(userRole != UserRole.Manager)
            return Unauthorized("Only managers can reassign tasks");

        await taskService.ReassignTasks();
        return Ok(taskService.GetAllTasks());
    }

    [HttpPost("complete/{taskId:guid}")]
    public async Task<IActionResult> CompleteTask(Guid taskId)
    {
        var userId = GetUserId();
        if(!userId.HasValue)
            return Unauthorized("There is no claim user_id");
        try
        {
            await taskService.CompleteTask(taskId, userId.Value);
            return Ok($"Task [{taskId}] completed");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("my")]
    public IActionResult GetTasks()
    {
        var userId = GetUserId();
        if(!userId.HasValue)
            return Unauthorized("There is no claim user_id");

        return Ok(taskService.GetAssignedTasks(userId.Value).ToArray());
    }
    
    [HttpGet("all")]
    public IActionResult GetAllTasks()
    {
        var userRole = GetUserRole();
        if(userRole == null)
            return Unauthorized("There is no claim role");
        
        return Ok(taskService.GetAllTasks().ToArray());
    }
    
    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var userRole = GetUserRole();
        if(userRole == null)
            return Unauthorized("There is no claim role");
        
        return Ok(userRepository.GetAll().ToArray());
    }
    
    private UserRole? GetUserRole()
    {
        var claimRole = HttpContext.Request.Headers["claims_role"].FirstOrDefault();
        return claimRole != null 
                   ? UserRepository.ParseUserRole(claimRole) 
                   : default(UserRole?);
    }

    private Guid? GetUserId()
    {
        var claimId = HttpContext.Request.Headers["claims_user_id"].FirstOrDefault();
        return claimId != null 
                   ? Guid.Parse(claimId) 
                   : default(Guid?);
    }
}

