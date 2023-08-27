using Analytics.Models;
using EventProvider;
using EventProvider.Models.Billing;
using EventProvider.Models.Task.Business;
using EventProvider.Models.User;
using Microsoft.EntityFrameworkCore;
using TransactionType = Analytics.Models.TransactionType;

namespace Analytics;

public class Storage
{
    public void Save(IEntity entity)
    {
        using var dbContext = new AnalyticsDbContext();
        dbContext.Add(entity);
        dbContext.SaveChanges();
    }

    public void UpdateUser(Guid publicId, Action<User> modifier)
    {
        using var dbContext = new AnalyticsDbContext();
        var entity = dbContext.Users.Single(x => x.PublicId == publicId);
        modifier.Invoke(entity);
        entity.Updated = DateTime.Now;
        dbContext.Update(entity);
        dbContext.SaveChanges();
    }
    
    public void UpdateTask(Guid publicId, Action<TaskEntity> modifier)
    {
        using var dbContext = new AnalyticsDbContext();
        var entity = dbContext.Tasks.Single(x => x.PublicId == publicId);
        modifier.Invoke(entity);
        entity.Updated = DateTime.Now;
        dbContext.Update(entity);
        dbContext.SaveChanges();
    }
    
    public void UpdateTransaction(Guid publicId, Action<Transaction> modifier)
    {
        using var dbContext = new AnalyticsDbContext();
        var entity = dbContext.Transactions.Single(x => x.PublicId == publicId);
        modifier.Invoke(entity);
        entity.Updated = DateTime.Now;
        dbContext.Update(entity);
        dbContext.SaveChanges();
    }
    
    public void UpdateBillingCycle(Guid publicId, Action<BillingCycle> modifier)
    {
        using var dbContext = new AnalyticsDbContext();
        var entity = dbContext.BillingCycles.Single(x => x.PublicId == publicId);
        modifier.Invoke(entity);
        entity.Updated = DateTime.Now;
        dbContext.Update(entity);
        dbContext.SaveChanges();
    }
}

public class AnalyticsDbContext: DbContext
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<BillingCycle> BillingCycles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }
}

public class BackgroundExecutor : BackgroundService
{
    private readonly Storage storage;
    private readonly Consumer consumer;

    public BackgroundExecutor(Storage storage)
    {
        this.storage = storage;
        consumer = new Consumer("analytics-group", new[]
                                                   {
                                                       Topics.UserStreaming,
                                                       Topics.TaskLifeCycle,
                                                       Topics.Billing,
                                                   });
    }
 
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Consume();

            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }

    private Task Consume()
    {
        var message = consumer.Consume();
        if (message != null)
            try
            {
                switch (message)
                {
                    case UserCreatedEventV1 userCreatedEventV1:
                        CreateUser(userCreatedEventV1.Data);
                        break;
                    case UserUpdatedEventV1 userUpdatedEventV1:
                        UpdateUser(userUpdatedEventV1.Data);
                        break;
                    case TaskAddedEventV1 taskAddedEventV1:
                        AddTask(taskAddedEventV1.Data);
                        break;
                    case TaskReassignedEventV1 taskAssignedEventV1:
                        UpdateTask(taskAssignedEventV1.Data);
                        break;
                    case TaskACompletedEventV1 taskACompletedEventV1:
                        CompleteTask(taskACompletedEventV1.Data);
                        break;
                    case TransactionAppliedEventV1 transactionCreatedEventV1:
                        CreateTransaction(transactionCreatedEventV1.Data);
                        break;
                    case BillingCycleOpenedEventV1 billingCycleCreatedEventV1:
                        CreateBillingCycle(billingCycleCreatedEventV1.Data);
                        break;
                    case BillingCycleClosedEventV1 billingCycleUpdatedEventV1:
                        CloseBillingCycle(billingCycleUpdatedEventV1.Data);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        return Task.CompletedTask;
    }
    

    private void CloseBillingCycle(BillingCycleClosedDataV1 data)
    {
        storage.UpdateBillingCycle(data.PublicId, x => x.EndDate = data.EndDate);
    }

    private void CreateBillingCycle(BillingCycleOpenedDataV1 data)
    {
        storage.Save(new BillingCycle
                     {
                         PublicId = data.PublicId,
                         Created = DateTime.Now,
                         AccountPublicId = data.AccountId,
                         UserPublicId = data.UserId,
                         StartDate = data.StartDate
                     });
    }

    private void CreateTransaction(TransactionAppliedDataV1 data)
    {
        storage.Save(new Transaction
                     {
                         PublicId = data.PublicId,
                         BillingCyclePublicId = data.BillingCyclePublicId,
                         Created = DateTime.Now,
                         Updated = DateTime.Now,
                         Credit = data.Credit,
                         Debit = data.Debit,
                         Description = data.Description,
                         Type = Enum.Parse<TransactionType>(data.Type.ToString())
                     });
    }

    private void UpdateTask(TaskReassignedDataV1 data)
    {
        storage.UpdateTask(data.TaskId, t =>
                                        {
                                            t.AssignedTo = data.AssignedToUserId;
                                        });
    }
    
    private void CompleteTask(TaskCompletedDataV1 data)
    {
        storage.UpdateTask(data.TaskId, t =>
                                        {
                                            t.CompleteBy = data.UserId;
                                            t.CompleteDate = DateTime.Now; //что конечно неправда, из события надо получать
                                        });
    }

    private void CreateUser(UserCreatedDataV1 data)
    {
        storage.Save(User.Create(data.UserId, data.UserName, EnumHelpers.ParseUserRole(data.UserRole)));
    }
    
    private void AddTask(TaskAddedDataV1 data)
    {
        storage.Save(TaskEntity.Create(data.TaskId, data.Title, null, null, data.AssignCost, data.CompleteCost));
    }

    private void UpdateUser(UserUpdatedDataV1 data)
    {
        storage.UpdateUser(data.UserId, x =>
                                        {
                                            x.Name = data.UserName;
                                            x.Role = EnumHelpers.ParseUserRole(data.UserRole);
                                        });
    }
}