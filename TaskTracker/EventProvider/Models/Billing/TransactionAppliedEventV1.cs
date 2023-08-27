using System.ComponentModel.DataAnnotations;
using NJsonSchema.Annotations;

namespace EventProvider.Models.Billing;

[JsonSchemaFlatten]
public class TransactionAppliedEventV1: Event<TransactionAppliedDataV1>
{
}

public class TransactionAppliedDataV1
{
    public TransactionAppliedDataV1(Guid billingCyclePublicId, string description, TransactionType type, decimal debit, decimal credit, Guid publicId)
    {
        BillingCyclePublicId = billingCyclePublicId;
        Description = description;
        Type = type;
        Debit = debit;
        Credit = credit;
        PublicId = publicId;
    }

    public TransactionAppliedDataV1()
    {
    }
    
    
    [Required]
    public Guid BillingCyclePublicId { get; set; }

    [Required]
    public string Description { get; set; }
    [Required]
    public TransactionType Type { get; set; }
    
    [Required]
    public decimal Debit { get; set; }
    [Required]
    public decimal Credit { get; set; }

    [Required]
    public Guid PublicId { get; set; }
}

public enum TransactionType
{
    Enrollment = 1,
    Withdrawal = 2,
    Payment = 3,
}