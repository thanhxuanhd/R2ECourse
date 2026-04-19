using System.ComponentModel.DataAnnotations;

namespace BankAccountSimulationMvc.Models
{
    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(20, ErrorMessage = "Account Number cannot be longer than 20 characters.")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Owner Name is required.")]
        [StringLength(100, ErrorMessage = "Owner Name cannot be longer than 100 characters.")]
        public string OwnerName { get; set; } = string.Empty;
    }
}
