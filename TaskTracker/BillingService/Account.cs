using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BillingService;

public class Account: IEntity
{
    public int UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; }
    
    [Column(TypeName = "decimal(5, 2)")]
    public decimal Balance { get; set; }

    public Guid PublicId { get; set; }
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public ICollection<BillingCycle> BillingCycles { get; set; }
}