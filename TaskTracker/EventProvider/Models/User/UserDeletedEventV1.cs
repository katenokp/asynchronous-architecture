using System.ComponentModel.DataAnnotations;
using NJsonSchema.Annotations;

namespace EventProvider.Models.User;

[JsonSchemaFlatten]
public class UserDeletedEventV1: Event<UserDeletedDataV1>
{
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