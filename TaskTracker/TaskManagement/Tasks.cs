using System.ComponentModel.DataAnnotations.Schema;
using EventProvider;
using EventProvider.Models.Task.Business;
using EventProvider.Models.Task.Streaming;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Controllers;

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
    
    public async Task<TaskEntity> Create(AddTaskModel model)
    {
        var taskEntity = taskRepository.Create(model.Title,
                                               model.Description,
                                               model.JiraId,
                                               GetAssignCost(),
                                               GetCompleteCost());

        var data = new TaskCreatedDataV1(taskEntity.PublicId, 
                                         taskEntity.Title, 
                                         taskEntity.Description, 
                                         taskEntity.JiraId, 
                                         taskEntity.AssignTaskCost, 
                                         taskEntity.CompleteTaskCost);
        await producer.Produce(Topics.TaskStreaming, data);
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
            await producer.Produce(Topics.TaskLifeCycle, new TaskReassignedDataV1(task.PublicId, user.PublicId));
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
                
        taskRepository.Update(task.PublicId, t =>
                                             {
                                                 t.State = TaskState.Completed;
                                                 t.CompletedBy = userId;
                                             });
        await producer.Produce(Topics.TaskLifeCycle, new TaskCompletedDataV1(task.PublicId, userId));
    }
    
    private async Task AssignTask(TaskEntity taskEntity)
    {
        var assignTo = GetUserToAssign();
        taskRepository.Update(taskEntity.PublicId, t => t.AssignedTo = assignTo);
        await producer.Produce(Topics.TaskLifeCycle,
                               new TaskAddedDataV1(taskEntity.PublicId,
                                                   assignTo,
                                                   taskEntity.Title,
                                                   taskEntity.AssignTaskCost,
                                                   taskEntity.CompleteTaskCost));
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

[PrimaryKey("Id")]
public class TaskEntity
{
    public static TaskEntity Create(string title, string? description, string? jiraId, decimal assignTaskCost, decimal completeTaskCost)
    {
        var now = DateTime.Now;
        return new TaskEntity
               {
                   PublicId = Guid.NewGuid(),
                   Title = title,
                   Description = description,
                   JiraId = jiraId,
                   AssignTaskCost = assignTaskCost,
                   CompleteTaskCost = completeTaskCost,
                   Created = now,
                   Updated = now,
                   State = TaskState.New,
               };
    }

    public Guid PublicId { get; set; }
    public string? Description { get; set; }
    public string Title { get; set; }
    public string? JiraId { get; set; }
    public TaskState State { get; set; }
    public Guid? AssignedTo  { get; set; }
    public Guid? CompletedBy  { get; set; }
    
    [Column(TypeName = "decimal(5, 2)")]
    public decimal AssignTaskCost { get; set; }
    
    [Column(TypeName = "decimal(5, 2)")]
    public decimal CompleteTaskCost { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public int Id { get; set; }

}


public class TaskRepository
{
    public TaskEntity Create(string title, string? description, string? jiraId, decimal assignTaskCost, decimal completeTaskCost)
    {
        var taskEntity = TaskEntity.Create(title, description, jiraId, assignTaskCost, completeTaskCost);
        
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