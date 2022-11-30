using System.Dynamic;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Xml;
using static System.Console; //to simplify Console references

namespace BankProject
{
    internal class Program
    {
        static BankAccount[] bankAccounts = { new BankAccount("maria", "1234", 100, 0), new BankAccount("pedro", "1234", 0, 0), new BankAccount("kalle", "1234", 0, 0), new BankAccount("johan", "1234", 0, 0), new BankAccount("matias", "1234", 0, 0) };

        //to get bank account
        static int GetAccountByUser(string? user)
        {
            for (int i = 0; i < bankAccounts.Length; i++)
            {
                if (bankAccounts[i].User == user)
                {
                    return i;
                }
            }

            return -1; //false
        }


        static void Main(string[] args)
        {
            RunLogin();
        }

        static void RunLogin()
        {
            Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n\t============WELCOME TO BANK XZ!============\n");
            Console.ResetColor();
            Console.Write("\n\tUser: \n\t");
            string? user = Console.ReadLine()?.ToLower();
            Console.Write("\n\tPassword: \n\t");
            string? password = Console.ReadLine();

            int accountIndex = GetAccountByUser(user); //get user index
            bool accountFound = accountIndex != -1;

            if (accountFound)
            {
                if (bankAccounts[accountIndex].Password == password)
                {
                    bool runLoginMenu = true;
                    while (runLoginMenu)
                    {
                        Clear();
                        string welcome1 = "\n\t============Welcome to your account============\n";

                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine(welcome1.ToUpper());
                        Console.ResetColor();
                        Console.WriteLine("\tPlease select one of the options below:");
                        Console.WriteLine("\n\t1. View Accounts and Balance\n" +
                            "\n\t2. Make a Deposit\n" +
                            "\n\t3. Transfer Between Accounts\n" +
                            "\n\t4. Withdraw Money\n" +
                            "\n\t5. Log Out\n");
                        Console.Write("\t Select menu: ");

                        int menuChoice;
                        int.TryParse(Console.ReadLine(), out menuChoice);

                        switch (menuChoice)
                        {
                            case 1:
                                ViewAccountsAndBalance(accountIndex);
                                break;
                            case 2:
                                MakeDeposit(accountIndex);
                                break;
                            case 3:
                                TransferBtwAccounts(accountIndex);
                                break;
                            case 4:
                                WithdrawMoney(accountIndex);
                                break;
                            case 5:
                                Console.WriteLine("\n\tThanks for your visit!");
                                Thread.Sleep(1000);
                                RunLogin();
                                break;
                            default:
                                Console.Clear();
                                InvalidOption();
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n\tPlease, choose 1-5 from the menu\n");
                                Console.ResetColor();
                                GoBackMenuOptions();
                                break;
                        }
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n\tInvalid password!\n".ToUpper());
                    Console.ResetColor();
                    GoBackMenuOptions();
                    RunLogin();
                }

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tInvalid username or password!\n".ToUpper());
                Console.ResetColor();
                GoBackMenuOptions();
                RunLogin();
            }
        }

        static void ViewAccountsAndBalance(int accountIndex)
        {
            Clear();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"\n\tHi {bankAccounts[accountIndex].User.ToUpper()}!\n" +
                $"\n\tSavings: {bankAccounts[accountIndex].Savings.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}\n" +
                $"\n\tSalary: {bankAccounts[accountIndex].Salary.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}\n");
            Console.ResetColor();
            GoBackMenuOptions();
        }

        static void MakeDeposit(int accountIndex)
        {
            Clear();
            Console.Write("\n\tHow much $$ do you want to deposit? ");
            string? deposit = Console.ReadLine();
            float depositAmount;
            float.TryParse(deposit, out depositAmount);

            Console.WriteLine($"\n\tselect account:\n" +
                $"\n\t1. Savings\n" +
                $"\n\t2. Salary\n" +
                $"\n\t3. Go Back\n");
            Console.Write("\t");

            int userChoiceAccount;
            int.TryParse(Console.ReadLine(), out userChoiceAccount);

            if (userChoiceAccount == 1)
            {
                if (depositAmount <= 0)
                {
                    NegativeAmmount();
                }
                else
                {
                    bankAccounts[accountIndex].Savings += depositAmount;
                    NewBalance(accountIndex);
                    GoBackMenuOptions();
                }
            }
            else if (userChoiceAccount == 2)
            {
                if (depositAmount <= 0)
                {
                    NegativeAmmount();
                }
                else
                {
                    bankAccounts[accountIndex].Salary += depositAmount;
                    NewBalance(accountIndex);
                    GoBackMenuOptions();
                }

            }
            else if (userChoiceAccount == 3)
            {
                Write("\n\tAre you sure do you want to come back?(Type YES or NO); ");
                string? restart = Console.ReadLine()?.ToUpper();

                if (restart == "NO")
                {
                    MakeDeposit(accountIndex);
                }
                else if (restart == "YES")
                {
                    GoBackMenuOptions();
                }
                else
                {
                    InvalidOption();
                    GoBackMenuOptions();
                }
            }
            else
            {
                InvalidOption();
                GoBackMenuOptions();
            }
        }

        static void WithdrawMoney(int accountIndex)
        {
            Clear();
            Console.Write("How much $$ do you want to withdraw? ");
            string? withdraw = Console.ReadLine();
            float withdrawAmount;
            float.TryParse(withdraw, out withdrawAmount);

            Console.WriteLine($"\n\tselect account:\n" +
                $"\n\t1. Savings\n" +
                $"\n\t2. Salary\n");
            Console.Write("\t");

            int userChoiceAccount;
            int.TryParse(Console.ReadLine(), out userChoiceAccount);

            if (userChoiceAccount == 1)
            {
                if (withdrawAmount <= 0)
                {
                    NegativeAmmount();
                }
                else
                {
                    if (withdrawAmount <= bankAccounts[accountIndex].Savings)
                    {
                        bankAccounts[accountIndex].Savings -= withdrawAmount;
                        NewBalance(accountIndex);
                    }
                    else
                    {
                        Console.WriteLine("\n\tYou don't have enough money");
                    }
                }
            }
            else if (userChoiceAccount == 2)
            {
                if (withdrawAmount <= 0)
                {
                    NegativeAmmount();
                }
                else
                {
                    if (withdrawAmount <= bankAccounts[accountIndex].Salary)
                    {
                        bankAccounts[accountIndex].Salary -= withdrawAmount;
                        NewBalance(accountIndex);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n\tYou don't have enough money");
                        ResetColor();
                    }
                }
            }
            else
            {
                InvalidOption();
            }
            GoBackMenuOptions();
        }

        static void TransferBtwAccounts(int accountIndex)
        {
            Clear();
            Console.Write("\n\tTo whom do you want to transfer?: ");
            string? transferTo = Console.ReadLine();

            int targetAccountIndex = GetAccountByUser(transferTo);

            bool accountTargetFound = targetAccountIndex != -1;
            if (accountTargetFound)
            {
                Clear();
                Console.WriteLine($"\n\tselect type of account to transfer from:\n" +
                $"\n\t1. Savings\n" +
                $"\n\t2. Salary\n");
                Console.Write("\t");

                int srcTypeAccount;
                int.TryParse(Console.ReadLine(), out srcTypeAccount);

                if (srcTypeAccount == 1)
                {
                    Clear();
                    Console.Write("\n\tHow much $$ do you want to transfer? ");
                    string? transfer = Console.ReadLine();
                    float transferAmount;
                    float.TryParse(transfer, out transferAmount);

                    if (transferAmount <= 0)
                    {
                        NegativeAmmount();
                    }
                    else if (bankAccounts[accountIndex].Savings < transferAmount)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n\tERROR! Not allowed. You don't have enough money");
                        ResetColor();
                    }
                    else
                    {
                        Clear();
                        Console.WriteLine($"\n\tselect type of account to transfer to:\n" +
                        $"\n\t1. Savings\n" +
                        $"\n\t2. Salary\n");
                        Console.Write("\t");

                        int targetTypeAccount;
                        int.TryParse(Console.ReadLine(), out targetTypeAccount);

                        if (targetTypeAccount == 1)
                        {
                            bankAccounts[accountIndex].Savings -= transferAmount;
                            bankAccounts[targetAccountIndex].Savings += transferAmount;
                        }
                        else if (targetTypeAccount == 2)
                        {
                            bankAccounts[accountIndex].Savings -= transferAmount;
                            bankAccounts[targetAccountIndex].Salary += transferAmount;

                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Transaction not allowed!");
                            ResetColor();
                            InvalidOption();
                        }
                    }
                }
                else if (srcTypeAccount == 2)
                {
                    Clear();
                    Console.Write("\n\tHow much $$ do you want to transfer? ");
                    string? transfer = Console.ReadLine();
                    float transferAmount;
                    float.TryParse(transfer, out transferAmount);

                    if (transferAmount <= 0)
                    {
                        NegativeAmmount();
                    }
                    else if (bankAccounts[accountIndex].Salary < transferAmount)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n\tERROR! Not allowed. You don't have enough money");
                        ResetColor();
                    }
                    else
                    {
                        Clear();
                        Console.WriteLine($"\n\tselect type of account to transfer to:\n" +
                        $"\n\t1. Savings\n" +
                        $"\n\t2. Salary\n");
                        Console.Write("\t");

                        int targetTypeAccount;
                        int.TryParse(Console.ReadLine(), out targetTypeAccount);

                        if (targetTypeAccount == 1)
                        {
                            bankAccounts[accountIndex].Salary -= transferAmount;
                            bankAccounts[targetAccountIndex].Savings += transferAmount;
                        }
                        else if (targetTypeAccount == 2)
                        {
                            bankAccounts[accountIndex].Salary -= transferAmount;
                            bankAccounts[targetAccountIndex].Salary += transferAmount;
                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Transaction not allowed!");
                            ResetColor();
                            InvalidOption();
                        }
                    }
                }
                else
                {
                    InvalidOption();
                }
                //Console.WriteLine($"{bankAccounts[accountIndex].User}, New Balance Savings: {bankAccounts[accountIndex].Savings.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}");
                //Console.WriteLine($"{bankAccounts[accountIndex].User}, New Balance Salary: {bankAccounts[accountIndex].Salary.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}");
                //Console.WriteLine($"{bankAccounts[targetAccountIndex].User}, New Balance Savings: {bankAccounts[targetAccountIndex].Savings.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}");
                //Console.WriteLine($"{bankAccounts[targetAccountIndex].User}, New Balance Salary: {bankAccounts[targetAccountIndex].Salary.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}");
                NewBalance(accountIndex);
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tInvalid User".ToUpper());
                ResetColor();
                InvalidOption();
            }
            GoBackMenuOptions();
        }
        static void GoBackMenuOptions()
        {
            Console.WriteLine("\n\tPress ENTER to go back to the menu.\n");
            Console.ReadLine();
        }

        static void NewBalance(int accountIndex)
        {
            Clear();
            Console.WriteLine("\n=============Balance Updated=============");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"\n\tHi {bankAccounts[accountIndex].User.ToUpper()},\n \n\tYour new balance is:\n\tSavings: {bankAccounts[accountIndex].Savings.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))} \n\tSalary: {bankAccounts[accountIndex].Salary.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}");
            Console.ResetColor();
        }
        static void InvalidOption()
        {
            ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\tERROR: Invalid Option!".ToUpper());
            ResetColor();
            ReadLine();
        }

        static void NegativeAmmount()
        {
            ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\tERROR! Transaction not allowed." +
                "\n\tThe ammount needs to be higher than 0,00".ToUpper());
            ResetColor();
            ReadLine();
        }
    }
}