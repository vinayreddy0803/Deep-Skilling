using System;

namespace MoqTesting
{
    /// <summary>
    /// Bank account model for data persistence
    /// All amounts in Indian Rupees (₹)
    /// </summary>
    public class Account
    {
        public int AccountId { get; set; }
        public string AccountHolder { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
