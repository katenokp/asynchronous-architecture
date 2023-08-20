using EventProvider;
using EventProvider.Models.User;

namespace AuthService;

public class UserService
{
    private const string ProducerName = "auth";
    private readonly UserRepository userRepository;
    private readonly Producer producer;

    public UserService(UserRepository userRepository, Producer producer)
    {
        this.userRepository = userRepository;
        this.producer = producer;
    }
    
    public async Task<User> Add(AddUserModel newUser)
    {
        if (userRepository.GetOrDefault(newUser.Name) != null)
            throw new Exception("User with the same name already exists");

        var user = userRepository.Create(newUser.Name, newUser.Password, newUser.Role);
        var data = new UserCreatedDataV1(user.PublicId, user.Name, user.Role.ToString());
        await producer.Produce(Topics.UserStreaming, EventNames.UserCreated, data);
        return user;
    }
    
    public async Task Update(EditUserModel userModel)
    {
        var user = userRepository.Get(userModel.PublicId);

        user = userRepository.Update(user, u =>
                                           {
                                               if (userModel.Name != null)
                                                   u.Name = userModel.Name;
                                               if (userModel.Password != null)
                                                   u.Password = userModel.Password;
                                               if (userModel.Role.HasValue)
                                                   u.Role = userModel.Role.Value;
                                               u.Updated = DateTime.Now;
                                           });

        var data = new UserUpdatedDataV1(user.PublicId, user.Name, user.Role.ToString());
        await producer.Produce(Topics.UserStreaming, EventNames.UserUpdated, data);
    }
    
    public async Task Delete(Guid publicId)
    {
        userRepository.Delete(publicId);
        await producer.Produce(Topics.UserStreaming, EventNames.UserDeleted, new UserDeletedDataV1(publicId));
    }

    public IEnumerable<User> GetAll()
    {
        return userRepository.GetAll();
    }
}