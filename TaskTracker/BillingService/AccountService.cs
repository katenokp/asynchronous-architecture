using EventProvider;

namespace BillingService;

public class AccountService
{
    private readonly AccountRepository accountRepository;
    private readonly UserRepository userRepository;
    private readonly Producer producer;

    public AccountService(AccountRepository accountRepository, UserRepository userRepository, Producer producer)
    {
        this.accountRepository = accountRepository;
        this.userRepository = userRepository;
        this.producer = producer;
    }
    
    public Account GetOrCreate(Guid userPublicId)
    {
        var user = userRepository.GetByPublicId(userPublicId) ?? userRepository.Create(userPublicId, string.Empty, UserRole.User);
        var account = accountRepository.GetByUser(user.Id);
        return account ?? accountRepository.Create(user.Id);
    }
    
    public Account Get(int id)
    {
        return accountRepository.Get(id);
    }
}