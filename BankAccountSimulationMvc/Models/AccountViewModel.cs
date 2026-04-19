namespace BankAccountSimulationMvc.Models
{
    public class AccountViewModel
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
