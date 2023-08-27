using System.ComponentModel.DataAnnotations;
using NJsonSchema.Annotations;

namespace EventProvider.Models.User;

[JsonSchemaFlatten]
public class UserCreatedEventV1: Event<UserCreatedDataV1>
{
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