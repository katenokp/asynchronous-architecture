namespace BillingService;

public class BalanceService
{
    private readonly AccountRepository accountRepository;
    private readonly BillingCycleRepository billingCycleRepository;
    private readonly TransactionRepository transactionRepository;

    public BalanceService(AccountRepository accountRepository, BillingCycleRepository billingCycleRepository, TransactionRepository transactionRepository)
    {
        this.accountRepository = accountRepository;
        this.billingCycleRepository = billingCycleRepository;
        this.transactionRepository = transactionRepository;
    }
    public BalanceInfo GetBalance(User user)
    {
        var account = accountRepository.GetByUser(user.Id) ?? accountRepository.Create(user.Id);
        var currentBillingCycle = billingCycleRepository.GetOpened(account.Id) ?? billingCycleRepository.Create(account.Id);
        var transactions = transactionRepository.GetByBillingCycle(currentBillingCycle.Id);
        var payments = billingCycleRepository
                       .GetClosed(account.Id)
                       .SelectMany(x => transactionRepository.GetByBillingCycle(x.Id).Where(t => t.Type == TransactionType.Payment))
                       .ToArray();
        return new BalanceInfo(payments.Select(x => new Payment(x.Credit, x.Description, x.Created)).ToArray(),
                               transactions,
                               account.Balance);
    }
}

public record Payment(decimal Sum, string Description, DateTime Date);

public record BalanceInfo(Payment[] Payments, Transaction[] Transactions, decimal Balance);