using System.ComponentModel.DataAnnotations;

namespace EventProvider.Models.User;

public class UserCreatedEventV1: Event<UserCreatedDataV1>
{
    [Required]
    public EventInfo EventInfo { get; set; }

    [Required]
    public UserCreatedDataV1 Data { get; set; }
}

public class UserCreatedDataV1
{
    public UserCreatedDataV1(Guid userId, string userName, string userRole)
    {
        UserId = userId;
        UserName = userName;
        UserRole = userRole;
    }

    public UserCreatedDataV1()
    {
    }

    [Required]
    public Guid UserId { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string UserRole { get; set; }
}