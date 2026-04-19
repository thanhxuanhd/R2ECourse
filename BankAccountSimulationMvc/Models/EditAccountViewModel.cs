using System.ComponentModel.DataAnnotations;

namespace BankAccountSimulationMvc.Models
{
    public class EditAccountViewModel
    {
        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Owner Name is required.")]
        [StringLength(100, ErrorMessage = "Owner Name cannot be longer than 100 characters.")]
        public string OwnerName { get; set; } = string.Empty;

        [Required]
        public AccountStatus Status { get; set; }
    }
}
