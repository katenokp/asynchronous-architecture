using Microsoft.EntityFrameworkCore;

namespace Analytics.Models;

[PrimaryKey("Id")]
public class User: IEntity
{
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

    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Name { get; set; }
    public UserRole Role { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}