using EventProvider;

namespace TaskManagement;

public class ConsumerService : BackgroundService
{
    private readonly Consumer consumer;
    private readonly UserRepository userRepository;

    public ConsumerService(UserRepository userRepository)
    {
        consumer = new Consumer(Topics.UserTopics.Registered, Topics.UserTopics.Updated, Topics.UserTopics.Deleted);
        this.userRepository = userRepository;
    }
 
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var message = consumer.Consume();
            if (!message.IsEmpty)

                switch (message.Value)
                {
                    case UserRegisteredData userRegisteredData:
                        userRepository.Create(userRegisteredData);
                        break;
                    case UserDeletedData userDeletedData:
                        userRepository.Delete(userDeletedData);
                        break;
                    case UserUpdatedData userUpdatedData:
                        userRepository.Update(userUpdatedData);
                        break;
                }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}