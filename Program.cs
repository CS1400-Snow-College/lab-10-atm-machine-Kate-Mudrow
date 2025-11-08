/*
Name: Kate Mudrow
Date: 
Lab: Lab 10 ATM
*/

Console.Clear();

List<(string username, int pin, decimal balance)> accounts = new();
List<List<string>> transactionHistories = new();

string[] bankData = File.ReadAllLines("bank.txt");

for (int i = 1; i < bankData.Length; i++)
{
    string[] columns = bankData[i].Split(",");

    string username = columns[0];
    int pin = Convert.ToInt32(columns[1]);
    decimal currentBalance = Convert.ToDecimal(columns[2]);

    accounts.Add((username, pin, currentBalance));
    transactionHistories.Add(new List<string>());
}

int attempts = 0;
bool validLogIn = false;
string currentUser = "";

Console.WriteLine("===== Welcome to the Bank=====");

do
{
    Console.Write("Please Enter You USERNAME: ");
    string usernameInput = Console.ReadLine().ToLower();

    Console.Write("Please Enter your PIN number: ");
    string pinInput = Console.ReadLine();


    if (int.TryParse(pinInput, out int pinInputNumber))
    {
        foreach (var account in accounts)
        {
            if (account.username == usernameInput && account.pin == pinInputNumber)
            {
                Console.Clear();
                Console.WriteLine("Login Successful");
                validLogIn = true;
                currentUser = account.username;
                break;
            }
        }
        if (!validLogIn)
        {
            Console.WriteLine("Invalid username or PIN.");
            attempts++;
        }
    }
    else
    {
        Console.WriteLine("PIN must be a number.");
        attempts++;
    }
    
} while (attempts < 3 && !validLogIn);

 if (!validLogIn)
    {
    Console.WriteLine("Too many failed attempts. Access denied.");
    return;
    }

successfulLogIn(accounts, transactionHistories, currentUser);


static void successfulLogIn (List<(string username, int pin, decimal balance)> accounts, List<List<string>> transactionHistories, string username)
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


static void withdrawMoney (List<(string username, int pin, decimal balance)> accounts, List<List<string>> transactionHistories,string username)
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
                transactionHistories[i].Add($"Withdraw: ${amount:F2}");

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


static void depositMoney (List<(string username, int pin, decimal balance)> accounts, List<List<string>> transactionHistories,string username)
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
            transactionHistories[i].Add($"Deposit: ${amount:F2}");
            Console.WriteLine();
            Console.WriteLine("Press Any Key to return to menu");
            Console.ReadKey(true);
            Console.Clear();
            return;
        }
    }

}

static void quickWithdraw (List<(string username, int pin, decimal balance)> accounts, List<List<string>> transactionHistories,string username,decimal amount)
{
    Console.Clear();

    for (int i = 0; i < accounts.Count; i++)
    {
        if (accounts[i].username == username)
        {
            if (accounts[i].balance >= amount)
            {
                accounts[i] = (accounts[i].username, accounts[i].pin, accounts[i].balance - amount);
                transactionHistories[i].Add($"Quick Withdraw: ${amount:F2}");
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

static void displayTransactions(List<(string username, int pin, decimal balance)> accounts,List<List<string>> transactionHistories,string username)
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
                for (int j = 0; j < transactionHistories[i].Count; j++)
                    Console.WriteLine(transactionHistories[i][j]);
            }
                
                Console.WriteLine();
                Console.WriteLine("Press any key to return to menu");
                Console.ReadKey(true);
                Console.Clear();
                return;
            }
        }
    }

