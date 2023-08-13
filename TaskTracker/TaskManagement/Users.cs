using EventProvider;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Controllers;

namespace TaskManagement;

[PrimaryKey("PublicId")]
public class UserEntity
{
    public static UserEntity Create(Guid publicId, string name, UserRole role)
    {
        var now = DateTime.Now;
        return new UserEntity
               {
                   PublicId = publicId,
                   Name = name,
                   Role = role,
                   Created = now,
                   Updated = now
               };
    }
    public Guid PublicId { get; set; }
    public string Name { get; set; }
    public UserRole Role { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}

public class UserRepository
{
    
    private UserEntity? Get(string name)
    {
        using var dbContext = new UsersDbContext();
        return dbContext.Users.FirstOrDefault(u => u.Name == name);
    }

    private UserEntity? Get(Guid id)
    {
        using var dbContext = new UsersDbContext();
        return dbContext.Users.FirstOrDefault(u => u.PublicId == id);
    }

    public IEnumerable<UserEntity> GetByRole(UserRole role)
    {
        using var dbContext = new UsersDbContext();
        return dbContext.Users.Where(u => u.Role == role).ToArray();
    }

    public UserEntity[] GetAll()
    {
        using var dbContext = new UsersDbContext();
        return dbContext.Users.ToArray();
    }
    
    public void Create(UserRegisteredData data)
    {
        if (Get(data.Name) != null)
        {
            Console.WriteLine("User with the same name already exists");
            return;
        }

        var user = UserEntity.Create(data.UserId, data.Name, ParseUserRole(data.Role));
        using var dbContext = new UsersDbContext();
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
    }
    
    public void Update(UserUpdatedData data)
    {
        var savedUser = Get(data.UserId);
        if (savedUser == null)
            return;

        savedUser.Name = data.Name;
        savedUser.Role = ParseUserRole(data.Role);
        savedUser.Updated = DateTime.Now;
        
        using var dbContext = new UsersDbContext();
        dbContext.Users.Update(savedUser);
        dbContext.SaveChanges();
    }

    public void Delete(UserDeletedData data)
    {
        var user = Get(data.UserId);
        if (user == null)
            return;
        
        using var dbContext = new UsersDbContext();
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

public class UsersDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(DbHelpers.ConnectionString);
    }
}

