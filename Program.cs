/*
Name: Kate Mudrow
Date: 
Lab: Lab 10 ATM
*/

Console.Clear();

List<(string username, int pin, decimal balance)> accounts = new ();

string[] bankData = File.ReadAllLines("bank.txt");

for (int i = 1; i < bankData.Length; i++)
{
    string[] columns = bankData[i].Split(",");

    string username = columns[0];
    int pin = Convert.ToInt32(columns[1]);
    decimal currentBalance = Convert.ToDecimal(columns[2]);

    accounts.Add((username, pin, currentBalance));
}

int attempts = 0;
bool validLogIn = false;

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
                Console.WriteLine("Login Successful");
                validLogIn = true;
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
    }


