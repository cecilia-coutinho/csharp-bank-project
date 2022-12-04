using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankProject
{
    public class BankUser
    {
        public int Id = 0;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public List<BankAccount> BankAccounts { get; set; }

        public BankUser(int id, string user, string password, List<BankAccount> bankAccounts)
        {
            Id = id;
            Id++;
            User = user;
            Password = password;
            BankAccounts = bankAccounts;
        }
    }

    public class BankAccount
    {
        //bank account needs a float(account value) and a string(account type)

        public string AccountType { get; set; } = string.Empty;
        public float Balance { get; set; }

        public BankAccount(string accountType, float balance)
        {
            AccountType = accountType;
            Balance = balance;
        }
    }
}
