using BankAccountSimulation.Models;
using BankAccountSimulation.Services;

namespace BankAccountSimulation;

public class BankManager(IAccountService accountService, ITransactionService transactionService)
{
    private readonly IAccountService _accountService = accountService;
    private readonly ITransactionService _transactionService = transactionService;
    private bool _running = true;
    private const decimal MinimumBalance = 100;

    public void Run()
    {
        try
        {
            while (_running)
            {
                PrintOptions();
                HandleOptions();
            }
        }
        catch (Exception ex)
        {
            PrintMessage($"An error occurred: {ex.Message}", ConsoleColor.Red);
        }
    }

    private void HandleOptions()
    {
        Console.Write("Enter your choice: ");
        var choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                CreateAccount();
                break;

            case "2":
                DepositMoney();
                break;

            case "3":
                WithdrawMoney();
                break;

            case "4":
                TransferMoney();
                break;

            case "5":
                ViewAccountDetails();
                break;

            case "6":
                ViewTransactionHistory();
                break;

            case "7":
                FreezeUnfreezeAccount();
                break;

            case "8":
                _running = false;
                break;

            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    private void FreezeUnfreezeAccount()
    {
        Console.WriteLine($"{Environment.NewLine}=== Freeze / Unfreeze Account ===");

        var accountNumber = GetValidatedInput("Enter Account Number", input =>
        {
            if (!_accountService.IsExistingAccount(input))
            {
                return (false, "Error: Account not found.");
            }

            return (true, string.Empty);
        });

        if (string.IsNullOrEmpty(accountNumber))
        {
            return;
        }

        var account = _accountService.GetAccount(accountNumber);

        Console.WriteLine($"Current Status: {account.Status}");
        Console.WriteLine("""
            Select Action:
            1. Freeze
            2. Unfreeze
            """);
        Console.Write("Choice: ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                if (!account.IsAccountActive)
                {
                    PrintMessage("Error: Account is already Frozen.", ConsoleColor.Red);
                }
                else
                {
                    _accountService.UpdateStatus(accountNumber, AccountStatus.Frozen);
                    PrintMessage("Account frozen successfully.", ConsoleColor.Green);
                }
                break;

            case "2":
                if (account.IsAccountActive)
                {
                    PrintMessage("Error: Account is already Active.", ConsoleColor.Red);
                }
                else
                {
                    _accountService.UpdateStatus(accountNumber, AccountStatus.Active);
                    PrintMessage("Account unfrozen successfully.", ConsoleColor.Green);
                }
                break;

            default:
                PrintMessage("Invalid choice.", ConsoleColor.Red);
                break;
        }
    }

    private void ViewTransactionHistory()
    {
        var accountNumber = GetValidatedInput("Enter Account Number", input =>
        {
            if (!_accountService.IsExistingAccount(input))
            {
                return (false, "Error: Account not found.");
            }

            return (true, string.Empty);
        });

        if (string.IsNullOrEmpty(accountNumber))
        {
            return;
        }

        Console.WriteLine("""
            Select Filter:
            1. All Transactions
            2. Deposits Only
            3. Withdrawals Only
            """);
        Console.Write("Choice: ");
        var filterChoice = Console.ReadLine();

        TransactionType? type = null;
        switch (filterChoice)
        {
            case "2":
                type = TransactionType.Deposit;
                break;

            case "3":
                type = TransactionType.Withdraw;
                break;
        }

        var filteredTransactions = _transactionService.GetFilteredTransactions(accountNumber, type);
        DisplayTransaction(filteredTransactions, accountNumber);
    }

    private void DisplayTransaction(IEnumerable<Transaction> filteredTransactions, string accountNumber)
    {
        Console.WriteLine($"{Environment.NewLine}--- Transaction History for {accountNumber} ---");
        Console.WriteLine($"{"Date",-20} | {"Type",-10} | {"Amount",-12} | {"Description"}");
        Console.WriteLine(new string('-', 80));

        var transactionList = filteredTransactions.ToList();
        if (transactionList.Count == 0)
        {
            PrintMessage($"No transactions found with {accountNumber}.", ConsoleColor.Blue);
            return;
        }

        foreach (var t in transactionList)
        {
            Console.WriteLine(t.ToString());
        }

        Console.WriteLine(new string('-', 80));
    }

    private void ViewAccountDetails()
    {
        Console.WriteLine($"{Environment.NewLine}=== View Account Details ===");

        var accountNumber = GetValidatedInput("Enter Account Number", input =>
        {
            if (!_accountService.IsExistingAccount(input))
            {
                return (false, "Error: Account not found.");
            }

            return (true, string.Empty);
        });

        if (string.IsNullOrEmpty(accountNumber))
        {
            return;
        }

        var account = _accountService.GetAccount(accountNumber);
        Console.WriteLine(account.ToString());
    }

    private void TransferMoney()
    {
        Console.WriteLine($"{Environment.NewLine}=== Transfer Money ===");

        var sourceAccountNumber = GetValidatedInput("Enter Source Account Number", input =>
        {
            var acc = _accountService.GetAccount(input);
            if (acc == null)
            {
                return (false, "Error: Source account must exist.");
            }

            if (!acc.IsAccountActive)
            {
                return (false, "Error: Source account must be Active.");
            }

            return (true, string.Empty);
        });

        if (string.IsNullOrEmpty(sourceAccountNumber))
        {
            return;
        }

        var sourceAccount = _accountService.GetAccount(sourceAccountNumber);

        var destAccountNumber = GetValidatedInput("Enter Destination Account Number", input =>
        {
            if (input == sourceAccountNumber)
            {
                return (false, "Error: Source and destination accounts must be different.");
            }

            var account = _accountService.GetAccount(input);
            if (account == null)
            {
                return (false, "Error: Destination account must exist.");
            }

            if (!account.IsAccountActive)
            {
                return (false, "Error: Destination account must be Active.");
            }

            return (true, string.Empty);
        });

        if (string.IsNullOrEmpty(destAccountNumber))
        {
            return;
        }

        var destAccount = _accountService.GetAccount(destAccountNumber);

        var amountInput = GetValidatedInput("Enter Transfer Amount", input =>
        {
            if (!decimal.TryParse(input, out decimal val) || val <= 0)
            {
                return (false, "Error: Transfer amount > 0.");
            }

            if (val > sourceAccount.Balance)
            {
                return (false, "Error: Source account must have sufficient funds.");
            }

            if (sourceAccount.Balance - val < MinimumBalance)
            {
                return (false, $"Error: Minimum balance rule. Source account balance after transfer must be at least {MinimumBalance:C}.");
            }

            return (true, string.Empty);
        });

        if (string.IsNullOrEmpty(amountInput))
        {
            return;
        }

        decimal amount = decimal.Parse(amountInput);

        // Deduct amount from source account
        _accountService.UpdateBalance(sourceAccountNumber, sourceAccount.Balance - amount);
        // Add amount to destination account
        _accountService.UpdateBalance(destAccountNumber, destAccount.Balance + amount);

        // Record 2 transactions
        var nextId = _transactionService.GetNextTransactionId();

        // Transfer Out (source)
        _transactionService.AddTransaction(new Transaction
        {
            TransactionId = nextId,
            Type = TransactionType.Transfer,
            Amount = amount,
            TransactionDate = DateTime.Now,
            Description = $"Transfer Out to {destAccountNumber}",
            AccountNumber = sourceAccountNumber
        });

        // Transfer In (destination)
        _transactionService.AddTransaction(new Transaction
        {
            TransactionId = nextId + 1,
            Type = TransactionType.Transfer,
            Amount = amount,
            TransactionDate = DateTime.Now,
            Description = $"Transfer In from {sourceAccountNumber}",
            AccountNumber = destAccountNumber
        });

        PrintMessage($"Transfer successful! {amount:C} transferred from {sourceAccountNumber} to {destAccountNumber}.", ConsoleColor.Green);
        PrintMessage($"Source account ({sourceAccountNumber}) remaining balance: {sourceAccount.Balance:C}", ConsoleColor.Green);
    }

    private void WithdrawMoney()
    {
        Console.WriteLine($"{Environment.NewLine}=== Withdraw Money ===");

        var accountNumber = GetValidatedInput("Enter Account Number", input =>
        {
            var accountExist = _accountService.GetAccount(input);
            if (accountExist == null)
            {
                return (false, "Error: Account must exist.");
            }

            if (!accountExist.IsAccountActive)
            {
                return (false, "Error: Account status must be Active.");
            }

            return (true, string.Empty);
        });
        if (accountNumber == null)
        {
            return;
        }

        var account = _accountService.GetAccount(accountNumber);

        var amountInput = GetValidatedInput("Enter Withdrawal Amount", input =>
        {
            if (!decimal.TryParse(input, out decimal amountValue) || amountValue <= 0)
            {
                return (false, "Error: Amount > 0.");
            }

            if (amountValue > account.Balance)
            {
                return (false, "Error: Amount must not exceed balance.");
            }

            if (account.Balance - amountValue < MinimumBalance)
            {
                return (false, $"Error: Minimum balance rule. Balance after withdrawal must be at least {MinimumBalance:C}.");
            }

            return (true, string.Empty);
        });

        if (amountInput == null)
        {
            return;
        }

        decimal amount = decimal.Parse(amountInput);

        // Decrease balance
        _accountService.UpdateBalance(accountNumber, account.Balance - amount);

        // Record transaction
        var transaction = new Transaction
        {
            Type = TransactionType.Withdraw,
            Amount = amount,
            TransactionDate = DateTime.Now,
            Description = $"Withdrawal of {amount:C} from account {accountNumber}",
            AccountNumber = accountNumber
        };
        _transactionService.AddTransaction(transaction);

        // Show remaining balance
        PrintMessage($"Withdrawal successful! Remaining balance: {account.Balance:C}", ConsoleColor.Green);
    }

    private void DepositMoney()
    {
        Console.WriteLine($"{Environment.NewLine}=== Deposit Money ===");

        var accountNumber = GetValidatedInput("Enter Account Number", input =>
        {
            var accountExist = _accountService.GetAccount(input);
            if (accountExist == null)
            {
                return (false, "Error: Account must exist.");
            }

            if (accountExist.Status != AccountStatus.Active)
            {
                return (false, "Error: Account status must be Active.");
            }

            return (true, string.Empty);
        });

        if (accountNumber == null)
        {
            return;
        }

        var account = _accountService.GetAccount(accountNumber);

        var amountInput = GetValidatedInput("Enter Deposit Amount", input =>
        {
            if (!decimal.TryParse(input, out decimal val) || val <= 0)
            {
                return (false, "Error: Amount > 0.");
            }

            return (true, string.Empty);
        });
        if (amountInput == null)
        {
            return;
        }

        decimal amount = decimal.Parse(amountInput);

        // Increase account balance
        _accountService.UpdateBalance(accountNumber, account.Balance + amount);

        // Add a Transaction of type Deposit
        var transaction = new Transaction
        {
            Type = TransactionType.Deposit,
            Amount = amount,
            TransactionDate = DateTime.Now,
            Description = $"Deposit of {amount:C} into account {accountNumber}",
            AccountNumber = accountNumber
        };
        _transactionService.AddTransaction(transaction);

        // Display updated balance
        PrintMessage($"Deposit successful! Updated balance: {account.Balance:C}", ConsoleColor.Green);
    }

    private void CreateAccount()
    {
        Console.WriteLine($"{Environment.NewLine}=== Create New Account ===");

        var accountNumber = GetValidatedInput("Enter Account Number", input =>
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return (false, "Error: Account number cannot be empty.");
            }

            if (_accountService.IsExistingAccount(input))
            {
                return (false, "Error: Account number already exists. Must be unique.");
            }

            return (true, string.Empty);
        });

        if (string.IsNullOrEmpty(accountNumber))
        {
            return;
        }

        var ownerName = GetValidatedInput("Enter Owner Name", input =>
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return (false, "Error: Owner name cannot be empty.");
            }

            return (true, string.Empty);
        });

        if (string.IsNullOrEmpty(ownerName))
        {
            return;
        }

        var balanceInput = GetValidatedInput("Enter Initial Balance", input =>
        {
            if (!decimal.TryParse(input, out decimal val) || val < 0)
            {
                return (false, "Error: Initial balance must be a valid number and >= 0.");
            }

            if (val < MinimumBalance)
            {
                return (false, $"Error: Initial balance must be at least {MinimumBalance:C}.");
            }

            return (true, string.Empty);
        });

        if (string.IsNullOrEmpty(balanceInput))
        {
            return;
        }

        decimal initialBalance = decimal.Parse(balanceInput);

        var newAccount = new Account
        {
            AccountNumber = accountNumber,
            OwnerName = ownerName,
            Status = AccountStatus.Active,
            CreatedAt = DateTime.Now
        };

        newAccount.UpdateBalance(initialBalance);

        _accountService.AddAccount(newAccount);
        PrintMessage("Account created successfully!", ConsoleColor.Green);
    }

    private static void PrintOptions()
    {
        Console.WriteLine("=== Bank Account System ===");
        Console.WriteLine("""
            1. Create new account
            2. Deposit money
            3. Withdraw money
            4. Transfer money
            5. View account details
            6. View transaction history
            7. Freeze / Unfreeze account
            8. Exit
        """);
    }

    private string GetValidatedInput(string prompt, Func<string, (bool IsValid, string ErrorMessage)> validator)
    {
        while (true)
        {
            Console.Write($"{prompt} (or type 'quit' to exit): ");
            var input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var (isValid, errorMessage) = validator(input);
            if (isValid)
            {
                return input;
            }

            PrintMessage(errorMessage, ConsoleColor.Red);
        }
    }

    private void PrintMessage(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
