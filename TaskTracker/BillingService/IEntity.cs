namespace BillingService;

public interface IEntity
{
    public Guid PublicId { get; set; }
    public int Id { get; set; }
    public DateTime Created { get; set; }
}