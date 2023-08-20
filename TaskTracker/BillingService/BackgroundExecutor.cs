using System.Text.Json;
using EventProvider;
using EventProvider.Models.Task.Business;
using EventProvider.Models.User;

namespace BillingService;

public class BackgroundExecutor : BackgroundService
{
    private readonly Consumer consumer;
    private readonly UserRepository userRepository;
    private readonly BillingService billingService;
    private readonly TaskRepository taskRepository;
    private readonly AccountRepository accountRepository;

    public BackgroundExecutor(UserRepository userRepository, BillingService billingService, TaskRepository taskRepository, AccountRepository accountRepository)
    {
        consumer = new Consumer("billing-group-1", new[]
                                                 {
                                                     Topics.UserStreaming,
                                                     Topics.TaskLifeCycle
                                                 });
        this.userRepository = userRepository;
        this.billingService = billingService;
        this.taskRepository = taskRepository;
        this.accountRepository = accountRepository;
    }
 
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Consume(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task Consume(CancellationToken stoppingToken)
    {
        var message = consumer.Consume();
        if (message != null)
            try
            {
                switch (message)
                {
                    case UserCreatedEventV1 userCreatedEventV1:
                        var user = userRepository.CreateOrUpdate(userCreatedEventV1.Data);
                        var account = accountRepository.Create(user.Id);
                        await billingService.OpenBillingCycle(account, user.PublicId);
                        break;
                    case UserDeletedEventV1 userDeletedEventV1:
                        userRepository.Delete(userDeletedEventV1.Data);
                        break;
                    case UserUpdatedEventV1 userUpdatedEventV1:
                        userRepository.Update(userUpdatedEventV1.Data);
                        break;
                    case TaskAddedEventV1 taskAddedEventV1:
                        taskRepository.Create(taskAddedEventV1.Data);
                        break;
                    case TaskReassignedEventV1 taskAssignedEventV1:
                        await billingService.ApplyEnrollTransaction(taskAssignedEventV1.Data);
                        break;
                    case TaskACompletedEventV1 taskACompletedEventV1:
                        await billingService.ApplyWithdrawTransaction(taskACompletedEventV1.Data);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var error = new Error
                            {
                                Data = JsonSerializer.Serialize((object)message)
                            };
                await using var dbConext = new BillingDbContext();
                dbConext.Errors.Add(error);
                await dbConext.SaveChangesAsync(stoppingToken);
            }
    }
}