namespace BillingService;

public class AccountRepository
{
    public Account Create(int userId)
    {
        var account = new Account
                      {
                          Balance = 0,
                          Created = DateTime.Now,
                          Updated = DateTime.Now,
                          PublicId = Guid.NewGuid(),
                          UserId = userId
                      };
        using var dbContext = new BillingDbContext();
        dbContext.Add(account);
        dbContext.SaveChanges();
        return account;
    }

    public Account? GetByUser(int userId)
    {
        using var dbContext = new BillingDbContext();
        return dbContext.Accounts.FirstOrDefault(a => a.UserId == userId);
    }

    public Account Get(int id)
    {
        using var dbContext = new BillingDbContext();
        return dbContext.Accounts.Single(a => a.Id == id);
    }
}

public class TransactionRepository
{
    public Transaction[] GetByBillingCycle(int billingCycleId)
    {
        using var dbContext = new BillingDbContext();
        return dbContext.Transactions.Where(x => x.BillingCycleId == billingCycleId).ToArray();
    }
}