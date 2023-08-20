namespace BillingService;

public class BillingCycleRepository
{
    public BillingCycle Create(int accountId)
    {
        var billingCycle = new BillingCycle
                           {
                               State = BillingCycleState.Opened,
                               StartDate = DateTime.Now.Date,
                               Created = DateTime.Now,
                               Updated = DateTime.Now,
                               PublicId = Guid.NewGuid(),
                               AccountId = accountId
                           };
        using var dbContext = new BillingDbContext();
        dbContext.Add(billingCycle);
        dbContext.SaveChanges();
        return billingCycle;
    }

    public BillingCycle Get(int id)
    {
        using var dbContext = new BillingDbContext();
        return dbContext.BillingCycles.Single(b => b.Id == id);
    }
    
    public BillingCycle? GetOpened(int accountId)
    {
        using var dbContext = new BillingDbContext();
        return dbContext.BillingCycles.SingleOrDefault(b => b.AccountId == accountId && b.State == BillingCycleState.Opened);
    }
    
    public BillingCycle[] GetClosed(int accountId)
    {
        using var dbContext = new BillingDbContext();
        return dbContext.BillingCycles.Where(b => b.AccountId == accountId && b.State == BillingCycleState.Closed).ToArray();
    }
}