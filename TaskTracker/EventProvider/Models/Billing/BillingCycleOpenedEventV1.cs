using System.ComponentModel.DataAnnotations;
using NJsonSchema.Annotations;

namespace EventProvider.Models.Billing;

[JsonSchemaFlatten]
public class BillingCycleOpenedEventV1: Event<BillingCycleOpenedDataV1>
{
}

public class BillingCycleOpenedDataV1
{
    public BillingCycleOpenedDataV1(Guid publicId, Guid accountId, Guid userId, DateTime startDate)
    {
        PublicId = publicId;
        AccountId = accountId;
        UserId = userId;
        StartDate = startDate;
    }

    public BillingCycleOpenedDataV1()
    {
    }
    

    [Required]
    public Guid PublicId { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public Guid AccountId { get; set; }
    [Required]
    public Guid UserId { get; set; }
}