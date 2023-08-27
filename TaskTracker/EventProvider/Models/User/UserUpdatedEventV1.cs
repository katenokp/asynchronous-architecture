using System.ComponentModel.DataAnnotations;
using NJsonSchema.Annotations;

namespace EventProvider.Models.User;

[JsonSchemaFlatten]
public class UserUpdatedEventV1: Event<UserUpdatedDataV1>
{
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