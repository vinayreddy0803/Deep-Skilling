using System;

namespace MoqTesting
{
    /// <summary>
    /// Account service that handles business logic
    /// Depends on IAccountRepository for data access and IEmailService for notifications
    /// All amounts in Indian Rupees (₹)
    /// </summary>
    public class AccountService
    {
        private readonly IAccountRepository _repository;
        private readonly IEmailService _emailService;

        public AccountService(IAccountRepository repository, IEmailService emailService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        /// <summary>
        /// Create a new account
        /// </summary>
        public Account CreateAccount(string accountHolder, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(accountHolder))
                throw new ArgumentException("Account holder name cannot be empty.", nameof(accountHolder));
            
            if (initialBalance < 0)
                throw new ArgumentException("Initial balance cannot be negative.", nameof(initialBalance));

            var account = _repository.CreateAccount(accountHolder, initialBalance);
            
            // Send account creation email notification
            _emailService.SendAccountCreationEmail($"{accountHolder}@bank.in", accountHolder);
            
            return account;
        }

        /// <summary>
        /// Deposit amount into account
        /// </summary>
        public bool Deposit(int accountId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));

            var account = _repository.GetAccountById(accountId);
            if (account == null)
                throw new InvalidOperationException($"Account {accountId} not found.");

            account.Balance += amount;
            bool success = _repository.UpdateAccount(account);

            if (success)
            {
                // Send deposit confirmation email
                _emailService.SendTransactionEmail(
                    $"{account.AccountHolder}@bank.in",
                    "Deposit Confirmation",
                    $"₹{amount:F2} deposited to your account. New balance: ₹{account.Balance:F2}"
                );
            }

            return success;
        }

        /// <summary>
        /// Withdraw amount from account
        /// </summary>
        public bool Withdraw(int accountId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));

            var account = _repository.GetAccountById(accountId);
            if (account == null)
                throw new InvalidOperationException($"Account {accountId} not found.");

            if (amount > account.Balance)
                throw new InvalidOperationException($"Insufficient funds. Current balance: ₹{account.Balance:F2}");

            account.Balance -= amount;
            bool success = _repository.UpdateAccount(account);

            if (success)
            {
                // Send withdrawal confirmation email
                _emailService.SendTransactionEmail(
                    $"{account.AccountHolder}@bank.in",
                    "Withdrawal Confirmation",
                    $"₹{amount:F2} withdrawn from your account. New balance: ₹{account.Balance:F2}"
                );
            }

            return success;
        }

        /// <summary>
        /// Get account details
        /// </summary>
        public Account? GetAccountDetails(int accountId)
        {
            return _repository.GetAccountById(accountId);
        }

        /// <summary>
        /// Transfer amount from one account to another
        /// </summary>
        public bool Transfer(int fromAccountId, int toAccountId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Transfer amount must be greater than zero.", nameof(amount));

            var fromAccount = _repository.GetAccountById(fromAccountId);
            if (fromAccount == null)
                throw new InvalidOperationException($"Source account {fromAccountId} not found.");

            var toAccount = _repository.GetAccountById(toAccountId);
            if (toAccount == null)
                throw new InvalidOperationException($"Destination account {toAccountId} not found.");

            if (amount > fromAccount.Balance)
                throw new InvalidOperationException($"Insufficient funds in source account. Balance: ₹{fromAccount.Balance:F2}");

            // Perform transfer
            fromAccount.Balance -= amount;
            toAccount.Balance += amount;

            bool fromSuccess = _repository.UpdateAccount(fromAccount);
            if (!fromSuccess)
                return false;

            bool toSuccess = _repository.UpdateAccount(toAccount);
            if (!toSuccess)
                return false;

            // Send confirmation emails
            _emailService.SendTransactionEmail(
                $"{fromAccount.AccountHolder}@bank.in",
                "Transfer Sent",
                $"₹{amount:F2} transferred to {toAccount.AccountHolder}. New balance: ₹{fromAccount.Balance:F2}"
            );

            _emailService.SendTransactionEmail(
                $"{toAccount.AccountHolder}@bank.in",
                "Transfer Received",
                $"₹{amount:F2} received from {fromAccount.AccountHolder}. New balance: ₹{toAccount.Balance:F2}"
            );

            return true;
        }

        /// <summary>
        /// Delete account
        /// </summary>
        public bool DeleteAccount(int accountId)
        {
            return _repository.DeleteAccount(accountId);
        }
    }
}
