using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("[controller]")]
public class UsersController: ControllerBase
{
    private readonly UserService userService;

    public UsersController(UserService userService)
    {
        this.userService = userService;
    }
    [HttpGet("all")]
    public List<User> GetAllUsers()
    {
        return userService.GetAll().ToList();
    }
    
    [HttpPost("add")]
    public async Task<IActionResult> AddUser([FromBody]AddUserModel model)
    {
        try
        {
            var user = await userService.Add(model);
            return Ok(user.PublicId);
        }

        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("edit")]
    public async Task<IActionResult> EditUser([FromBody]EditUserModel user)
    {
        try
        {
            await userService.Update(user);
            return Ok(user);
        }
        
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("delete/{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        try
        {
            var userRole = GetUserRole();
            if(userRole == null)
                return Unauthorized("There is no claim role");
        
            if(userRole != UserRole.Admin)
                return Unauthorized("Only admin can delete user");
            await userService.Delete(userId);
            return Ok($"User [{userId}] deleted");
        }

        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    private UserRole? GetUserRole()
    {
        var claimRole = HttpContext.Request.Headers["claims_role"].FirstOrDefault();
        return claimRole != null 
                   ? Enum.Parse<UserRole>(claimRole) 
                   : default(UserRole?);
    }
}