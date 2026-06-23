namespace MoqTesting
{
    /// <summary>
    /// Repository interface for account data access
    /// Abstracts database operations for dependency injection and testing
    /// </summary>
    public interface IAccountRepository
    {
        Account? GetAccountById(int accountId);
        bool UpdateAccount(Account account);
        bool DeleteAccount(int accountId);
        Account CreateAccount(string accountHolder, decimal initialBalance);
    }
}
