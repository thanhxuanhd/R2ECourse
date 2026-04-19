using BankAccountSimulationMvc.Interfaces;
using BankAccountSimulationMvc.Mappings;
using BankAccountSimulationMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace BankAccountSimulationMvc.Controllers
{
    public class AccountController(IAccountService accountService, IOptions<DataResourceOptions> dataResourceOptions) : Controller
    {
        private readonly IAccountService _accountService = accountService;
        private readonly DataResourceOptions _dataResourceOptions = dataResourceOptions.Value;

        public IActionResult Index(string? searchString, AccountStatus? statusFilter)
        {
            var accounts = _accountService.GetFilteredAccounts(searchString, statusFilter);

            var statuses = Enum.GetValues(typeof(AccountStatus)).Cast<AccountStatus>();
            var selectList = statuses.Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString(),
                Selected = statusFilter.HasValue && statusFilter.Value == s
            });

            var accountViewModels = accounts.Select(a => a.ToViewModel()).ToList();

            var viewModel = new AccountIndexViewModel
            {
                Accounts = accountViewModels,
                SearchString = searchString,
                StatusFilter = statusFilter,
                StatusList = selectList
            };

            return View(viewModel);
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Account number is required.";
                return RedirectToAction(nameof(Index));
            }

            var account = _accountService.GetAccount(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = $"Account with ID '{id}' was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(account);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("AccountNumber,OwnerName")] CreateAccountViewModel viewModel)
        {
            if (ValidateCreate(viewModel) is IActionResult error)
            {
                return error;
            }

            var account = new Account
            {
                AccountNumber = viewModel.AccountNumber,
                OwnerName = viewModel.OwnerName,
                CreatedAt = DateTime.Now,
                Status = AccountStatus.Active
            };

            account.UpdateBalance(0);

            _accountService.AddAccount(account);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "Account number is required.";
                return RedirectToAction(nameof(Index));
            }

            var account = _accountService.GetAccount(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = $"Account with ID '{id}' was not found.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = account.ToEditViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("AccountNumber,OwnerName,Status")] EditAccountViewModel viewModel)
        {
            if (ValidateEdit(id, viewModel, out var account) is IActionResult error)
            {
                return error;
            }

            account.OwnerName = viewModel.OwnerName;
            account.Status = viewModel.Status;

            _accountService.UpdateAccount(account);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(string id)
        {
            var account = _accountService.GetAccount(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = $"Account with ID '{id}' was not found.";
                return RedirectToAction(nameof(Index));
            }

            var newStatus = account.Status == AccountStatus.Active ? AccountStatus.Frozen : AccountStatus.Active;
            _accountService.UpdateStatus(id, newStatus);

            return RedirectToAction(nameof(Index));
        }

        private IActionResult? ValidateModelState(object viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "There were some errors in the form. Please review the details below.");
                return View(viewModel);
            }
            return null;
        }

        private IActionResult? ValidateCreate(CreateAccountViewModel viewModel)
        {
            if (ValidateModelState(viewModel) is IActionResult modelError)
            {
                return modelError;
            }

            if (_accountService.IsExistingAccount(viewModel.AccountNumber))
            {
                ModelState.AddModelError("AccountNumber", "Account number already exists.");
                return View(viewModel);
            }

            return null;
        }

        private IActionResult? ValidateEdit(string id, EditAccountViewModel viewModel, out Account? account)
        {
            account = null;
            if (id != viewModel.AccountNumber)
            {
                TempData["ErrorMessage"] = "Account number mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (ValidateModelState(viewModel) is IActionResult modelError)
            {
                return modelError;
            }

            account = _accountService.GetAccount(id);
            if (account == null)
            {
                TempData["ErrorMessage"] = $"Account with ID '{id}' was not found.";
                return RedirectToAction(nameof(Index));
            }

            return null;
        }
    }
}