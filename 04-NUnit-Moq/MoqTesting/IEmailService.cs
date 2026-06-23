namespace MoqTesting
{
    /// <summary>
    /// Email service interface for sending notifications
    /// Abstracts email logic for dependency injection and testing with Moq
    /// </summary>
    public interface IEmailService
    {
        bool SendTransactionEmail(string recipientEmail, string subject, string message);
        bool SendAccountCreationEmail(string recipientEmail, string accountHolder);
    }
}
