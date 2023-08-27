using System.ComponentModel.DataAnnotations.Schema;
using EventProvider;
using EventProvider.Models.Billing;
using EventProvider.Models.Task.Business;
using Microsoft.EntityFrameworkCore;

namespace BillingService;

public class BillingService
{
    private readonly BillingCycleRepository billingCycleRepository;
    private readonly AccountService accountService;
    private readonly TaskRepository taskRepository;
    private readonly Producer producer;

    public BillingService(BillingCycleRepository billingCycleRepository,
                          AccountService accountService,
                          TaskRepository taskRepository,
                          Producer producer)
    {
        this.billingCycleRepository = billingCycleRepository;
        this.accountService = accountService;
        this.taskRepository = taskRepository;
        this.producer = producer;
    }

    public async Task ApplyEnrollTransaction(TaskReassignedDataV1 data)
    {
        var account = accountService.GetOrCreate(data.AssignedToUserId);
        var billingCycle = await GetCurrentBillingCycle(account, data.AssignedToUserId);
        var task = taskRepository.Get(data.TaskId);
        if (task == null)
            throw new Exception($"Task with id [{data.TaskId}] is not exist");
        
        await CreateTransaction(billingCycle, account, task.AssignTaskCost, 0, $"Assign task {task.Title}", TransactionType.Enrollment);
    }
    
    public async Task ApplyWithdrawTransaction(TaskCompletedDataV1 data)
    {
        var account = accountService.GetOrCreate(data.UserId);
        var billingCycle = await GetCurrentBillingCycle(account, data.UserId);
        var task = taskRepository.Get(data.TaskId);
        if (task == null)
            throw new Exception($"Task with id [{data.TaskId}] is not exist");
        
        await CreateTransaction(billingCycle, account, 0, task.CompleteTaskCost, $"Complete task {task.Title}", TransactionType.Withdrawal);
    }

    public async Task Close(User user)
    {
        var account = accountService.GetOrCreate(user.PublicId);
        var billingCycle = billingCycleRepository.GetOpened(account.Id);
        if (billingCycle == null)
            return;
        
        var transaction = MakeTransaction(billingCycle.Id, 0, account.Balance, $"$Payment for account [{account.Id}]", TransactionType.Payment);
        account.Balance = 0;
        billingCycle.State = BillingCycleState.Closed;

        await using var dbContext = new BillingDbContext();
        await using var dbTransaction = await dbContext.Database.BeginTransactionAsync();
        dbContext.Update(account);
        dbContext.Update(billingCycle);
        dbContext.Add(transaction);
        await dbContext.SaveChangesAsync();
        await dbTransaction.CommitAsync();
        
        await producer.Produce(Topics.Billing,
                               EventNames.TransactionApplied,
                               new TransactionAppliedDataV1(billingCycle.PublicId,
                                                            transaction.Description,
                                                            Enum.Parse<EventProvider.Models.Billing.TransactionType>(transaction.Type.ToString()),
                                                            transaction.Debit,
                                                            transaction.Credit,
                                                            transaction.PublicId));

        await producer.Produce(Topics.Billing,
                               EventNames.BillingCycleClosed,
                               new BillingCycleClosedDataV1(billingCycle.PublicId, DateTime.Now));
    }
    
    public async Task<BillingCycle> OpenBillingCycle(Account account, Guid userPublicId)
    {
        var billingCycle = billingCycleRepository.Create(account.Id);
        await producer.Produce(Topics.Billing,
                               EventNames.BillingCycleOpened,
                               new BillingCycleOpenedDataV1(billingCycle.PublicId,
                                                             account.PublicId,
                                                             userPublicId,
                                                             DateTime.Now));
        return billingCycle;
    }

    private async Task<BillingCycle> GetCurrentBillingCycle(Account account, Guid userPublicId)
    {
        var currentBillingCycle = billingCycleRepository.GetOpened(account.Id);
        if (currentBillingCycle != null)
            return currentBillingCycle;
        
        var newBillingCycle = await OpenBillingCycle(account, userPublicId);
        return newBillingCycle;
    }

    private async Task CreateTransaction(BillingCycle billingCycle,
                                         Account account,
                                         decimal debitSum,
                                         decimal creditSum,
                                         string description,
                                         TransactionType type)
    {
        var transaction = MakeTransaction(billingCycle.Id, debitSum, creditSum, description, type);
        await using var dbContext = new BillingDbContext();
        account.Balance += creditSum;
        account.Balance -= debitSum;

        await using var dbTransaction = dbContext.Database.BeginTransaction();
        dbContext.Update(account);
        dbContext.Add(transaction);
        await dbContext.SaveChangesAsync();
        await dbTransaction.CommitAsync();
        
        await producer.Produce(Topics.Billing,
                               EventNames.TransactionApplied,
                               new TransactionAppliedDataV1(billingCycle.PublicId,
                                                            transaction.Description,
                                                            Enum.Parse<EventProvider.Models.Billing.TransactionType>(transaction.Type.ToString()),
                                                            transaction.Debit,
                                                            transaction.Credit,
                                                            transaction.PublicId));
    }

    private static Transaction MakeTransaction(int billingCycleId, decimal debitSum, decimal creditSum, string description, TransactionType type)
    {
        return new Transaction
               {
                   BillingCycleId = billingCycleId,
                   Created = DateTime.Now,
                   Updated = DateTime.Now,
                   Debit = debitSum,
                   Credit = creditSum,
                   PublicId = Guid.NewGuid(),
                   Description = description,
                   Type = type,
               };
    }
}

public class TaskRepository
{
    public TaskEntity Create(TaskAddedDataV1 data)
    {
        var taskEntity = TaskEntity.Create(data.TaskId, data.Title, data.AssignCost, data.CompleteCost);
        
        using var dbContext = new BillingDbContext();
        dbContext.Tasks.Add(taskEntity);
        dbContext.SaveChanges();
        return taskEntity;
    }

    public TaskEntity Update(Guid taskId, Action<TaskEntity> modifier)
    {
        var task = Get(taskId);
        if (task == null)
            throw new Exception($"Couldn't find task [{taskId}]");
                
        modifier.Invoke(task);
        task.Updated = DateTime.Now;

        using var dbContext = new BillingDbContext();
        dbContext.Tasks.Update(task);
        dbContext.SaveChanges();
        return task;
    }

    public TaskEntity? Get(Guid taskId)
    {
        using var dbContext = new BillingDbContext();
        return dbContext.Tasks.FirstOrDefault(t => t.PublicId == taskId);
    }
}

[PrimaryKey("Id")]
public class TaskEntity: IEntity
{
    public Guid PublicId { get; set; }
    public string Title { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal AssignTaskCost { get; set; }
    
    [Column(TypeName = "decimal(5, 2)")]
    public decimal CompleteTaskCost { get; set; }
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    
    public static TaskEntity Create(Guid taskId, string title, decimal assignTaskCost, decimal completeTaskCost)
    {
        var now = DateTime.Now;
        return new TaskEntity
               {
                   PublicId = taskId,
                   Title = title,
                   AssignTaskCost = assignTaskCost,
                   CompleteTaskCost = completeTaskCost,
                   Created = now,
                   Updated = now,
               };
    }
}