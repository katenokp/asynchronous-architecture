using EventProvider.Models.User;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement;

[PrimaryKey("Id")]
public class User
{
    public static User Create(Guid publicId, string name, UserRole role)
    {
        var now = DateTime.Now;
        return new User
               {
                   PublicId = publicId,
                   Name = name,
                   Role = role,
                   Created = now,
                   Updated = now
               };
    }

    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Name { get; set; }
    public UserRole Role { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}

public class UserRepository
{
    private User? Get(Guid publicId)
    {
        using var dbContext = new TasksDbContext();
        return dbContext.Users.FirstOrDefault(u => u.PublicId == publicId);
    }

    public IEnumerable<User> GetByRole(UserRole role)
    {
        using var dbContext = new TasksDbContext();
        return dbContext.Users.Where(u => u.Role == role).ToArray();
    }

    public User[] GetAll()
    {
        using var dbContext = new TasksDbContext();
        return dbContext.Users.ToArray();
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
        using var dbContext = new TasksDbContext();
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
        return user;
    }
    
    public User UpdateOrCreate(UserUpdatedDataV1 data)
    {
        var user = Get(data.UserId);
        if(user == null)
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
        using var dbContext = new TasksDbContext();
        dbContext.Update(user);
        dbContext.SaveChanges();
        return user;
    }
    
    public User Create(Guid publicId, string name, UserRole role)
    {
        var user = new User
                   {
                       PublicId = publicId,
                       Name = name,
                       Role = role,
                       Created = DateTime.Now,
                       Updated = DateTime.Now
                   };
        using var dbContext = new TasksDbContext();
        dbContext.Add(user);
        dbContext.SaveChanges();
        return user;
    }

    public void Delete(UserDeletedDataV1 data)
    {
        var user = Get(data.UserId);
        if (user == null)
            return;
        
        using var dbContext = new TasksDbContext();
        dbContext.Users.Remove(user);
        dbContext.SaveChanges();
    }

    public static UserRole ParseUserRole(string role)
    {
        return role switch
               {
                   "Popug" => UserRole.User,
                   _ => UserRole.Manager
               };
    }
}

public enum UserRole
{
    Manager = 1,
    User = 2
}