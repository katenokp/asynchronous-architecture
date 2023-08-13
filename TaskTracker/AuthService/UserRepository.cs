using EventProvider;

namespace AuthService;

public class UserService
{
    private readonly UserRepository userRepository;
    private readonly Producer producer;

    public UserService(UserRepository userRepository, Producer producer)
    {
        this.userRepository = userRepository;
        this.producer = producer;

        foreach (var user in this.userRepository.GetAll())
        {
            producer.ProduceUserEvent(new UserRegisteredData(user.PublicId, user.Name, user.Role.ToString())).ConfigureAwait(false);
        }
    }
    
    public async Task<User> Add(AddUserModel newUser)
    {
        if (userRepository.GetOrDefault(newUser.Name) != null)
            throw new Exception("User with the same name already exists");

        var user = userRepository.Create(newUser.Name, newUser.Password, newUser.Role);
        await producer.ProduceUserEvent(new UserRegisteredData(user.PublicId, user.Name, user.Role.ToString()));
        return user;
    }
    
    public async Task Update(EditUserModel userModel)
    {
        var role = userRepository.GetRole(userModel.PublicId);
        var userRoleChanged = userModel.Role.HasValue && userModel.Role != role;

        var user = userRepository.Update(userModel.PublicId, u =>
                                                          {
                                                              if(userModel.Name != null)
                                                                u.Name = userModel.Name;
                                                              if(userModel.Password != null)
                                                                u.Password = userModel.Password;
                                                              if(userModel.Role.HasValue)
                                                                  u.Role = userModel.Role.Value;
                                                              u.Updated = DateTime.Now;
                                                          });

        await producer.ProduceUserEvent(new UserUpdatedData(user.PublicId, user.Name, user.Role.ToString()));
        if(userRoleChanged)
            await producer.ProduceUserEvent(new UserRoleChangedData(user.PublicId, user.Role.ToString()));
    }
    
    public async Task Delete(Guid publicId)
    {
        userRepository.Delete(publicId);
        await producer.ProduceUserEvent(new UserDeletedData(publicId));
    }

    public IEnumerable<User> GetAll()
    {
        return userRepository.GetAll();
    }
}

public class UserRepository
{
    
    private readonly List<User> Users = new List<User>()
                                               {
                                                   new("admin", "admin", UserRole.Admin), 
                                                   new("user", "user", UserRole.Popug)
                                               };

    public User? GetOrDefault(string name)
    {
        return Users.SingleOrDefault(u => u.Name == name);
    }

    private User Get(Guid id)
    {
        return Users.Single(x => x.PublicId == id);
    }
    
    public UserRole GetRole(Guid id)
    {
        return Users.Single(x => x.PublicId == id).Role;
    }

    public IEnumerable<User> GetAll()
    {
        return Users.ToArray();
    }

    public User Create(string name, string password, UserRole role)
    {
        var user = new User(name, password, role);
        Users.Add(user);
        return user;
    }
    
    public User Update(Guid userId, Action<User> modifier)
    {
        var user = Get(userId);
        modifier.Invoke(user);
        return user;
    }

    public void Delete(Guid userId)
    {
        Users.Remove(Get(userId));
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