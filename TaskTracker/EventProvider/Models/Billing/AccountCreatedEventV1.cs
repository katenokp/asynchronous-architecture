using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.Billing;

public class AccountCreatedEventV1: Event<AccountCreatedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }
    [Required]
    public AccountCreatedDataV1 Data { get; set; }
}

public class AccountCreatedDataV1
{
    public AccountCreatedDataV1(Guid publicId, Guid userId)
    {
        PublicId = publicId;
        UserId = userId;
    }

    public AccountCreatedDataV1()
    {
    }
    

    [Required]
    public Guid PublicId { get; set; }
    [Required]
    public Guid UserId { get; set; }
}