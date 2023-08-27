using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.Billing;

public class BillingCycleClosedEventV1: Event<BillingCycleClosedDataV1>
{
}

public class BillingCycleClosedDataV1
{

    public BillingCycleClosedDataV1()
    {
    }

    public BillingCycleClosedDataV1(Guid publicId, DateTime endDate)
    {
        PublicId = publicId;
        EndDate = endDate;
    }

    [Required]
    public Guid PublicId { get; set; }
    [Required]
    public DateTime EndDate { get; set; }

}