using EventProvider;
using EventProvider.Models.User;

namespace TaskManagement;

public class ConsumerService : BackgroundService
{
    private readonly Consumer consumer;
    private readonly UserRepository userRepository;

    public ConsumerService(UserRepository userRepository)
    {
        consumer = new Consumer("task-management-group", new []{Topics.UserStreaming});
        this.userRepository = userRepository;
    }
 
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = consumer.Consume();
            if (message != null)

                switch (message)
                {
                    case UserCreatedEventV1 userCreatedEventV1:
                        userRepository.CreateOrUpdate(userCreatedEventV1.Data);
                        break;
                    case UserDeletedEventV1 userDeletedData:
                        userRepository.Delete(userDeletedData.Data);
                        break;
                    case UserUpdatedEventV1 userUpdatedData:
                        userRepository.UpdateOrCreate(userUpdatedData.Data);
                        break;
                }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}