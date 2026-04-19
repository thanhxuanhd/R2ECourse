using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankAccountSimulationMvc.Models
{
    public class AccountIndexViewModel
    {
        public IEnumerable<AccountViewModel> Accounts { get; set; } = Enumerable.Empty<AccountViewModel>();
        public string? SearchString { get; set; }
        public AccountStatus? StatusFilter { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
