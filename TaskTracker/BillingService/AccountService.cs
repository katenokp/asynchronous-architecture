using EventProvider;
using EventProvider.Models.Billing;

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
    
    public async Task<Account> GetOrCreate(Guid userPublicId)
    {
        var user = userRepository.GetByPublicId(userPublicId) ?? userRepository.Create(userPublicId, string.Empty, UserRole.User);
        var account = accountRepository.GetByUser(user.Id);
        if (account != null)
            return account;

        var newAccount = accountRepository.Create(user.Id);
        await producer.Produce(Topics.BillingStreaming,
                               EventNames.AccountCreated,
                               new AccountCreatedDataV1
                               {
                                   PublicId = newAccount.PublicId,
                                   UserId = user.PublicId
                               });
        return newAccount;
    }
    
    public Account Get(int id)
    {
        return accountRepository.Get(id);
    }
}