using System.ComponentModel.DataAnnotations;

namespace BankAccountSimulationMvc.Models
{
    public class TransferViewModel
    {
        [Required]
        [Display(Name = "From Account")]
        public string FromAccountNumber { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        [Required(ErrorMessage = "Target Account Number is required.")]
        [Display(Name = "To Account")]
        public string ToAccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Transfer Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Transfer Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }
}
