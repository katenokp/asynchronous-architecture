using System.ComponentModel.DataAnnotations;
using NJsonSchema.Annotations;

namespace EventProvider.Models.Task.Business;

[JsonSchemaFlatten]
public class TaskACompletedEventV1: Event<TaskCompletedDataV1>
{
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