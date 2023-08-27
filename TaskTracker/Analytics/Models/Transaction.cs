using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Models;

[PrimaryKey("Id")]
public class Transaction: IEntity
{
    public Guid BillingCyclePublicId { get; set; }
    
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

public enum TransactionType
{
    Enrollment = 1,
    Withdrawal = 2,
    Payment = 3,
}


public interface IEntity
{
    public Guid PublicId { get; set; }
    public int Id { get; set; }
    public DateTime Created { get; set; }
}