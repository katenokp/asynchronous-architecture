using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Models;

[PrimaryKey("Id")]
public class TaskEntity: IEntity
{
    public static TaskEntity Create(Guid publicId, string title, string? description, string? jiraId, decimal assignTaskCost, decimal completeTaskCost)
    {
        var now = DateTime.Now;
        return new TaskEntity
               {
                   PublicId = publicId,
                   Title = title,
                   Description = description,
                   JiraId = jiraId,
                   AssignTaskCost = assignTaskCost,
                   CompleteTaskCost = completeTaskCost,
                   Created = now,
                   Updated = now,
               };
    }

    public Guid PublicId { get; set; }
    public string? Description { get; set; }
    public string Title { get; set; }
    public string? JiraId { get; set; }
    public Guid? AssignedTo  { get; set; }
    public Guid? CompletedBy  { get; set; }
    
    [Column(TypeName = "decimal(5, 2)")]
    public decimal AssignTaskCost { get; set; }
    
    [Column(TypeName = "decimal(5, 2)")]
    public decimal CompleteTaskCost { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public int Id { get; set; }
    public DateTime? CompleteDate { get; set; }
    public Guid? CompleteBy { get; set; }
}