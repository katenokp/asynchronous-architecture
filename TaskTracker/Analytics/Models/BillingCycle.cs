using Microsoft.EntityFrameworkCore;

namespace Analytics.Models;

[PrimaryKey("Id")]
public class BillingCycle: IEntity
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsOpened { get; set; }
    
    public Guid AccountPublicId { get; set; }
    public Guid UserPublicId { get; set; }
    public Guid PublicId { get; set; }
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}