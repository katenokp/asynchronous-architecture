namespace AuthService;

public class User
{
    public User(string name, string password, UserRole role)
    {
        PublicId = Guid.NewGuid();
        Name = name;
        Password = password;
        Role = role;
        Created = DateTime.Now;
        Updated = Created;
    }
    public Guid PublicId { get; }
    public int Id { get; set; }
    public string Name { get; set; }
    public UserRole Role { get; set; }
    public string Password { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}