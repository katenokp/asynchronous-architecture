using System.ComponentModel.DataAnnotations;
using NJsonSchema.Annotations;

namespace EventProvider.Models.Task.Business;

[JsonSchemaFlatten]
public class TaskReassignedEventV1: Event<TaskReassignedDataV1>
{
}

public class TaskReassignedDataV1
{
    public TaskReassignedDataV1(Guid taskId, Guid assignedToUserId)
    {
        TaskId = taskId;
        AssignedToUserId = assignedToUserId;
    }

    public TaskReassignedDataV1()
    {
    }

    [Required]
    public Guid TaskId { get; set; }
    [Required]
    public Guid AssignedToUserId { get; set; }
}