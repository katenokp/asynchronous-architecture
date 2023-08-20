using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.Task.Streaming;

public class TaskCreatedEventV1: Event<TaskCreatedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }
    [Required]
    public TaskCreatedDataV1 Data { get; set; }
}

public class TaskCreatedDataV1
{
    public TaskCreatedDataV1(Guid taskId, string title, string? description, string? jiraId, decimal assignCost, decimal completeCost)
    {
        TaskId = taskId;
        Title = title;
        Description = description;
        JiraId = jiraId;
        AssignCost = assignCost;
        CompleteCost = completeCost;
    }

    public TaskCreatedDataV1()
    {
    }

    [Required]
    public Guid TaskId { get; set; }
    [Required]
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? JiraId { get; set; }
    [Required]
    public decimal AssignCost { get; set; }
    [Required]
    public decimal CompleteCost { get; set; }
}