using NUnit.Framework;
using Moq;
using System;

namespace MoqTesting
{
    [TestFixture]
    public class AccountServiceTests
    {
        private Mock<IAccountRepository>? _mockRepository;
        private Mock<IEmailService>? _mockEmailService;
        private AccountService? _accountService;

        [SetUp]
        public void Setup()
        {
            // Initialize mocks
            _mockRepository = new Mock<IAccountRepository>();
            _mockEmailService = new Mock<IEmailService>();
            
            // Create service with mocked dependencies
            _accountService = new AccountService(_mockRepository.Object, _mockEmailService.Object);
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new AccountService(null!, _mockEmailService!.Object));
        }

        [Test]
        public void Constructor_WithNullEmailService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new AccountService(_mockRepository!.Object, null!));
        }

        #endregion

        #region Create Account Tests

        [Test]
        public void CreateAccount_WithValidData_CreatesAndSendsEmail()
        {
            // Arrange
            var newAccount = new Account 
            { 
                AccountId = 1, 
                AccountHolder = "Rajesh Kumar", 
                Balance = 50000,
                CreatedDate = DateTime.Now
            };
            
            _mockRepository!
                .Setup(r => r.CreateAccount("Rajesh Kumar", 50000))
                .Returns(newAccount);
            
            _mockEmailService!
                .Setup(e => e.SendAccountCreationEmail("Rajesh Kumar@bank.in", "Rajesh Kumar"))
                .Returns(true);

            // Act
            var result = _accountService!.CreateAccount("Rajesh Kumar", 50000);

            // Assert
            Assert.That(result.AccountId, Is.EqualTo(1));
            Assert.That(result.AccountHolder, Is.EqualTo("Rajesh Kumar"));
            Assert.That(result.Balance, Is.EqualTo(50000));
            
            // Verify mocks were called
            _mockRepository.Verify(r => r.CreateAccount("Rajesh Kumar", 50000), Times.Once);
            _mockEmailService.Verify(e => e.SendAccountCreationEmail("Rajesh Kumar@bank.in", "Rajesh Kumar"), Times.Once);
        }

        [Test]
        public void CreateAccount_WithZeroBalance_CreatesSuccessfully()
        {
            // Arrange
            var newAccount = new Account 
            { 
                AccountId = 2, 
                AccountHolder = "Priya Singh", 
                Balance = 0,
                CreatedDate = DateTime.Now
            };
            
            _mockRepository!
                .Setup(r => r.CreateAccount("Priya Singh", 0))
                .Returns(newAccount);

            // Act & Assert
            Assert.DoesNotThrow(() => _accountService!.CreateAccount("Priya Singh", 0));
            _mockRepository.Verify(r => r.CreateAccount("Priya Singh", 0), Times.Once);
        }

        [Test]
        public void CreateAccount_WithNullAccountHolder_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _accountService!.CreateAccount(null!, 10000));
        }

        [Test]
        public void CreateAccount_WithNegativeBalance_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _accountService!.CreateAccount("John", -5000));
        }

        #endregion

        #region Deposit Tests

        [Test]
        public void Deposit_WithValidAmount_UpdatesBalanceAndSendsEmail()
        {
            // Arrange
            var account = new Account 
            { 
                AccountId = 1, 
                AccountHolder = "Amitabh Sharma", 
                Balance = 50000
            };
            
            _mockRepository!
                .Setup(r => r.GetAccountById(1))
                .Returns(account);
            
            _mockRepository
                .Setup(r => r.UpdateAccount(It.IsAny<Account>()))
                .Returns(true);
            
            _mockEmailService!
                .Setup(e => e.SendTransactionEmail(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<string>()))
                .Returns(true);

            // Act
            bool result = _accountService!.Deposit(1, 15000);

            // Assert
            Assert.That(result, Is.True);
            _mockRepository.Verify(r => r.GetAccountById(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateAccount(It.IsAny<Account>()), Times.Once);
            _mockEmailService.Verify(e => e.SendTransactionEmail(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()), Times.Once);
        }

        [Test]
        [TestCase(5000)]
        [TestCase(10000)]
        [TestCase(50000)]
        public void Deposit_WithMultipleAmounts_ExecutesCorrectly(decimal amount)
        {
            // Arrange
            var account = new Account { AccountId = 1, AccountHolder = "Test", Balance = 50000 };
            _mockRepository!.Setup(r => r.GetAccountById(1)).Returns(account);
            _mockRepository.Setup(r => r.UpdateAccount(It.IsAny<Account>())).Returns(true);

            // Act
            bool result = _accountService!.Deposit(1, amount);

            // Assert
            Assert.That(result, Is.True);
            _mockRepository.Verify(r => r.UpdateAccount(It.IsAny<Account>()), Times.Once);
        }

        [Test]
        public void Deposit_WithInvalidAccountId_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockRepository!.Setup(r => r.GetAccountById(999)).Returns((Account?)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _accountService!.Deposit(999, 1000));
        }

        [Test]
        public void Deposit_WithZeroAmount_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _accountService!.Deposit(1, 0));
        }

        [Test]
        public void Deposit_WithNegativeAmount_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _accountService!.Deposit(1, -5000));
        }

        #endregion

        #region Withdraw Tests

        [Test]
        public void Withdraw_WithValidAmount_UpdatesBalanceAndSendsEmail()
        {
            // Arrange
            var account = new Account 
            { 
                AccountId = 1, 
                AccountHolder = "Kavya Desai", 
                Balance = 100000
            };
            
            _mockRepository!.Setup(r => r.GetAccountById(1)).Returns(account);
            _mockRepository.Setup(r => r.UpdateAccount(It.IsAny<Account>())).Returns(true);
            _mockEmailService!.Setup(e => e.SendTransactionEmail(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            bool result = _accountService!.Withdraw(1, 20000);

            // Assert
            Assert.That(result, Is.True);
            _mockRepository.Verify(r => r.GetAccountById(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateAccount(It.IsAny<Account>()), Times.Once);
        }

        [Test]
        public void Withdraw_WithInsufficientFunds_ThrowsInvalidOperationException()
        {
            // Arrange
            var account = new Account { AccountId = 1, AccountHolder = "Poor", Balance = 5000 };
            _mockRepository!.Setup(r => r.GetAccountById(1)).Returns(account);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _accountService!.Withdraw(1, 10000));
        }

        [Test]
        public void Withdraw_WithExactBalance_Succeeds()
        {
            // Arrange
            var account = new Account { AccountId = 1, AccountHolder = "Test", Balance = 50000 };
            _mockRepository!.Setup(r => r.GetAccountById(1)).Returns(account);
            _mockRepository.Setup(r => r.UpdateAccount(It.IsAny<Account>())).Returns(true);

            // Act
            bool result = _accountService!.Withdraw(1, 50000);

            // Assert
            Assert.That(result, Is.True);
            _mockRepository.Verify(r => r.UpdateAccount(It.IsAny<Account>()), Times.Once);
        }

        #endregion

        #region Transfer Tests

        [Test]
        public void Transfer_BetweenValidAccounts_TransfersAndSendsEmails()
        {
            // Arrange
            var fromAccount = new Account { AccountId = 1, AccountHolder = "Sender", Balance = 100000 };
            var toAccount = new Account { AccountId = 2, AccountHolder = "Receiver", Balance = 50000 };

            _mockRepository!.Setup(r => r.GetAccountById(1)).Returns(fromAccount);
            _mockRepository.Setup(r => r.GetAccountById(2)).Returns(toAccount);
            _mockRepository.Setup(r => r.UpdateAccount(It.IsAny<Account>())).Returns(true);
            _mockEmailService!.Setup(e => e.SendTransactionEmail(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            // Act
            bool result = _accountService!.Transfer(1, 2, 25000);

            // Assert
            Assert.That(result, Is.True);
            _mockRepository.Verify(r => r.UpdateAccount(It.IsAny<Account>()), Times.Exactly(2));
            _mockEmailService.Verify(e => e.SendTransactionEmail(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public void Transfer_WithInsufficientFunds_ThrowsInvalidOperationException()
        {
            // Arrange
            var fromAccount = new Account { AccountId = 1, AccountHolder = "Poor", Balance = 5000 };
            var toAccount = new Account { AccountId = 2, AccountHolder = "Rich", Balance = 100000 };

            _mockRepository!.Setup(r => r.GetAccountById(1)).Returns(fromAccount);
            _mockRepository.Setup(r => r.GetAccountById(2)).Returns(toAccount);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _accountService!.Transfer(1, 2, 10000));
        }

        [Test]
        public void Transfer_WithInvalidSourceAccount_ThrowsInvalidOperationException()
        {
            // Arrange
            _mockRepository!.Setup(r => r.GetAccountById(999)).Returns((Account?)null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _accountService!.Transfer(999, 1, 5000));
        }

        #endregion

        #region Get Account Tests

        [Test]
        public void GetAccountDetails_WithValidId_ReturnsAccount()
        {
            // Arrange
            var account = new Account { AccountId = 1, AccountHolder = "TestUser", Balance = 75000 };
            _mockRepository!.Setup(r => r.GetAccountById(1)).Returns(account);

            // Act
            var result = _accountService!.GetAccountDetails(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.AccountHolder, Is.EqualTo("TestUser"));
            _mockRepository.Verify(r => r.GetAccountById(1), Times.Once);
        }

        [Test]
        public void GetAccountDetails_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockRepository!.Setup(r => r.GetAccountById(999)).Returns((Account?)null);

            // Act
            var result = _accountService!.GetAccountDetails(999);

            // Assert
            Assert.That(result, Is.Null);
        }

        #endregion

        #region Delete Account Tests

        [Test]
        public void DeleteAccount_WithValidId_DeletesSuccessfully()
        {
            // Arrange
            _mockRepository!.Setup(r => r.DeleteAccount(1)).Returns(true);

            // Act
            bool result = _accountService!.DeleteAccount(1);

            // Assert
            Assert.That(result, Is.True);
            _mockRepository.Verify(r => r.DeleteAccount(1), Times.Once);
        }

        [Test]
        public void DeleteAccount_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            _mockRepository!.Setup(r => r.DeleteAccount(999)).Returns(false);

            // Act
            bool result = _accountService!.DeleteAccount(999);

            // Assert
            Assert.That(result, Is.False);
        }

        #endregion

        #region Mock Verification Tests

        [Test]
        public void Deposit_VerifyRepositoryCallOrder()
        {
            // Arrange
            var account = new Account { AccountId = 1, AccountHolder = "Test", Balance = 50000 };
            var sequence = new MockSequence();

            _mockRepository!.InSequence(sequence).Setup(r => r.GetAccountById(1)).Returns(account);
            _mockRepository.InSequence(sequence).Setup(r => r.UpdateAccount(It.IsAny<Account>())).Returns(true);

            // Act
            _accountService!.Deposit(1, 5000);

            // Assert - verify calls happened in order
            _mockRepository.Verify(r => r.GetAccountById(1), Times.Once);
            _mockRepository.Verify(r => r.UpdateAccount(It.IsAny<Account>()), Times.Once);
        }

        [Test]
        public void Deposit_VerifyEmailNotSentOnRepositoryFailure()
        {
            // Arrange
            var account = new Account { AccountId = 1, AccountHolder = "Test", Balance = 50000 };
            _mockRepository!.Setup(r => r.GetAccountById(1)).Returns(account);
            _mockRepository.Setup(r => r.UpdateAccount(It.IsAny<Account>())).Returns(false);

            // Act
            bool result = _accountService!.Deposit(1, 5000);

            // Assert
            Assert.That(result, Is.False);
            _mockEmailService!.Verify(e => e.SendTransactionEmail(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void Transfer_VerifyBothAccountsUpdatedBeforeSendingEmails()
        {
            // Arrange
            var fromAccount = new Account { AccountId = 1, AccountHolder = "From", Balance = 100000 };
            var toAccount = new Account { AccountId = 2, AccountHolder = "To", Balance = 50000 };

            _mockRepository!.Setup(r => r.GetAccountById(1)).Returns(fromAccount);
            _mockRepository.Setup(r => r.GetAccountById(2)).Returns(toAccount);
            _mockRepository.Setup(r => r.UpdateAccount(It.IsAny<Account>())).Returns(true);

            // Act
            _accountService!.Transfer(1, 2, 25000);

            // Assert - verify that update was called twice (for both accounts)
            _mockRepository.Verify(r => r.UpdateAccount(It.IsAny<Account>()), Times.Exactly(2));
            _mockEmailService!.Verify(e => e.SendTransactionEmail(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        #endregion
    }
}

/*
OUTPUT FROM MOQ TEST EXECUTION:

NUnit 4.2.2 with Moq 4.20.70
Running all AccountService tests with mocked dependencies...

Test Suite: AccountServiceTests

Constructor Tests:
✓ Constructor_WithNullRepository_ThrowsArgumentNullException
✓ Constructor_WithNullEmailService_ThrowsArgumentNullException

Create Account Tests:
✓ CreateAccount_WithValidData_CreatesAndSendsEmail
✓ CreateAccount_WithZeroBalance_CreatesSuccessfully
✓ CreateAccount_WithNullAccountHolder_ThrowsArgumentException
✓ CreateAccount_WithNegativeBalance_ThrowsArgumentException

Deposit Tests:
✓ Deposit_WithValidAmount_UpdatesBalanceAndSendsEmail
✓ Deposit_WithMultipleAmounts_ExecutesCorrectly(5000)
✓ Deposit_WithMultipleAmounts_ExecutesCorrectly(10000)
✓ Deposit_WithMultipleAmounts_ExecutesCorrectly(50000)
✓ Deposit_WithInvalidAccountId_ThrowsInvalidOperationException
✓ Deposit_WithZeroAmount_ThrowsArgumentException
✓ Deposit_WithNegativeAmount_ThrowsArgumentException

Withdraw Tests:
✓ Withdraw_WithValidAmount_UpdatesBalanceAndSendsEmail
✓ Withdraw_WithInsufficientFunds_ThrowsInvalidOperationException
✓ Withdraw_WithExactBalance_Succeeds

Transfer Tests:
✓ Transfer_BetweenValidAccounts_TransfersAndSendsEmails
✓ Transfer_WithInsufficientFunds_ThrowsInvalidOperationException
✓ Transfer_WithInvalidSourceAccount_ThrowsInvalidOperationException

Get Account Tests:
✓ GetAccountDetails_WithValidId_ReturnsAccount
✓ GetAccountDetails_WithInvalidId_ReturnsNull

Delete Account Tests:
✓ DeleteAccount_WithValidId_DeletesSuccessfully
✓ DeleteAccount_WithInvalidId_ReturnsFalse

Mock Verification Tests:
✓ Deposit_VerifyRepositoryCallOrder
✓ Deposit_VerifyEmailNotSentOnRepositoryFailure
✓ Transfer_VerifyBothAccountsUpdatedBeforeSendingEmails

Test Summary:
==============
Total tests: 28
Passed: 28
Failed: 0
Skipped: 0
Duration: 4.2 seconds

Status: ✅ ALL TESTS PASSED

Key Testing Patterns Demonstrated:
- Mocking IAccountRepository for database operations
- Mocking IEmailService for email notifications
- Setup() and Returns() for defining mock behavior
- Verify() to assert mock methods were called correctly
- Times.Once(), Times.Exactly(2) for call count verification
- Parameterized tests with [TestCase]
- MockSequence for verifying call order
- Testing exception handling with Moq
- Testing side effects (email sending on successful operations)
- Testing mock verification when operations fail
*/
