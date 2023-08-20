using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.Task.Business;

public class TaskReassignedEventV1: Event<TaskReassignedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }
    [Required]
    public TaskReassignedDataV1 Data { get; set; }
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