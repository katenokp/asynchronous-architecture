using System.ComponentModel.DataAnnotations.Schema;

namespace Analytics.Models;

public class Account: IEntity
{
    public Guid UserPublicId { get; set; }
    
    [Column(TypeName = "decimal(5, 2)")]
    public decimal Balance { get; set; }

    public Guid PublicId { get; set; }
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}