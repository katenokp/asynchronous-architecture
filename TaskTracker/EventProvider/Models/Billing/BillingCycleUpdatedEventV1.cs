using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.Billing;

public class BillingCycleUpdatedEventV1: Event<BillingCycleUpdatedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }
    [Required]
    public BillingCycleUpdatedDataV1 Data { get; set; }
}

public class BillingCycleUpdatedDataV1
{

    public BillingCycleUpdatedDataV1()
    {
    }

    public BillingCycleUpdatedDataV1(Guid publicId, DateTime endDate)
    {
        PublicId = publicId;
        EndDate = endDate;
    }

    [Required]
    public Guid PublicId { get; set; }
    [Required]
    public DateTime EndDate { get; set; }

}