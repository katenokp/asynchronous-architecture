namespace AuthService;

public class AddUserModel
{
    public string Name { get; set; }
    public UserRole Role { get; set; }
    public string Password { get; set; }
}

public class EditUserModel
{
    public Guid PublicId { get; set; }
    public string? Name { get; set; }
    public UserRole? Role { get; set; }
    public string? Password { get; set; }
}