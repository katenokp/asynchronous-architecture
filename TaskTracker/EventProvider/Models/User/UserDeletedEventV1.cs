using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.User;

public class UserDeletedEventV1: Event<UserDeletedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }

    [Required]
    public UserDeletedDataV1 Data { get; set; }
}

public class UserDeletedDataV1
{
    public UserDeletedDataV1(Guid userId)
    {
        UserId = userId;
    }

    public UserDeletedDataV1()
    {
    }

    [Required]
    public Guid UserId { get; set; }
}