using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.Billing;

public class BillingCycleCreatedEventV1: Event<BillingCycleCreatedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }
    [Required]
    public BillingCycleCreatedDataV1 Data { get; set; }
}

public class BillingCycleCreatedDataV1
{
    public BillingCycleCreatedDataV1(Guid publicId, Guid accountId, Guid userId, DateTime startDate)
    {
        PublicId = publicId;
        AccountId = accountId;
        UserId = userId;
        StartDate = startDate;
    }

    public BillingCycleCreatedDataV1()
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