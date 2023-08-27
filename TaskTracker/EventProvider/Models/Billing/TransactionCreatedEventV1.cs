using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.Billing;

public class TransactionCreatedEventV1: Event<TransactionCreatedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }
    [Required]
    public TransactionCreatedDataV1 Data { get; set; }
}

public class TransactionCreatedDataV1
{
    public TransactionCreatedDataV1(Guid billingCyclePublicId, string description, TransactionType type, decimal debit, decimal credit, Guid publicId)
    {
        BillingCyclePublicId = billingCyclePublicId;
        Description = description;
        Type = type;
        Debit = debit;
        Credit = credit;
        PublicId = publicId;
    }

    public TransactionCreatedDataV1()
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