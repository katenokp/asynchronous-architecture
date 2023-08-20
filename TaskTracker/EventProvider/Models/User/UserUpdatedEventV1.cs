using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.User;

public class UserUpdatedEventV1: Event<UserUpdatedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }

    [Required]
    public UserUpdatedDataV1 Data { get; set; }
}

public class UserUpdatedDataV1
{
    public UserUpdatedDataV1(Guid userId, string userName, string userRole)
    {
        UserId = userId;
        UserName = userName;
        UserRole = userRole;
    }

    public UserUpdatedDataV1()
    {
    }

    [Required]
    public Guid UserId { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string UserRole { get; set; }
}