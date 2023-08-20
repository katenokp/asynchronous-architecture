using System.Text.Json.Serialization;

namespace BillingService;

public class BillingCycle: IEntity
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public BillingCycleState State { get; set; }
    
    public int AccountId { get; set; }
    public Guid PublicId { get; set; }
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public ICollection<Transaction> Transaction { get; set; }
}