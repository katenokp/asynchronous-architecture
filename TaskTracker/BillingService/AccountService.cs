namespace BillingService;

public class AccountService
{
    private readonly AccountRepository accountRepository;
    private readonly UserRepository userRepository;
    private readonly BillingCycleRepository billingCycleRepository;

    public AccountService(AccountRepository accountRepository, UserRepository userRepository, BillingCycleRepository billingCycleRepository)
    {
        this.accountRepository = accountRepository;
        this.userRepository = userRepository;
        this.billingCycleRepository = billingCycleRepository;
    }
    
    public Account GetOrCreate(Guid userPublicId)
    {
        var user = userRepository.GetByPublicId(userPublicId) ?? userRepository.Create(userPublicId, string.Empty, UserRole.User);
        var account = accountRepository.GetByUser(user.Id);
        if (account != null)
            return account;
        
        
        return accountRepository.Create(user.Id);
    }
    
    public Account Get(int id)
    {
        return accountRepository.Get(id);
    }
}