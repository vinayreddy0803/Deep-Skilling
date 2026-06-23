using System;

namespace NUnitTesting
{
    /// <summary>
    /// BankAccount class demonstrates basic banking operations with proper exception handling.
    /// Exercises: deposit, withdrawal, balance tracking, and validation.
    /// All amounts in Indian Rupees (₹)
    /// </summary>
    public class BankAccount
    {
        private string _accountHolder;
        private decimal _balance;

        public string AccountHolder => _accountHolder;
        public decimal Balance => _balance;

        public BankAccount(string accountHolder, decimal initialBalance = 0)
        {
            if (string.IsNullOrWhiteSpace(accountHolder))
                throw new ArgumentException("Account holder name cannot be empty.", nameof(accountHolder));
            
            if (initialBalance < 0)
                throw new ArgumentException("Initial balance cannot be negative.", nameof(initialBalance));

            _accountHolder = accountHolder;
            _balance = initialBalance;
        }

        /// <summary>
        /// Deposit amount into the account
        /// </summary>
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));
            
            _balance += amount;
        }

        /// <summary>
        /// Withdraw amount from the account
        /// </summary>
        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));
            
            if (amount > _balance)
                throw new InvalidOperationException($"Insufficient funds. Current balance: ₹{_balance:F2}");

            _balance -= amount;
        }

        /// <summary>
        /// Get current account balance
        /// </summary>
        public decimal GetBalance()
        {
            return _balance;
        }

        /// <summary>
        /// Transfer amount from this account to another
        /// </summary>
        public void TransferTo(BankAccount recipient, decimal amount)
        {
            if (recipient == null)
                throw new ArgumentNullException(nameof(recipient), "Recipient account cannot be null.");
            
            Withdraw(amount);  // This will validate amount and balance
            recipient.Deposit(amount);
        }
    }
}
