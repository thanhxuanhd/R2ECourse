using BankAccountSimulation.Models;
using System.Runtime.CompilerServices;
using System.Text.Json;

string GetSourceFilePath([CallerFilePath] string sourceFilePath = "") => sourceFilePath;
string sourceDirectory = Path.GetDirectoryName(GetSourceFilePath()) ?? string.Empty;

bool running = true;
const string accountFile = "account.json";
const string transactionFile = "transaction.json";
string folderPath = Path.Combine(sourceDirectory, "Data");
string accountFilePath = Path.Combine(folderPath, accountFile);
string transactionFilePath = Path.Combine(folderPath, transactionFile);
const decimal MinimumBalance = 100;

List<Account> accounts = [];
List<Transaction> transactions = [];

try
{
    CreateDataResource();
    LoadDataInMemory();

    while (running)
    {
        PrintOptions();
        HandleOptions();
    }
}
catch (Exception ex)
{
    PrintMessage($"An error occurred: {ex.Message}", ConsoleColor.Red);
}

void HandleOptions()
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
            running = false;
            break;

        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }
}

void FreezeUnfreezeAccount()
{
    Console.WriteLine($"{Environment.NewLine}=== Freeze / Unfreeze Account ===");

    var accountNumber = GetValidatedInput("Enter Account Number", input =>
    {
        if (!IsExistingAccount(input))
        {
            return (false, "Error: Account not found.");
        }

        return (true, string.Empty);
    });

    if (string.IsNullOrEmpty(accountNumber))
    {
        return;
    }

    var account = GetAccount(accountNumber);

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
                account.Status = AccountStatus.Frozen;
                PrintMessage("Account frozen successfully.", ConsoleColor.Green);
                SaveAccountToFile();
            }
            break;

        case "2":
            if (account.IsAccountActive)
            {
                PrintMessage("Error: Account is already Active.", ConsoleColor.Red);
            }
            else
            {
                account.Status = AccountStatus.Active;
                PrintMessage("Account unfrozen successfully.", ConsoleColor.Green);
                SaveAccountToFile();
            }
            break;

        default:
            PrintMessage("Invalid choice.", ConsoleColor.Red);
            break;
    }
}

void ViewTransactionHistory()
{
    Console.WriteLine($"{Environment.NewLine}=== View Transaction History ===");

    var accountNumber = GetValidatedInput("Enter Account Number", input =>
    {
        if (!IsExistingAccount(input))
        {
            return (false, "Error: Account not found.");
        }

        return (true, string.Empty);
    });

    if (string.IsNullOrEmpty(accountNumber))
    {
        return;
    }

    var account = GetAccount(accountNumber);

    Console.WriteLine("""
        Select Filter:
        1. All Transactions
        2. Deposits Only
        3. Withdrawals Only
        """);
    Console.Write("Choice: ");
    var filterChoice = Console.ReadLine();

    IEnumerable<Transaction> filteredTransactions = transactions.Where(t => t.AccountNumber == accountNumber);

    switch (filterChoice)
    {
        case "2":
            filteredTransactions = filteredTransactions.Where(t => t.Type == TransactionType.Deposit);
            break;

        case "3":
            filteredTransactions = filteredTransactions.Where(t => t.Type == TransactionType.Withdraw);
            break;
    }

    DisplayTransaction(filteredTransactions, accountNumber);
}

void DisplayTransaction(IEnumerable<Transaction> filteredTransactions, string accountNumber)
{
    Console.WriteLine($"{Environment.NewLine}--- Transaction History for {accountNumber} ---");
    Console.WriteLine($"{"Date",-20} | {"Type",-10} | {"Amount",-12} | {"Description"}");
    Console.WriteLine(new string('-', 80));

    if (transactions.Count == 0)
    {
        PrintMessage($"No transactions found with {accountNumber}.", ConsoleColor.Blue);
        return;
    }

    foreach (var t in filteredTransactions.OrderByDescending(t => t.TransactionDate))
    {
        Console.WriteLine(t.ToString());
    }

    Console.WriteLine(new string('-', 80));
}

void ViewAccountDetails()
{
    Console.WriteLine($"{Environment.NewLine}=== View Account Details ===");

    var accountNumber = GetValidatedInput("Enter Account Number", input =>
    {
        if (!IsExistingAccount(input))
        {
            return (false, "Error: Account not found.");
        }

        return (true, string.Empty);
    });

    if (string.IsNullOrEmpty(accountNumber))
    {
        return;
    }

    var account = GetAccount(accountNumber);
    Console.WriteLine(account.ToString());
}

void TransferMoney()
{
    Console.WriteLine($"{Environment.NewLine}=== Transfer Money ===");

    var sourceAccountNumber = GetValidatedInput("Enter Source Account Number", input =>
    {
        var acc = GetAccount(input);
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

    var sourceAccount = GetAccount(sourceAccountNumber);

    var destAccountNumber = GetValidatedInput("Enter Destination Account Number", input =>
    {
        if (input == sourceAccountNumber)
        {
            return (false, "Error: Source and destination accounts must be different.");
        }

        var account = GetAccount(input);
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

    var destAccount = GetAccount(destAccountNumber);

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
    sourceAccount.UpdateBalance(sourceAccount.Balance - amount);
    // Add amount to destination account
    destAccount.UpdateBalance(destAccount.Balance + amount);

    // Create 2 transactions
    var nextId = transactions.Count > 0 ? transactions.Max(t => t.TransactionId) + 1 : 1;

    // Transfer Out (source)
    transactions.Add(new Transaction
    {
        TransactionId = nextId,
        Type = TransactionType.Transfer,
        Amount = amount,
        TransactionDate = DateTime.Now,
        Description = $"Transfer Out to {destAccountNumber}",
        AccountNumber = sourceAccountNumber
    });

    // Transfer In (destination)
    transactions.Add(new Transaction
    {
        TransactionId = nextId + 1,
        Type = TransactionType.Transfer,
        Amount = amount,
        TransactionDate = DateTime.Now,
        Description = $"Transfer In from {sourceAccountNumber}",
        AccountNumber = destAccountNumber
    });

    // Save changes
    SaveAccountToFile();
    SaveTransactionToFile();

    PrintMessage($"Transfer successful! {amount:C} transferred from {sourceAccountNumber} to {destAccountNumber}.", ConsoleColor.Green);
    PrintMessage($"Source account ({sourceAccountNumber}) remaining balance: {sourceAccount.Balance:C}", ConsoleColor.Green);
}

void WithdrawMoney()
{
    Console.WriteLine($"{Environment.NewLine}=== Withdraw Money ===");

    var accountNumber = GetValidatedInput("Enter Account Number", input =>
    {
        var accountExist = GetAccount(input);
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

    var account = GetAccount(accountNumber);

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
    account.UpdateBalance(account.Balance - amount);

    // Record transaction
    var transaction = new Transaction
    {
        TransactionId = transactions.Count > 0 ? transactions.Max(t => t.TransactionId) + 1 : 1,
        Type = TransactionType.Withdraw,
        Amount = amount,
        TransactionDate = DateTime.Now,
        Description = $"Withdrawal of {amount:C} from account {accountNumber}",
        AccountNumber = accountNumber
    };
    transactions.Add(transaction);

    // Save changes
    SaveAccountToFile();
    SaveTransactionToFile();

    // Show remaining balance
    PrintMessage($"Withdrawal successful! Remaining balance: {account.Balance:C}", ConsoleColor.Green);
}

void DepositMoney()
{
    Console.WriteLine($"{Environment.NewLine}=== Deposit Money ===");

    var accountNumber = GetValidatedInput("Enter Account Number", input =>
    {
        var accountExist = GetAccount(input);
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

    var account = GetAccount(accountNumber);

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
    account.UpdateBalance(account.Balance + amount);

    // Add a Transaction of type Deposit
    var transaction = new Transaction
    {
        TransactionId = transactions.Count > 0 ? transactions.Max(t => t.TransactionId) + 1 : 1,
        Type = TransactionType.Deposit,
        Amount = amount,
        TransactionDate = DateTime.Now,
        Description = $"Deposit of {amount:C} into account {accountNumber}",
        AccountNumber = accountNumber
    };
    transactions.Add(transaction);

    // Save changes
    SaveAccountToFile();
    SaveTransactionToFile();

    // Display updated balance
    PrintMessage($"Deposit successful! Updated balance: {account.Balance:C}", ConsoleColor.Green);
}

void CreateAccount()
{
    Console.WriteLine($"{Environment.NewLine}=== Create New Account ===");

    var accountNumber = GetValidatedInput("Enter Account Number", input =>
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return (false, "Error: Account number cannot be empty.");
        }

        if (accounts.Any(a => a.AccountNumber == input))
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

    accounts.Add(newAccount);
    SaveAccountToFile();
    PrintMessage("Account created successfully!", ConsoleColor.Green);
}

void PrintOptions()
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

void CreateDataResource()
{
    if (!Directory.Exists(folderPath))
    {
        Directory.CreateDirectory(folderPath);
    }

    if (!File.Exists(accountFilePath))
    {
        File.Create(accountFilePath);
    }

    if (!File.Exists(transactionFilePath))
    {
        File.Create(transactionFilePath);
    }
}

void LoadDataInMemory()
{
    LoadAccount();

    LoadTransaction();
}

void SaveAccountToFile()
{
    WriteToFile(accountFilePath, accounts);
}

void SaveTransactionToFile()
{
    WriteToFile(transactionFilePath, transactions);
}

void WriteToFile<T>(string filePath, List<T> data)
{
    File.WriteAllText(filePath, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true, }));
}

void LoadAccount()
{
    if (File.Exists(accountFilePath))
    {
        accounts = LoadJsonFile<Account>(accountFilePath);
    }
}

void LoadTransaction()
{
    if (File.Exists(transactionFilePath))
    {
        transactions = LoadJsonFile<Transaction>(transactionFilePath);
    }
}

List<T> LoadJsonFile<T>(string filePath)
{
    var json = File.ReadAllText(filePath);
    if (!string.IsNullOrEmpty(json))
    {
        return JsonSerializer.Deserialize<List<T>>(json) ?? [];
    }

    return [];
}

Account GetAccount(string accountNumber)
{
    return accounts.Find(a => a.AccountNumber == accountNumber);
}

bool IsExistingAccount(string accountNumber)
{
    return accounts.FirstOrDefault(a => a.AccountNumber == accountNumber) is null;
}

string GetValidatedInput(string prompt, Func<string, (bool IsValid, string ErrorMessage)> validator)
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

void PrintMessage(string message, ConsoleColor color)
{
    Console.ForegroundColor = color;
    Console.WriteLine(message);
    Console.ResetColor();
}