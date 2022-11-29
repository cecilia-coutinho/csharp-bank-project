using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankProject
{
    public class BankAccount
    {
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public float Savings { get; set; }
        public float Salary { get; set; }

        public BankAccount(string user, string password, float savings, float salary)
        {
            User = user;
            Password = password;
            Salary = salary;
            Savings = savings;
        }
    }
}
