using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankAccountSimulationMvc.Models
{
    public class TransactionIndexViewModel
    {
        public IEnumerable<TransactionViewModel> Transactions { get; set; } = [];
        public string? SearchString { get; set; }
        public TransactionType? TypeFilter { get; set; }
        public IEnumerable<SelectListItem> TypeList { get; set; } = [];
    }
}
