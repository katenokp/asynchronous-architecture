using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountingController : ControllerBase
{
    private readonly BillingService billingService;
    private readonly AccountService accountService;
    private readonly UserRepository userRepository;
    private readonly BalanceService balanceService;

    public AccountingController(BillingService billingService, AccountService accountService, UserRepository userRepository, BalanceService balanceService)
    {
        this.billingService = billingService;
        this.accountService = accountService;
        this.userRepository = userRepository;
        this.balanceService = balanceService;
    }
    
    [HttpGet("myBalance")]
    public IActionResult GetBalanceInfo()
    {
        var (actionResult, user) = GetUser();
        if (actionResult != null) 
            return actionResult;

        var balanceInfo = balanceService.GetBalance(user!);
        return Ok(balanceInfo);
    }
    

    [HttpPost("pay")]
    public IActionResult Pay()
    {
        var (actionResult, user) = GetUser();
        if (actionResult != null) 
            return actionResult;

        billingService.Close(user!);
        return Ok("Period closed");
    }

    
    private (IActionResult? actionResult, User? user) GetUser()
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return (Unauthorized("There is no claim user_id"), null);

        var user = userRepository.GetByPublicId(userId.Value);
        if (user != null) 
            return (null, user);
        
        var userRole = GetUserRole();
        if (!userRole.HasValue)
            return (Unauthorized("Unable to create user without the role claim"), null);

        return (null, userRepository.Create(userId.Value, string.Empty, userRole.Value));

    }
    
    private Guid? GetUserId()
    {
        var claimId = HttpContext.Request.Headers["claims_user_id"].FirstOrDefault();
        return claimId != null 
                   ? Guid.Parse(claimId) 
                   : default(Guid?);
    }
    
    private UserRole? GetUserRole()
    {
        var claimRole = HttpContext.Request.Headers["claims_role"].FirstOrDefault();
        return claimRole != null 
                   ? UserRepository.ParseUserRole(claimRole) 
                   : default(UserRole?);
    }
    
}
