﻿namespace BankProject
{
    public class BankUser
    {
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public DateTime LockAccountDateTime { get; set; }

        public List<BankAccount> BankAccounts { get; set; }

        public BankUser(string user, string password, List<BankAccount> bankAccounts)
        {
            User = user;
            Password = password;
            BankAccounts = bankAccounts;
        }
    }

    public class BankAccount
    {
        //bank account needs a float(account value) and a string(account type)

        public string AccountType { get; set; } = string.Empty;
        public double Balance { get; set; }

        public BankAccount(string accountType, double balance)
        {
            AccountType = accountType;
            Balance = balance;
        }
    }
}
