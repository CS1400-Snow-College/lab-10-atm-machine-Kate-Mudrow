/*
Name: Kate Mudrow
Date: 
Lab: Lab 10 ATM
*/

using System.Diagnostics;
Console.Clear();

(List<(string username, int pin, decimal balance)> accounts, 
List<Queue<string>> transactionHistories) = LoadBankCustomers();


int attempts = 0;
bool validLogIn = false;
string currentUser = "";
int selection;

do
{
    Console.WriteLine("===== Welcome to the Bank=====");
    Console.WriteLine("Are you a new or current customer?");
    Console.WriteLine(@"
1) Current Customer
2) New Customer");

    string newOrReturning = Console.ReadLine();
    if (int.TryParse(newOrReturning, out selection))
    {
        switch (selection)
        {
            case 1:
                break;
            case 2:
                newCustomer(accounts, transactionHistories);
                break;
            default:
                Console.WriteLine("Invalid choice. Press any key to try again...");
                Console.ReadKey(true);
                break;
        }

    }
} while (selection != 1);


    do
        {
        Console.Clear();
        Console.WriteLine("=== Current Customer ===");
        Console.Write("Please Enter You USERNAME: ");
        string usernameInput = Console.ReadLine().ToLower();

        Console.Write("Please Enter your PIN number: ");
        string pinInput = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
            else if (char.IsDigit(key.KeyChar))
            {
                pinInput += key.KeyChar;
                Console.Write("*");
            }

        } while (true);


        if (ValidateUser(accounts, usernameInput, pinInput, out currentUser))
        {
            validLogIn = true;
            Console.Clear();
            Console.WriteLine("Login Successful");

            successfulLogIn(accounts, transactionHistories, currentUser);

            saveBankCustomers(accounts, transactionHistories);

            break;
        }
        else
        {
            attempts++;
        }


    } while (attempts < 3 && !validLogIn);

    if (!validLogIn)
{
         Console.WriteLine();
        Console.WriteLine("Too many failed attempts. Access denied.");
        return;
    }



    static void successfulLogIn(List<(string username, int pin, decimal balance)> accounts, List<Queue<string>> transactionHistories, string username)
    {
        int choice;

        do
        {
            string bankMenu = @$"
        ===Welcome {username}===
Please select what action you would like to do: 

1) Check Balance
2) Withdraw
3) Deposit
4) Display last 5 transactions
5) Quick Withdraw $40
6) Quick Withdraw $100
7) End current session
        ";
            Console.WriteLine(bankMenu);

            string userChoice = Console.ReadLine();

            if (int.TryParse(userChoice, out choice))
            {
                switch (choice)
                {
                    case 1:
                        checkBalance(accounts, username);
                        break;
                    case 2:
                        withdrawMoney(accounts, transactionHistories, username);
                        break;
                    case 3:
                        depositMoney(accounts, transactionHistories, username);
                        break;
                    case 4:
                        displayTransactions(accounts, transactionHistories, username);
                        break;
                    case 5:
                        quickWithdraw(accounts, transactionHistories, username, 40);
                        break;
                    case 6:
                        quickWithdraw(accounts, transactionHistories, username, 100);
                        break;
                    case 7:
                        Console.Clear();
                        Console.WriteLine("Ending session. Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid Option");
                        break;
                }
            }
        } while (choice != 7);
    }



    // Methods

static (List<(string username, int pin, decimal balance)> accounts, List<Queue<string>> transactionHistories) LoadBankCustomers()
{
        List<(string username, int pin, decimal balance)> accounts = new List<(string username, int pin, decimal balance)>();
        List<Queue<string>> transactionHistories = new();

        string[] bankData = File.ReadAllLines("bank.txt");

        for (int i = 1; i < bankData.Length; i++)
        {
            string[] columns = bankData[i].Split(",");

            string username = columns[0];
            int pin = Convert.ToInt32(columns[1]);
            decimal currentBalance = Convert.ToDecimal(columns[2]);

            accounts.Add((username, pin, currentBalance));
            transactionHistories.Add(new Queue<string>());

        }
        return (accounts, transactionHistories);
    }



    static bool ValidateUser(List<(string username, int pin, decimal balance)> accounts, string usernameInput, string pinInput, out string currentUser)
    {
        currentUser = "";

        int pinInputNumber;
        if (!int.TryParse(pinInput, out pinInputNumber))
        {
            Console.WriteLine("PIN must be a number.");
            return false;
        }

        for (int i = 0; i < accounts.Count; i++)
        {
            if (accounts[i].username == usernameInput && accounts[i].pin == pinInputNumber)
            {
                Console.Clear();
                Console.WriteLine("Login Successful");
                currentUser = accounts[i].username;
                return true;
            }
        }

        Console.WriteLine("Invalid username or PIN.");
        return false;
    }



    static void checkBalance(List<(string username, int pin, decimal balance)> accounts, string username)
    {
        foreach ((string user, int pin, decimal balance) in accounts)
        {
            if (user == username)
            {
                Console.Clear();
                Console.WriteLine($"Your current balance is: ${balance:F2}");
                Console.WriteLine();
                Console.WriteLine("Press Any Key to return to menu");
                Console.ReadKey(true);
                Console.Clear();
                break;
            }
        }
    }


    static void withdrawMoney(List<(string username, int pin, decimal balance)> accounts, List<Queue<string>> transactionHistories, string username)
    {
        Console.Clear();
        Console.Write("Enter amount to withdraw: $");
        string input = Console.ReadLine();

        decimal amount;
        if (!decimal.TryParse(input, out amount))
        {
            Console.Clear();
            Console.WriteLine("Invalid amount.");
            Console.WriteLine();
            Console.WriteLine("Press Any Key to return to menu");
            Console.ReadKey(true);
            Console.Clear();
            return;
        }

        if (amount <= 0)
        {
            Console.Clear();
            Console.WriteLine("Amount must be greater than zero.");
            Console.WriteLine();
            Console.WriteLine("Press Any Key to return to menu");
            Console.ReadKey(true);
            Console.Clear();
            return;
        }

        for (int i = 0; i < accounts.Count; i++)
        {
            if (accounts[i].username == username)
            {
                if (accounts[i].balance >= amount)
                {
                    accounts[i] = (accounts[i].username, accounts[i].pin, accounts[i].balance - amount);
                    AddTransaction(transactionHistories[i], $"Withdraw: ${amount:F2}");

                    Console.WriteLine($"Withdrawal successful. New balance: ${accounts[i].balance:F2}");
                    Console.WriteLine();
                    Console.WriteLine("Press Any Key to return to menu");
                    Console.ReadKey(true);
                    Console.Clear();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Insufficient funds.");
                    Console.WriteLine();
                    Console.WriteLine("Press Any Key to return to menu");
                    Console.ReadKey(true);
                    Console.Clear();
                }
                return;
            }
        }

    }


    static void depositMoney(List<(string username, int pin, decimal balance)> accounts, List<Queue<string>> transactionHistories, string username)
    {
        Console.Clear();
        Console.Write("Enter amount to deposit: $");
        string input = Console.ReadLine();

        decimal amount;
        if (!decimal.TryParse(input, out amount))
        {
            Console.WriteLine("Invalid amount.");
            return;
        }

        if (amount <= 0)
        {
            Console.Clear();
            Console.WriteLine("Amount must be greater than zero.");
            Console.WriteLine();
            Console.WriteLine("Press Any Key to return to menu");
            Console.ReadKey(true);
            Console.Clear();
            return;
        }

        for (int i = 0; i < accounts.Count; i++)
        {
            if (accounts[i].username == username)
            {

                accounts[i] = (accounts[i].username, accounts[i].pin, accounts[i].balance + amount);
                Console.WriteLine($"Deposit successful. New balance: ${accounts[i].balance:F2}");
                AddTransaction(transactionHistories[i], $"Deposit: ${amount:F2}");
                Console.WriteLine();
                Console.WriteLine("Press Any Key to return to menu");
                Console.ReadKey(true);
                Console.Clear();
                return;
            }
        }

    }

    static void quickWithdraw(List<(string username, int pin, decimal balance)> accounts, List<Queue<string>> transactionHistories, string username, decimal amount)
{
    Console.Clear();

    for (int i = 0; i < accounts.Count; i++)
    {
        if (accounts[i].username == username)
        {
            if (accounts[i].balance >= amount)
            {
                accounts[i] = (accounts[i].username, accounts[i].pin, accounts[i].balance - amount);
                AddTransaction(transactionHistories[i], $"Withdraw: ${amount:F2}");
                Console.WriteLine($" Quick withdraw of ${amount} successful. New balance: ${accounts[i].balance:F2}");
                Console.WriteLine();
                Console.WriteLine("Press Any Key to return to menu");
                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Insufficient funds for quick withdraw.");
                Console.WriteLine();
                Console.WriteLine("Press Any Key to return to menu");
                Console.ReadKey(true);
                Console.Clear();
            }
            return;
        }
    }

}
    
    static void AddTransaction(Queue<string> history, string transaction)
{
    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    string transactionWithTime = $"[{timestamp}] {transaction}";

        if (history.Count >= 5)
        {
            history.Dequeue();
        }
        history.Enqueue(transactionWithTime);
    }

    static void displayTransactions(List<(string username, int pin, decimal balance)> accounts, List<Queue<string>> transactionHistories, string username)
    {
        for (int i = 0; i < accounts.Count; i++)
        {
            if (accounts[i].username == username)
            {
                Console.Clear();
                Console.WriteLine("Last 5 Transactions:");

                if (transactionHistories[i].Count == 0)
                {
                    Console.WriteLine("No transactions yet.");
                }
                else
                {
                    foreach (string transaction in transactionHistories[i])
                    {
                        Console.WriteLine(transaction);
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to return to menu");
                Console.ReadKey(true);
                Console.Clear();
                return;
            }
        }
    }

static void saveBankCustomers(List<(string username, int pin, decimal balance)> accounts, List<Queue<string>> transactionHistories)
{
    string[] accountLines = new string[accounts.Count + 1];
    accountLines[0] = "Username,Pin,Balance";

    for (int i = 0; i < accounts.Count; i++)
    {
        accountLines[i + 1] = $"{accounts[i].username},{accounts[i].pin},{accounts[i].balance:F2}";
    }
    File.WriteAllLines("bank.txt", accountLines);

    string[] transactionLines = new string[accounts.Count];
    for (int i = 0; i < accounts.Count; i++)
    {
        transactionLines[i] = string.Join("|", transactionHistories[i]);
    }
    File.WriteAllLines("transactions.txt", transactionLines);
}


static void newCustomer(List<(string username, int pin, decimal balance)> accounts, List<Queue<string>> transactionHistories)
{
    string newUsername = "";
    int newUserPin = 0;
    decimal newDeposit = 0;

    Console.Clear();
    Console.WriteLine("=== Create New Customer ===");


    bool validUsername = false;
    do
    {
        Console.Write("Enter new username (must be all lowercase): ");
        newUsername = Console.ReadLine().Trim().ToLower();

        if (string.IsNullOrEmpty(newUsername))
        {
            Console.WriteLine("Username cannot be empty.");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            Console.Clear();
            continue;
        }

        bool usernameExists = accounts.Any(a => a.username.Equals(newUsername, StringComparison.OrdinalIgnoreCase));
        if (usernameExists)
        {
            Console.WriteLine($"Username '{newUsername}' already exists. Try again.");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            Console.Clear();
            continue;
        }

        validUsername = true;
    } while (!validUsername);

    Console.WriteLine("Username created successfully!");
    Console.WriteLine();
    Console.WriteLine("Press any key to continue...");
    Console.ReadKey(true);
    Console.Clear();


    bool validPin = false;
    do
    {
        Console.Write("Enter a 5-digit PIN: ");
        string newPin = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }
            else if (char.IsDigit(key.KeyChar) && newPin.Length < 5)
            {
                newPin += key.KeyChar;
                Console.Write("*");
            }

        } while (true);

        Console.WriteLine();

        if (newPin.Length != 5 || !int.TryParse(newPin, out newUserPin))
        {
            Console.WriteLine("Invalid PIN. Must be exactly 5 digits.");
            Console.WriteLine();
            Console.WriteLine("Press any key to try again:");
            Console.ReadKey(true);
            Console.Clear();
            continue;
        }

        validPin = true;
    } while (!validPin);


    bool validDeposit = false;
    do
    {
        Console.Write("Enter initial deposit (must be at least $50): $");
        string depositInput = Console.ReadLine().Trim();

        if (!decimal.TryParse(depositInput, out newDeposit) || newDeposit < 50)
        {
            Console.WriteLine("Invalid deposit. Must be $50 or greater.");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue:");
            Console.ReadKey(true);
            Console.Clear();
            continue;
        }

        validDeposit = true;
        Console.Clear();

    } while (!validDeposit);
    
    accounts.Add((newUsername, newUserPin, newDeposit));


    Queue<string> newHistory = new Queue<string>();
    AddTransaction(newHistory, $"Initial deposit: ${newDeposit:F2}");
    transactionHistories.Add(newHistory);

    saveBankCustomers(accounts, transactionHistories);


    Console.Clear();
    Console.WriteLine($"New customer '{newUsername}' created successfully!");
    Console.WriteLine($"Initial deposit of ${newDeposit:F2} recorded.");
    Console.WriteLine();
    Console.WriteLine("Press any key to return to the welcome screen:");
    Console.ReadKey(true);
    Console.Clear();
}
    
