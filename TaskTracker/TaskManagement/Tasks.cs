using EventProvider;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement;

public class TaskService
{
    private readonly UserRepository userRepository;
    private readonly TaskRepository taskRepository;
    private readonly Producer producer;
    private readonly Random random;

    public TaskService(UserRepository userRepository, TaskRepository taskRepository, Producer producer)
    {
        this.userRepository = userRepository;
        this.taskRepository = taskRepository;
        this.producer = producer;
        random = new Random();
    }
    
    public async Task<TaskEntity> Create(string description)
    {
        var taskEntity = taskRepository.Create(description, 
                                               GetUserToAssign(), 
                                               GetAssignCost(),
                                               GetCompleteCost());
        
        await producer.ProduceTaskEvent(new TaskAddedData(taskEntity.PublicId, taskEntity.AssignedTo));
        await producer.ProduceTaskEvent(new TaskAssignedData(taskEntity.PublicId, taskEntity.AssignedTo));
        return taskEntity;
    }

    public async Task ReassignTasks()
    {
        var availableTasks = taskRepository.GetAvailable();
        var availableUsers = userRepository.GetByRole(UserRole.User).ToArray();
        foreach (var task in availableTasks)
        {
            var user = availableUsers[random.Next(availableUsers.Length)];
            taskRepository.Update(task.PublicId, t => t.AssignedTo = user.PublicId);
            await producer.ProduceTaskEvent(new TaskAssignedData(task.PublicId, user.PublicId));
        }
    }

    public IEnumerable<TaskEntity> GetAssignedTasks(Guid userId)
    {
        return taskRepository.GetForUser(userId);
    }
    
    public IEnumerable<TaskEntity> GetAllTasks()
    {
        return taskRepository.GetAll();
    }

    public async Task CompleteTask(Guid taskId, Guid userId)
    {
        var task = taskRepository.Get(taskId);
        if (userId != task.AssignedTo)
            throw new Exception($"User [{userId}] tried to complete task assigned to user [{task.AssignedTo}]");
                
        taskRepository.Update(task.PublicId, t => t.State = TaskState.Completed);
        await producer.ProduceTaskEvent(new TaskCompletedData(task.PublicId, task.AssignedTo));
    }

    private Guid GetUserToAssign()
    {
        var availableUsers = userRepository.GetAll()
                                           .Where(x => x.Role == UserRole.User)
                                           .ToArray();
        var randValue = random.Next(availableUsers.Length);
        return availableUsers[randValue].PublicId;
    }

    private decimal GetAssignCost()
    {
        return random.Next(10, 20);
    }

    private decimal GetCompleteCost()
    {
        return random.Next(20, 40);
    }
}

[PrimaryKey("PublicId")]
public class TaskEntity
{
    public static TaskEntity Create(string description, Guid assignedTo, decimal assignTaskCost, decimal completeTaskCost)
    {
        var now = DateTime.Now;
        return new TaskEntity
               {
                   PublicId = Guid.NewGuid(),
                   Description = description,
                   AssignedTo = assignedTo,
                   AssignTaskCost = assignTaskCost,
                   CompleteTaskCost = completeTaskCost,
                   Created = now,
                   Updated = now,
                   State = TaskState.New,
               };
    }

    public Guid PublicId { get; set; }
    public string Description { get; set; }
    public Guid AssignedTo  { get; set; }
    public decimal AssignTaskCost { get; set; }
    public decimal CompleteTaskCost { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public TaskState State { get; set; }
}


public class TaskRepository
{
    public TaskEntity Create(string description, Guid assignedTo, decimal assignTaskCost, decimal completeTaskCost)
    {
        var taskEntity = TaskEntity.Create(description, assignedTo, assignTaskCost, completeTaskCost);
        
        using var dbContext = new TasksDbContext();
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
        
        using var dbContext = new TasksDbContext();
        dbContext.Tasks.Update(task);
        dbContext.SaveChanges();
        return task;
    }

    public TaskEntity? Get(Guid taskId)
    {
        using var dbContext = new TasksDbContext();
        return dbContext.Tasks.FirstOrDefault(t => t.PublicId == taskId);
    }

    public TaskEntity[] GetAll()
    {
        using var dbContext = new TasksDbContext();
        return dbContext.Tasks.ToArray();
    }
    
    public TaskEntity[] GetForUser(Guid userId)
    {
        using var dbContext = new TasksDbContext();
        return dbContext.Tasks.Where(x => x.AssignedTo == userId && x.State == TaskState.New).ToArray();
    }
    
    public TaskEntity[] GetAvailable()
    {
        using var dbContext = new TasksDbContext();
        return dbContext.Tasks.Where(x => x.State != TaskState.Completed).ToArray();
    }
}

public enum TaskState
{
    New,
    Completed
}

public class User
{
    public Guid PublicId { get; set; }
    public UserRole UserRole { get; set; }
}

public enum UserRole
{
    Manager,
    User
}

public class TasksDbContext: DbContext
{
    public DbSet<TaskEntity> Tasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(DbHelpers.ConnectionString);
    }
}