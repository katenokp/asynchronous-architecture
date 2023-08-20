using EventProvider.Models.User;

namespace BillingService;

public class UserRepository
{
    public User? GetByPublicId(Guid publicId)
    {
        using var dbContext = new BillingDbContext();
        return dbContext.Users.FirstOrDefault(u => u.PublicId == publicId);
    }

    public User Create(Guid publicId, string name, UserRole role)
    {
        var user = new User
                   {
                       PublicId = publicId,
                       Role = role,
                       Name = name,
                       Created = DateTime.Now,
                       Updated = DateTime.Now
                   };
        using var dbContext = new BillingDbContext();
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
        return user;
    }
    
    public static UserRole ParseUserRole(string role)
    {
        return role switch
               {
                   "Popug" => UserRole.User,
                   _ => UserRole.Manager
               };
    }
    
    private User? Get(Guid publicId)
    {
        using var dbContext = new BillingDbContext();
        return dbContext.Users.FirstOrDefault(u => u.PublicId == publicId);
    }
    
    public User CreateOrUpdate(UserCreatedDataV1 data)
    {
        var savedUser = Get(data.UserId);
        if (savedUser != null)
        {
            Console.WriteLine("User with the same id already exists");
            return Update(savedUser, u =>
                                     {
                                         u.Name = data.UserName;
                                         u.Role = ParseUserRole(data.UserRole);
                                         u.Updated = DateTime.Now;
                                     });
        }

        var user = User.Create(data.UserId, data.UserName, ParseUserRole(data.UserRole));
        using var dbContext = new BillingDbContext();
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
        return user;
    }
    
    public User Update(UserUpdatedDataV1 data)
    {
        var user = Get(data.UserId);
        if (user == null)
            return Create(data.UserId, data.UserName, ParseUserRole(data.UserRole));
        
        return Update(user, u =>
                            {
                                u.Name = data.UserName;
                                u.Role = ParseUserRole(data.UserRole);
                                u.Updated = DateTime.Now;
                            });
    }

    public User Update(User user, Action<User> modifier)
    {
        modifier.Invoke(user);
        using var dbContext = new BillingDbContext();
        dbContext.Update(user);
        dbContext.SaveChanges();
        return user;
    }

    public void Delete(UserDeletedDataV1 data)
    {
        var user = Get(data.UserId);
        if (user == null)
            return;
        
        using var dbContext = new BillingDbContext();
        dbContext.Users.Remove(user);
        dbContext.SaveChanges();
    }
}