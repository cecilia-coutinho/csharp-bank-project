using System.Dynamic;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Xml;
using static System.Console; //to simplify Console references

namespace BankProject
{
    internal class Program
    {
        static BankUser[] bankUsers = {
            new BankUser(0, "maria", "1234", new List<BankAccount>{new BankAccount("Savings", 1000), new BankAccount("Salary", 100) }),
            new BankUser(0, "pedro", "1234", new List<BankAccount>{new BankAccount("Savings", 100), new BankAccount("Salary", 10) }),
            new(0, "kalle", "1234", new List<BankAccount>{new BankAccount("Savings", 100), new BankAccount("Salary", 0) }),
            new BankUser(0, "johan", "1234", new List < BankAccount >{new BankAccount("Savings", 0), new BankAccount("Salary", 0) }),
            new BankUser(0, "matias", "1234", new List < BankAccount >{new BankAccount("Savings", 0), new BankAccount("Salary", 0) }) };

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

            int userIndex = GetBankUserIndexByUserName(user); //get user index
            bool accountFound = userIndex != -1;

            if (accountFound)
            {
                if (bankUsers[userIndex].Password == password)
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
                NewBalance(userIndex);
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
                        NewBalance(userIndex);
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

                        //ADD PIN TO CONFIRM TRANSACTION

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

                                NewBalance(userIndex);
                            }

                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Transaction not allowed. Check your data!");
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
        static void GoBackMenuOptions()
        {
            Console.WriteLine("\n\tPress ENTER to go back to the menu.\n");
            Console.ReadLine();
        }

        static void NewBalance(int userIndex)
        {
            Clear();
            Console.WriteLine("\n=============Balance Updated=============");
            ViewAccountsAndBalance(userIndex);
        }

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