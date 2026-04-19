using BankAccountSimulationMvc.Models;

namespace BankAccountSimulationMvc.Mappings
{
    public static class AccountMappingExtensions
    {
        public static AccountViewModel ToViewModel(this Account account)
        {
            return new AccountViewModel
            {
                AccountNumber = account.AccountNumber,
                OwnerName = account.OwnerName,
                Balance = account.Balance,
                Status = account.Status,
                CreatedAt = account.CreatedAt
            };
        }

        public static EditAccountViewModel ToEditViewModel(this Account account)
        {
            return new EditAccountViewModel
            {
                AccountNumber = account.AccountNumber,
                OwnerName = account.OwnerName,
                Status = account.Status
            };
        }
    }
}
