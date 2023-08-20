using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.Task.Business;

public class TaskAddedEventV1: Event<TaskAddedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }
    [Required]
    public TaskAddedDataV1 Data { get; set; }
}

public class TaskAddedDataV1
{
    public TaskAddedDataV1(Guid taskId, Guid assignedTo, string title, decimal assignCost, decimal completeCost)
    {
        TaskId = taskId;
        AssignedTo = assignedTo;
        Title = title;
        AssignCost = assignCost;
        CompleteCost = completeCost;
    }

    public TaskAddedDataV1()
    {
    }

    [Required]
    public Guid TaskId { get; set; }
    [Required]
    public Guid AssignedTo { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public decimal AssignCost { get; set; }
    [Required]
    public decimal CompleteCost { get; set; }
}