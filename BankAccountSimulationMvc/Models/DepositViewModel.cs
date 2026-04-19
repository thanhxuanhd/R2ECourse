using System.ComponentModel.DataAnnotations;

namespace BankAccountSimulationMvc.Models
{
    public class DepositViewModel
    {
        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Deposit Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Deposit Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public string? Description { get; set; }
    }
}
