using BankAccountSimulationMvc.Models;

namespace BankAccountSimulationMvc.Mappings
{
    public static class TransactionMappingExtensions
    {
        public static TransactionViewModel ToViewModel(this Transaction transaction)
        {
            return new TransactionViewModel
            {
                TransactionId = transaction.TransactionId,
                Type = transaction.Type,
                Amount = transaction.Amount,
                TransactionDate = transaction.TransactionDate,
                Description = transaction.Description,
                AccountNumber = transaction.AccountNumber
            };
        }
    }
}
