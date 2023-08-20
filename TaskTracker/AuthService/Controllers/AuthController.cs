using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserRepository userRepository;

    public AuthController(UserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    
    [HttpGet("[action]")]
    public IActionResult Token(string userName, string password)
    {
        var (result, user) = userRepository.TryGetUser(userName, password);
        if (user == null)
            return Unauthorized(result.ToString());
        
        var key = "G3VF4C6KFV43JH6GKCDFGJH45V36JHGV3H4C6F3GJC63HG45GH6V345GHHJ4623FJL3HCVMO1P23PZ07W8"u8.ToArray();
        // const string issuer = "arbems.com";
        // const string audience = "Public";

        var tokenDescriptor = new SecurityTokenDescriptor
                              {
                                  // Issuer = issuer,
                                  // Audience = audience,
                                  Subject = new ClaimsIdentity(new Claim[]
                                                               {
                                                                   new("Name", user.Name),
                                                                   new("Role", user.Role.ToString()),
                                                                   new("UserId", user.PublicId.ToString())
                                                               }),
                                  // Expires = DateTime.UtcNow.AddDays(7),
                                  SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                              };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(tokenHandler.WriteToken(token));
    }
}