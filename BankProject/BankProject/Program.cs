using System.Diagnostics.Metrics;
using System.Dynamic;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Xml;
using static System.Console; //to simplify Console references

namespace BankProject
{
    internal class Program
    {
        static BankUser[] bankUsers = {
            new BankUser("maria", "1234", new List<BankAccount>{new BankAccount("Savings", 1000), new BankAccount("Salary", 100) }),
            new BankUser("pedro", "1234", new List<BankAccount>{new BankAccount("Savings", 100) }),
            new BankUser("kalle", "1234", new List<BankAccount>{new BankAccount("Savings", 100) }),
            new BankUser("johan", "1234", new List < BankAccount >{new BankAccount("Salary", 0) }),
            new BankUser("matias", "1234", new List < BankAccount >{new BankAccount("Savings", 0), new BankAccount("Salary", 0) }) };

        //to get index of array of each user
        static int GetBankUserIndexByUserName(string? user)
        {
            for (int i = 0; i < bankUsers.Length; i++)
            {
                if (bankUsers[i].User == user)
                {
                    return i;
                }
            }

            return -1; //false
        }


        static void Main(string[] args)
        {
            RunSystem();
        }

        static void RunSystem()
        {
            Clear();
            PrintWelcome();
            Console.Write("\n\tUser: \n\t");
            string? user = Console.ReadLine()?.ToLower();

            int userIndex = GetBankUserIndexByUserName(user); //get user index
            bool accountFound = userIndex != -1;

            if (accountFound)
            {
                CheckPassAndRunAccount(user, userIndex);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tInvalid username\n".ToUpper());
                Console.ResetColor();
                Console.WriteLine($"\n\tPress ENTER to try again.\n");
                Console.ReadLine();
                RunSystem();
            }
        }

        static void PrintWelcome()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n\t============WELCOME TO BANK XZ!============\n");
            Console.ResetColor();
        }

        static void CheckPassAndRunAccount(string? user, int userIndex)
        {
            int maxInvalidPassAttempts = 3;
            int countPassAttempts;

            for (countPassAttempts = 1; countPassAttempts <= maxInvalidPassAttempts; countPassAttempts++)
            {
                Clear();
                PrintWelcome();
                Console.Write("\n\tPassword: \n\t");
                string? password = Console.ReadLine();

                if (bankUsers[userIndex].Password == password)
                {
                    RunMenu(user, password, userIndex);
                    break;
                }
                else
                {
                    //max attempts failed password

                    if (countPassAttempts < maxInvalidPassAttempts)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n\tInvalid password!\n".ToUpper());
                        Console.ResetColor();
                        Console.WriteLine($"\n\tFailed Attempts: {countPassAttempts}" +
                            $"\n\tPress ENTER to try again.\n");
                        Console.ReadLine();
                    }
                    else
                    {
                        //Block new login attempt after 3 minutes
                        CounterTimeFailedAttempt(countPassAttempts);
                    }
                }
            }
        }

        static void CounterTimeFailedAttempt(int countPassAttempts)
        {
            int secondsCounter = 180;
            for (int i = secondsCounter - 1; i <= secondsCounter; i--)
            {
                Thread.Sleep(1000);
                if (i <= 0)
                {
                    RunSystem();
                    break;
                }
                else
                {
                    Clear();
                    Console.WriteLine($"\n\tFailed Attempts: {countPassAttempts}");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n\tMax failed attempts.\n".ToUpper());
                    Console.ResetColor();
                    Console.WriteLine($"\n\tPlease, wait! The system will automatically restart after {ConvertSecondsIntToTimeFormat(i)}.\n");
                }
            }
        }

        static string ConvertSecondsIntToTimeFormat(int secs)
        {
            TimeSpan t = TimeSpan.FromSeconds(secs);
            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                (int)t.TotalHours,
                t.Minutes,
                t.Seconds);
        }


        static void RunMenu(string? user, string? pass, int userIndex)
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
                    "\n\t5. Create New Account\n" +
                    "\n\t6. Log Out\n");
                Console.Write("\t Select menu: ");

                int menuChoice;
                int.TryParse(Console.ReadLine(), out menuChoice);

                switch (menuChoice)
                {
                    case 1:
                        ViewAccountsAndBalance(userIndex);
                        GoBackMenuOptions();
                        break;
                    case 2:
                        MakeDeposit(userIndex);
                        GoBackMenuOptions();
                        break;
                    case 3:
                        TransferBtwAccounts(userIndex);
                        GoBackMenuOptions();
                        break;
                    case 4:
                        WithdrawMoney(userIndex);
                        GoBackMenuOptions();
                        break;
                    case 5:
                        CreateNewAccount(userIndex);
                        GoBackMenuOptions();
                        break;
                    case 6:
                        Console.WriteLine("\n\tThanks for your visit!");
                        Thread.Sleep(1000);
                        RunSystem();
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

        static void ViewAccountsAndBalance(int userIndex)
        {
            Clear();
            Console.WriteLine("\n=============BALANCE=============");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"\n\tHi {bankUsers[userIndex].User.ToUpper()}!\n");

            //to get user and bank account
            for (int i = 0; i < bankUsers[userIndex].BankAccounts.Count; i++)
            {
                //show balance accounts and types
                var accountBalance = bankUsers[userIndex].BankAccounts[i].Balance;
                var accountType = bankUsers[userIndex].BankAccounts[i].AccountType;

                Console.WriteLine($"\n\t{accountType}: {accountBalance.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}\n");

            }
            Console.ResetColor();
        }

        static void MakeDeposit(int userIndex)
        {
            Clear();
            Console.Write("\n\tHow much $$ do you want to deposit? ");
            string? deposit = Console.ReadLine();
            float depositAmount;
            float.TryParse(deposit, out depositAmount);

            int userChoiceAccount = SelectAccount(userIndex);
            int goBackOption = GetGoBackOption(userIndex);

            bool userChoiceAccountIsValid = userChoiceAccount != -1;


            if (depositAmount <= 0)
            {
                NegativeAmount();
            }
            else if (userChoiceAccountIsValid)
            {
                bankUsers[userIndex].BankAccounts[userChoiceAccount].Balance += depositAmount;
                ViewAccountsAndBalance(userIndex);
            }
        }

        static void WithdrawMoney(int userIndex)
        {
            Clear();
            Console.Write("How much $$ do you want to withdraw? ");
            string? withdraw = Console.ReadLine();
            float withdrawAmount;
            float.TryParse(withdraw, out withdrawAmount);

            int userChoiceAccount = SelectAccount(userIndex);
            int goBackOption = GetGoBackOption(userIndex);
            bool userChoiceAccountIsValid = userChoiceAccount != -1;

            if (withdrawAmount <= 0)
            {
                NegativeAmount();
            }
            else if (userChoiceAccountIsValid)
            {
                Console.Write("\n\tENTER your Password: \n\t");
                string? password = Console.ReadLine();


                if (bankUsers[userIndex].Password == password)
                {
                    var accountBalance = bankUsers[userIndex].BankAccounts[userChoiceAccount].Balance;

                    if (withdrawAmount <= accountBalance)
                    {
                        bankUsers[userIndex].BankAccounts[userChoiceAccount].Balance -= withdrawAmount;
                        ViewAccountsAndBalance(userIndex);
                    }
                }
                else
                {
                    Console.WriteLine("\n\tYou don't have enough money");
                }
            }
        }

        static void TransferBtwAccounts(int userIndex)
        {
            Clear();
            Console.Write("\n\tTo whom do you want to transfer?: ");
            string? transferTo = Console.ReadLine();

            int targetUserIndex = GetBankUserIndexByUserName(transferTo);

            bool accountTargetFound = targetUserIndex != -1;

            if (accountTargetFound)
            {
                Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n\tselect type of account to transfer from:\n");
                Console.ResetColor();

                int userChoiceAccount = SelectAccount(userIndex);
                //int goBackOption = GetGoBackOption(userIndex);

                Console.WriteLine($"\n\tselect type of account to transfer to:\n");

                int targetUserChoiceAccount = SelectAccount(targetUserIndex);

                bool invalidUserChoiceAccount = userChoiceAccount == -1;

                if (!invalidUserChoiceAccount)
                {
                    ForegroundColor = ConsoleColor.Green;
                    Console.Write("\n\tHow much $$ do you want to transfer? ");
                    string? transfer = Console.ReadLine();
                    ResetColor();
                    float transferAmount;
                    float.TryParse(transfer, out transferAmount);

                    if (transferAmount <= 0)
                    {
                        NegativeAmount();
                    }
                    else if (bankUsers[userIndex].BankAccounts[userChoiceAccount].Balance < transferAmount)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n\tERROR! Not allowed. You don't have enough money");
                        ResetColor();
                        Console.WriteLine("\n\tPress ENTER to see your Balance");
                        Console.ReadLine();
                        ViewAccountsAndBalance(userIndex);

                    }
                    else
                    {

                        //add pin to confirm transaction

                        Console.Write("\n\tENTER your Password: \n\t");
                        string? password = Console.ReadLine();


                        if (bankUsers[userIndex].Password == password)
                        {
                            Clear();
                            //targetUserChoiceAccount = SelectAccount(targetUserIndex);
                            bool invalidTargetUserChoiceAccount = targetUserChoiceAccount == -1;

                            if (!invalidTargetUserChoiceAccount)
                            {
                                bankUsers[userIndex].BankAccounts[userChoiceAccount].Balance -= transferAmount;
                                bankUsers[targetUserIndex].BankAccounts[targetUserChoiceAccount].Balance += transferAmount;

                                ViewAccountsAndBalance(userIndex);
                            }

                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Transaction not allowed. Check your password!");
                            ResetColor();
                            //InvalidOption();
                        }
                    }

                }

                ////PRINT TO CHECK TRANSFER
                //for (int i = 0; i < bankUsers[userIndex].BankAccounts.Count; i++)
                //{
                //    //show balance accounts and types
                //    var accountBalance = bankUsers[userIndex].BankAccounts[i].Balance;
                //    var accountType = bankUsers[userIndex].BankAccounts[i].AccountType;

                //    var targetAccountBalance = bankUsers[targetUserIndex].BankAccounts[i].Balance;
                //    var targetAccountType = bankUsers[targetUserIndex].BankAccounts[i].AccountType;

                //    Console.WriteLine($"\n\tHi {bankUsers[userIndex].User.ToUpper()}!\n");
                //    Console.WriteLine($"\n\t{accountType}: {accountBalance.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}\n");

                //    Console.WriteLine($"\n\tHi {bankUsers[targetUserIndex].User.ToUpper()}!\n");
                //    Console.WriteLine($"\n\t{targetAccountType}: {targetAccountBalance.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}\n");

                //}
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tInvalid User".ToUpper());
                ResetColor();
                //InvalidOption();
            }
        }

        static void CreateNewAccount(int userIndex)
        {
            //all bank accounts
            List<string> allAccountTypes = new List<string>();
            allAccountTypes.Add("Savings");
            allAccountTypes.Add("Salary");

            Console.WriteLine($"\n\tselect account:\n");

            //get available accounts for creation
            for (int j = 0; j < allAccountTypes?.Count; j++)
            {
                var menuNumber = j;

                for (int i = 0; i < bankUsers[userIndex].BankAccounts.Count; i++)
                {
                    var accountType = bankUsers[userIndex].BankAccounts[i].AccountType;


                    if (allAccountTypes.Contains(accountType))
                    {
                        allAccountTypes.Remove(accountType);
                    }
                }

                //print menu with available accounts
                if (allAccountTypes.Count > 0)
                {
                    Clear();
                    Console.WriteLine($"\n\t{menuNumber + 1}. {allAccountTypes[j]}\n");

                    var goBackOption = GetGoBackOption(userIndex);

                    Console.WriteLine($"\n\t{goBackOption + 1}. Go Back\n");
                    Console.Write("\t");

                    //get menu choice

                    int userChoiceAccount;
                    int.TryParse(Console.ReadLine(), out userChoiceAccount);
                    bool userChoiceIsValid = Convert.ToBoolean(userChoiceAccount);
                    userChoiceAccount -= 1; //to get the correct index instead of the printed index

                    //add new account
                    if (userChoiceIsValid && userChoiceAccount != goBackOption)
                    {
                        for (int i = 0; i < allAccountTypes.Count; i++)
                        {
                            bankUsers[userIndex].BankAccounts.Add(new BankAccount(allAccountTypes[userChoiceAccount], 0));
                            ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"\n\t New account '{allAccountTypes[userChoiceAccount]}' successfully created!\t");
                            ResetColor();
                            Console.WriteLine("\n\t Press enter to check your accounts and balance\t");
                            ReadLine();
                            ViewAccountsAndBalance(userIndex);
                        }
                    }
                    else if (userChoiceAccount == goBackOption)
                    {
                        break;
                    }
                    else
                    {
                        InvalidOption();
                    }
                }
                else
                {
                    Clear();
                    ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n\t You already have all available accounts\n");
                    ResetColor();
                    break;
                }
            }
        }

        static void GoBackMenuOptions()
        {
            Console.WriteLine("\n\tPress ENTER to go back to the menu.\n");
            Console.ReadLine();
        }

        //static void NewBalance(int userIndex)
        //{
        //    Clear();
        //    Console.WriteLine("\n=============Balance Updated=============");
        //    ViewAccountsAndBalance(userIndex);
        //}

        static void InvalidOption()
        {
            ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\tERROR: Invalid Option!".ToUpper());
            ResetColor();
        }

        static void NegativeAmount()
        {
            ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\tERROR! Transaction not allowed." +
                "\n\tThe Amount needs to be valid or higher than 0,00".ToUpper());
            ResetColor();
        }

        static int SelectAccount(int userIndex)
        {
            Console.WriteLine($"\n\tselect account:\n");

            for (int i = 0; i < bankUsers[userIndex].BankAccounts.Count; i++)
            {
                var accountType = bankUsers[userIndex].BankAccounts[i].AccountType;
                var menuNumber = i;

                //to start menu choices with option 1 instead of 0
                Console.WriteLine($"\n\t{menuNumber + 1}. {accountType}\n");

            }
            var goBackOption = GetGoBackOption(userIndex);

            Console.WriteLine($"\n\t{goBackOption + 1}. Go Back\n");
            Console.Write("\t");

            int userChoiceAccount;
            bool userChoiceIsValid = int.TryParse(Console.ReadLine(), out userChoiceAccount);
            userChoiceAccount -= 1; //to get the correct index instead of the printed index

            if (userChoiceIsValid)
            {
                if (userChoiceAccount < bankUsers[userIndex].BankAccounts.Count)
                {
                    return userChoiceAccount;
                }
            }

            if (userChoiceAccount != goBackOption)
            {
                InvalidOption();
            }

            return -1;
        }

        static int GetGoBackOption(int userIndex)
        {
            return bankUsers[userIndex].BankAccounts.Count;
        }
    }
}
