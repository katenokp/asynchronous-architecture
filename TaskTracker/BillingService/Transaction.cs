using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BillingService;

public class Transaction: IEntity
{
    public int BillingCycleId { get; set; }
    [JsonIgnore]
    public BillingCycle BillingCycle { get; set; }

    public string Description { get; set; }
    public TransactionType Type { get; set; }
    
    [Column(TypeName = "decimal(5, 2)")]
    public decimal Debit { get; set; }
    [Column(TypeName = "decimal(5, 2)")]
    public decimal Credit { get; set; }

    public Guid PublicId { get; set; }
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}