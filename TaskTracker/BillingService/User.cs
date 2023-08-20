using System.Text.Json;
using EventProvider;
using Microsoft.EntityFrameworkCore;

namespace BillingService;

public class User: IEntity
{
    public string Name { get; set; }
    public UserRole Role { get; set; }
    public Guid PublicId { get; set; }
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }

    public static User Create(Guid publicId, string name, UserRole role)
    {
        var now = DateTime.Now;
        return new User
               {
                   PublicId = publicId,
                   Name = name,
                   Role = role,
                   Created = now,
                   Updated = now
               };
    }
}

[PrimaryKey("Id")]
public class Error
{
    public string Data { get; set; }
    public int Id { get; set; }
}