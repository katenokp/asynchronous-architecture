namespace AuthService;

public class UserRepository
{
    public User? GetOrDefault(string name)
    {
        using var dbContext = new AuthDbContext();
        return dbContext.Users.SingleOrDefault(u => u.Name == name);
    }

    public User Get(Guid id)
    {
        using var dbContext = new AuthDbContext();
        return dbContext.Users.Single(x => x.PublicId.ToString() == id.ToString());
    }
    

    public IEnumerable<User> GetAll()
    {
        using var dbContext = new AuthDbContext();
        return dbContext.Users.ToArray();
    }

    public User Create(string name, string password, UserRole role)
    {
        var user = new User(name, password, role);
        using var dbContext = new AuthDbContext();
        dbContext.Add(user);
        dbContext.SaveChanges();
        return user;
    }
    
    public User Update(User user, Action<User> modifier)
    {
        modifier.Invoke(user);
        using var dbContext = new AuthDbContext();
        dbContext.Update(user);
        dbContext.SaveChanges();
        return user;
    }

    public void Delete(Guid userId)
    {
        using var dbContext = new AuthDbContext();
        dbContext.Remove(Get(userId));
        dbContext.SaveChanges();
    }

    public (CheckUserResult result, User? user) TryGetUser(string userName, string password)
    {
        var user = GetOrDefault(userName);
        if (user == null)
            return (CheckUserResult.WrongUser, null);
        
        if (!user.Password.Equals(password))
            return (CheckUserResult.WrongPassword, null);
        
        return (CheckUserResult.Ok, user);
    }
}