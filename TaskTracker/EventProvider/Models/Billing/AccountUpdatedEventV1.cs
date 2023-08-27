using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.Billing;

public class AccountUpdatedEventV1: Event<AccountUpdatedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }
    [Required]
    public AccountUpdatedDataV1 Data { get; set; }
}

public class AccountUpdatedDataV1
{
    public AccountUpdatedDataV1(Guid publicId, decimal balance)
    {
        PublicId = publicId;
        Balance = balance;
    }

    public AccountUpdatedDataV1()
    {
    }
    

    [Required]
    public Guid PublicId { get; set; }
    [Required]
    public decimal Balance { get; set; }
}