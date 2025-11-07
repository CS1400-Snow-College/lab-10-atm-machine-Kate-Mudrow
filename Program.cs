/*
Name: Kate Mudrow
Date: 
Lab: Lab 10 ATM
*/

Console.Clear();

string[] bankData = File.ReadAllLines("bank.txt");

for (int i = 1; i < bankData.Length; i++)
{
    string[] columns = bankData[i].Split(",");

    string username = columns[0];
    int pin = Convert.ToInt32(columns[1]);
    decimal currentBalance = Convert.ToDecimal(columns[2]);
}

