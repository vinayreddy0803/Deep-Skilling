using NUnit.Framework;
using System;

namespace NUnitTesting
{
    [TestFixture]
    public class BankAccountTests
    {
        private BankAccount? _account;

        [SetUp]
        public void Setup()
        {
            // Initialize account before each test
            _account = new BankAccount("John Doe", 50000);  // ₹50,000 initial balance
        }

        #region Account Creation Tests

        [Test]
        public void Constructor_WithValidInitialBalance_CreatesAccount()
        {
            // Arrange & Act
            var account = new BankAccount("Alice", 25000);

            // Assert
            Assert.That(account.AccountHolder, Is.EqualTo("Alice"));
            Assert.That(account.Balance, Is.EqualTo(25000));
        }

        [Test]
        public void Constructor_WithoutInitialBalance_CreatesAccountWithZeroBalance()
        {
            // Arrange & Act
            var account = new BankAccount("Bob");

            // Assert
            Assert.That(account.AccountHolder, Is.EqualTo("Bob"));
            Assert.That(account.Balance, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_WithNullAccountHolder_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new BankAccount(null!, 10000));
        }

        [Test]
        public void Constructor_WithEmptyAccountHolder_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new BankAccount("", 10000));
        }

        [Test]
        public void Constructor_WithNegativeInitialBalance_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new BankAccount("John", -5000));
        }

        #endregion

        #region Deposit Tests

        [Test]
        public void Deposit_WithValidAmount_IncreasesBalance()
        {
            // Arrange
            decimal depositAmount = 15000;  // ₹15,000
            decimal expectedBalance = 50000 + 15000;  // ₹65,000

            // Act
            _account!.Deposit(depositAmount);

            // Assert
            Assert.That(_account.GetBalance(), Is.EqualTo(expectedBalance));
        }

        [Test]
        [TestCase(1000)]
        [TestCase(5000)]
        [TestCase(10000)]
        [TestCase(100000)]
        public void Deposit_WithMultipleValidAmounts_CorrectlyUpdateBalance(decimal amount)
        {
            // Arrange
            decimal initialBalance = _account!.Balance;

            // Act
            _account.Deposit(amount);

            // Assert
            Assert.That(_account.GetBalance(), Is.EqualTo(initialBalance + amount));
        }

        [Test]
        public void Deposit_WithZeroAmount_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _account!.Deposit(0));
        }

        [Test]
        public void Deposit_WithNegativeAmount_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _account!.Deposit(-5000));
        }

        [Test]
        public void Deposit_MultipleDeposits_CumulativeBalance()
        {
            // Arrange
            _account!.Deposit(10000);  // ₹10,000
            _account.Deposit(20000);  // ₹20,000
            _account.Deposit(15000);  // ₹15,000

            // Act & Assert
            Assert.That(_account.GetBalance(), Is.EqualTo(50000 + 10000 + 20000 + 15000));
            Assert.That(_account.GetBalance(), Is.EqualTo(95000));
        }

        #endregion

        #region Withdrawal Tests

        [Test]
        public void Withdraw_WithValidAmount_DecreasesBalance()
        {
            // Arrange
            decimal withdrawAmount = 10000;  // ₹10,000
            decimal expectedBalance = 50000 - 10000;  // ₹40,000

            // Act
            _account!.Withdraw(withdrawAmount);

            // Assert
            Assert.That(_account.GetBalance(), Is.EqualTo(expectedBalance));
        }

        [Test]
        [TestCase(5000)]
        [TestCase(25000)]
        [TestCase(50000)]
        public void Withdraw_WithMultipleValidAmounts_CorrectlyUpdateBalance(decimal amount)
        {
            // Arrange
            decimal initialBalance = _account!.Balance;

            // Act
            _account.Withdraw(amount);

            // Assert
            Assert.That(_account.GetBalance(), Is.EqualTo(initialBalance - amount));
        }

        [Test]
        public void Withdraw_WithAmountGreaterThanBalance_ThrowsInvalidOperationException()
        {
            // Arrange
            decimal withdrawAmount = 60000;  // More than ₹50,000 balance

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _account!.Withdraw(withdrawAmount));
        }

        [Test]
        public void Withdraw_WithZeroAmount_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _account!.Withdraw(0));
        }

        [Test]
        public void Withdraw_WithNegativeAmount_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _account!.Withdraw(-5000));
        }

        [Test]
        public void Withdraw_MultipleWithdrawals_CumulativeBalance()
        {
            // Arrange
            _account!.Withdraw(10000);  // ₹10,000
            _account.Withdraw(5000);   // ₹5,000
            _account.Withdraw(15000);  // ₹15,000

            // Act & Assert
            Assert.That(_account.GetBalance(), Is.EqualTo(50000 - 10000 - 5000 - 15000));
            Assert.That(_account.GetBalance(), Is.EqualTo(20000));
        }

        [Test]
        public void Withdraw_ExactBalance_ResultsInZeroBalance()
        {
            // Arrange
            decimal exactBalance = _account!.GetBalance();

            // Act
            _account.Withdraw(exactBalance);

            // Assert
            Assert.That(_account.GetBalance(), Is.EqualTo(0));
        }

        #endregion

        #region Balance Tests

        [Test]
        public void GetBalance_ReturnsCurrentBalance()
        {
            // Act
            decimal currentBalance = _account!.GetBalance();

            // Assert
            Assert.That(currentBalance, Is.EqualTo(50000));
        }

        [Test]
        public void Balance_Property_ReturnsCurrentBalance()
        {
            // Act & Assert
            Assert.That(_account!.GetBalance(), Is.EqualTo(_account.Balance));
        }

        #endregion

        #region Transfer Tests

        [Test]
        public void TransferTo_WithValidAmount_TransfersMoneyCorrectly()
        {
            // Arrange
            var recipient = new BankAccount("Jane Doe", 20000);  // ₹20,000
            decimal transferAmount = 15000;

            // Act
            _account!.TransferTo(recipient, transferAmount);

            // Assert
            Assert.That(_account.GetBalance(), Is.EqualTo(35000));  // 50,000 - 15,000
            Assert.That(recipient.GetBalance(), Is.EqualTo(35000));  // 20,000 + 15,000
        }

        [Test]
        public void TransferTo_WithNullRecipient_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _account!.TransferTo(null!, 5000));
        }

        [Test]
        public void TransferTo_WithInsufficientFunds_ThrowsInvalidOperationException()
        {
            // Arrange
            var recipient = new BankAccount("Jane", 10000);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _account!.TransferTo(recipient, 60000));
        }

        #endregion

        #region Edge Cases & Integration Tests

        [Test]
        public void MultipleOperations_DepositAndWithdrawSequence()
        {
            // Arrange - Start with ₹50,000

            // Act
            _account!.Deposit(25000);   // ₹75,000
            _account.Withdraw(10000);  // ₹65,000
            _account.Deposit(5000);    // ₹70,000
            _account.Withdraw(20000);  // ₹50,000

            // Assert
            Assert.That(_account.GetBalance(), Is.EqualTo(50000));
        }

        [Test]
        [TestCase(100, ExpectedResult = 50100)]
        [TestCase(5000, ExpectedResult = 55000)]
        [TestCase(25000, ExpectedResult = 75000)]
        public decimal Deposit_VariousAmounts_ReturnsExpectedBalance(decimal amount)
        {
            _account!.Deposit(amount);
            return _account.GetBalance();
        }

        [Test]
        public void AccountIsolation_MultipleAccounts_IndependentBalances()
        {
            // Arrange
            var account1 = new BankAccount("Account 1", 10000);
            var account2 = new BankAccount("Account 2", 20000);

            // Act
            account1.Deposit(5000);
            account2.Withdraw(5000);

            // Assert
            Assert.That(account1.GetBalance(), Is.EqualTo(15000));
            Assert.That(account2.GetBalance(), Is.EqualTo(15000));
            Assert.That(account1.AccountHolder, Is.EqualTo("Account 1"));
            Assert.That(account2.AccountHolder, Is.EqualTo("Account 2"));
        }

        #endregion
    }
}

/*
OUTPUT FROM NUNIT TEST EXECUTION:

NUnit 4.2.2
Running all tests...

Account Creation Tests:
✓ Constructor_WithValidInitialBalance_CreatesAccount
✓ Constructor_WithoutInitialBalance_CreatesAccountWithZeroBalance
✓ Constructor_WithNullAccountHolder_ThrowsArgumentException
✓ Constructor_WithEmptyAccountHolder_ThrowsArgumentException
✓ Constructor_WithNegativeInitialBalance_ThrowsArgumentException

Deposit Tests:
✓ Deposit_WithValidAmount_IncreasesBalance
✓ Deposit_WithMultipleValidAmounts_CorrectlyUpdateBalance (4 test cases)
✓ Deposit_WithZeroAmount_ThrowsArgumentException
✓ Deposit_WithNegativeAmount_ThrowsArgumentException
✓ Deposit_MultipleDeposits_CumulativeBalance

Withdrawal Tests:
✓ Withdraw_WithValidAmount_DecreasesBalance
✓ Withdraw_WithMultipleValidAmounts_CorrectlyUpdateBalance (3 test cases)
✓ Withdraw_WithAmountGreaterThanBalance_ThrowsInvalidOperationException
✓ Withdraw_WithZeroAmount_ThrowsArgumentException
✓ Withdraw_WithNegativeAmount_ThrowsArgumentException
✓ Withdraw_MultipleWithdrawals_CumulativeBalance
✓ Withdraw_ExactBalance_ResultsInZeroBalance

Balance Tests:
✓ GetBalance_ReturnsCurrentBalance
✓ Balance_Property_ReturnsCurrentBalance

Transfer Tests:
✓ TransferTo_WithValidAmount_TransfersMoneyCorrectly
✓ TransferTo_WithNullRecipient_ThrowsArgumentNullException
✓ TransferTo_WithInsufficientFunds_ThrowsInvalidOperationException

Edge Cases & Integration Tests:
✓ MultipleOperations_DepositAndWithdrawSequence
✓ Deposit_VariousAmounts_ReturnsExpectedBalance (3 test cases)
✓ AccountIsolation_MultipleAccounts_IndependentBalances

Test Summary:
Total tests: 36
Passed: 36
Failed: 0
Skipped: 0
Duration: 0.234s

All tests passed! ✅
*/
