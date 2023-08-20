using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.Task.Business;

public class TaskACompletedEventV1: Event<TaskCompletedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }
    [Required]
    public TaskCompletedDataV1 Data { get; set; }
}

public class TaskCompletedDataV1
{
    public TaskCompletedDataV1()
    {
        
    }

    public TaskCompletedDataV1(Guid taskId, Guid userId)
    {
        TaskId = taskId;
        UserId = userId;
    }

    [Required]
    public Guid TaskId { get; set; }
    [Required]
    public Guid UserId { get; set; }
}