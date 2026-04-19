using BankAccountSimulationMvc.Interfaces;
using BankAccountSimulationMvc.Mappings;
using BankAccountSimulationMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace BankAccountSimulationMvc.Controllers
{
    public class TransactionController(
        IOptions<DataResourceOptions> dataResourceOptions,
        IAccountService accountService,
        ITransactionService transactionService) : Controller
    {
        private readonly DataResourceOptions _dataResourceOptions = dataResourceOptions.Value;
        private readonly IAccountService _accountService = accountService;
        private readonly ITransactionService _transactionService = transactionService;

        public IActionResult Index(string? searchString, TransactionType? typeFilter)
        {
            var transactions = _transactionService.GetFilteredTransactions(searchString, typeFilter);

            var types = Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>();
            var selectList = types.Select(s => new SelectListItem
            {
                Text = s.ToString(),
                Value = s.ToString(),
                Selected = typeFilter.HasValue && typeFilter.Value == s
            });

            var transactionViewModels = transactions.Select(t => t.ToViewModel()).ToList();

            var viewModel = new TransactionIndexViewModel
            {
                Transactions = transactionViewModels,
                SearchString = searchString,
                TypeFilter = typeFilter,
                TypeList = selectList
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Deposit(string accountId)
        {
            if (ValidateAccount(accountId, out var account, "Cannot deposit to a frozen account.") is IActionResult error)
            {
                return error;
            }

            var model = new DepositViewModel
            {
                AccountNumber = accountId,
                Balance = account.Balance
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Deposit(DepositViewModel model)
        {
            if (ValidateDeposit(model, out var account) is IActionResult error)
            {
                return error;
            }

            account.UpdateBalance(account.Balance + model.Amount);
            _accountService.UpdateAccount(account);

            var description = string.IsNullOrWhiteSpace(model.Description) ? "Deposit" : model.Description;
            var transaction = CreateTransaction(model.AccountNumber, model.Amount, TransactionType.Deposit, description);
            _transactionService.AddTransactions([transaction]);

            TempData["SuccessMessage"] = $"Successfully deposited {model.Amount:C} to account {model.AccountNumber}.";
            return RedirectToAction("Index", "Account");
        }

        [HttpGet]
        public IActionResult Withdraw(string accountId)
        {
            if (ValidateAccount(accountId, out var account, "Cannot withdraw from a frozen account.") is IActionResult error)
            {
                return error;
            }

            var model = new WithdrawViewModel
            {
                AccountNumber = accountId,
                Balance = account.Balance
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Withdraw(WithdrawViewModel model)
        {
            if (ValidateWithdrawal(model, out var account) is IActionResult error)
            {
                return error;
            }

            account.UpdateBalance(account.Balance - model.Amount);
            _accountService.UpdateAccount(account);

            var description = string.IsNullOrWhiteSpace(model.Description) ? "Withdrawal" : model.Description;
            var transaction = CreateTransaction(model.AccountNumber, model.Amount, TransactionType.Withdraw, description);
            _transactionService.AddTransactions([transaction]);

            TempData["SuccessMessage"] = $"Successfully withdrew {model.Amount:C} from account {model.AccountNumber}.";
            return RedirectToAction("Index", "Account");
        }

        [HttpGet]
        public IActionResult Transfer(string accountId)
        {
            if (ValidateAccount(accountId, out var account, "Cannot transfer from a frozen account.") is IActionResult error)
            {
                return error;
            }

            var model = new TransferViewModel
            {
                FromAccountNumber = accountId,
                Balance = account.Balance
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Transfer(TransferViewModel model)
        {
            if (ValidateTransfer(model, out var fromAccount, out var toAccount) is IActionResult error)
            {
                return error;
            }

            // Perform transfer
            fromAccount.UpdateBalance(fromAccount.Balance - model.Amount);
            toAccount.UpdateBalance(toAccount.Balance + model.Amount);

            _accountService.UpdateAccount(fromAccount);
            _accountService.UpdateAccount(toAccount);

            var descriptionBase = string.IsNullOrWhiteSpace(model.Description) ? "" : $" - {model.Description}";

            var transactionOut = CreateTransaction(model.FromAccountNumber, model.Amount, TransactionType.Transfer, $"Transfer Out to {model.ToAccountNumber}{descriptionBase}");
            var transactionIn = CreateTransaction(model.ToAccountNumber, model.Amount, TransactionType.Transfer, $"Transfer In from {model.FromAccountNumber}{descriptionBase}");

            _transactionService.AddTransactions([transactionOut, transactionIn]);

            TempData["SuccessMessage"] = $"Successfully transferred {model.Amount:C} to account {model.ToAccountNumber}.";
            return RedirectToAction("Index", "Account");
        }

        private IActionResult? ValidateAccount(string accountId, out Account? account, string errorMessage, string errorKey = "", object? model = null, Action<Account>? onValidationError = null)
        {
            account = _accountService.GetAccount(accountId);
            if (account == null)
            {
                if (model != null && !string.IsNullOrEmpty(errorKey))
                {
                    ModelState.AddModelError(errorKey, "Account does not exist.");
                    return View(model);
                }
                TempData["ErrorMessage"] = $"Account with ID '{accountId}' was not found.";
                return RedirectToAction("Index", "Account");
            }

            if (account.Status != AccountStatus.Active)
            {
                if (model != null)
                {
                    ModelState.AddModelError(errorKey, errorMessage);
                    onValidationError?.Invoke(account);
                    return View(model);
                }
                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction("Index", "Account");
            }

            return null;
        }

        private IActionResult? ValidateModelState(object model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "There were some errors in the form. Please review the details below.");
                return View(model);
            }
            return null;
        }

        private IActionResult? ValidateInsufficientFunds(Account account, decimal amount, object model, string operationName, Action<Account>? onValidationError = null)
        {
            if (account.Balance < amount)
            {
                ModelState.AddModelError("Amount", $"Insufficient funds for this {operationName}.");
                onValidationError?.Invoke(account);
                return View(model);
            }
            return null;
        }

        private IActionResult? ValidateDeposit(DepositViewModel model, out Account? account)
        {
            if (ValidateModelState(model) is IActionResult modelError)
            {
                account = null;
                return modelError;
            }

            return ValidateAccount(model.AccountNumber, out account, "Cannot deposit to a frozen account.", model: model, onValidationError: a => model.Balance = a.Balance);
        }

        private IActionResult? ValidateWithdrawal(WithdrawViewModel model, out Account? account)
        {
            if (ValidateModelState(model) is IActionResult modelError)
            {
                account = null;
                return modelError;
            }

            if (ValidateAccount(model.AccountNumber, out account, "Cannot withdraw from a frozen account.", model: model, onValidationError: a => model.Balance = a.Balance) is IActionResult accountError)
            {
                return accountError;
            }

            return ValidateInsufficientFunds(account!, model.Amount, model, "withdrawal", a => model.Balance = a.Balance);
        }

        private IActionResult? ValidateTransfer(TransferViewModel model, out Account? fromAccount, out Account? toAccount)
        {
            toAccount = null;
            fromAccount = null;

            if (ValidateModelState(model) is IActionResult modelError)
            {
                return modelError;
            }

            if (model.FromAccountNumber == model.ToAccountNumber)
            {
                ModelState.AddModelError("ToAccountNumber", "Cannot transfer funds to the same account.");
                return View(model);
            }

            if (ValidateAccount(model.FromAccountNumber, out var sourceAccount, "Cannot transfer from a frozen account.", model: model, onValidationError: a => model.Balance = a.Balance) is IActionResult fromError)
            {
                return fromError;
            }
            fromAccount = sourceAccount;

            if (ValidateInsufficientFunds(fromAccount!, model.Amount, model, "transfer", a => model.Balance = a.Balance) is IActionResult fundsError)
            {
                return fundsError;
            }

            if (ValidateAccount(model.ToAccountNumber, out var targetAccount, "Target account is frozen and cannot receive funds.", errorKey: "ToAccountNumber", model: model, onValidationError: a => model.Balance = sourceAccount!.Balance) is IActionResult toError)
            {
                return toError;
            }
            toAccount = targetAccount;

            return null;
        }

        private static Transaction CreateTransaction(string accountNumber, decimal amount, TransactionType type, string description)
        {
            return new Transaction
            {
                AccountNumber = accountNumber,
                Amount = amount,
                Type = type,
                TransactionDate = DateTime.Now,
                Description = description
            };
        }
    }
}
