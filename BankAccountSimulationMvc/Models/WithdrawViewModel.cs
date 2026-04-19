using System.ComponentModel.DataAnnotations;

namespace BankAccountSimulationMvc.Models
{
    public class WithdrawViewModel
    {
        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Withdrawal Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Withdrawal Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public string? Description { get; set; }
    }
}
