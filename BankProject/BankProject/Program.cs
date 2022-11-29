using System.Dynamic;

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
            Console.WriteLine("Welcome");
            Console.Write("User: ");
            string? user = Console.ReadLine();
            Console.Write("Password: ");
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
                        string welcome1 = "Welcome to your account";
                        Console.WriteLine(welcome1.ToUpper());
                        Console.WriteLine("\tPlease select one of the options below:");
                        Console.WriteLine("\n\t1. View Accounts and Balance\n" +
                            "\n\t2. Deposit\n" +
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
                                Deposit(accountIndex);
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
                                runLoginMenu = false;
                                break;
                            default:
                                Console.Clear();
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("\n\tInvlid selection! " +
                                    "Please, choose 1-5 from the menu\n");
                                Console.ResetColor();
                                Console.WriteLine("\n\tclick enter to return to the menu\n");
                                Console.ReadLine();
                                break;
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine();
            }
        }

        static void ViewAccountsAndBalance(int accountIndex)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"\n\tHi {bankAccounts[accountIndex].User}!\n" +
                $"\n\tSavings: {bankAccounts[accountIndex].Savings}\n" +
                $"\n\tSalary: {bankAccounts[accountIndex].Salary}\n");
            Console.ResetColor();
        }

        static void Deposit(int accountIndex)
        {
            Console.Write("How much $$ do you want to deposit? ");
            string? deposit = Console.ReadLine();
            float depositAmount;
            float.TryParse(deposit, out depositAmount);

            Console.Write($"\n\tselect account:\n" +
                $"\n\t1. Savings\n" +
                $"\n\t2. Salary\n");

            int userChoiceAccount;
            int.TryParse(Console.ReadLine(), out userChoiceAccount);

            if (userChoiceAccount == 1)
            {
                bankAccounts[accountIndex].Savings += depositAmount;
                Console.WriteLine($"Your new balance is {bankAccounts[accountIndex].Savings}");
            }
            else if (userChoiceAccount == 2)
            {
                bankAccounts[accountIndex].Salary += depositAmount;
                Console.WriteLine($"Your new balance is {bankAccounts[accountIndex].Salary}");
            }
            else
            {
                Console.WriteLine("Invalid Option!");
            }
        }

        static void WithdrawMoney(int accountIndex)
        {
            Console.Write("How much $$ do you want to withdraw? ");
            string? withdraw = Console.ReadLine();
            float withdrawAmount;
            float.TryParse(withdraw, out withdrawAmount);

            Console.Write($"\n\tselect account:\n" +
                $"\n\t1. Savings\n" +
                $"\n\t2. Salary\n");

            int userChoiceAccount;
            int.TryParse(Console.ReadLine(), out userChoiceAccount);

            if (userChoiceAccount == 1)
            {
                if (withdrawAmount <= bankAccounts[accountIndex].Savings)
                {
                    bankAccounts[accountIndex].Savings -= withdrawAmount;
                    Console.WriteLine($"Your new balance is {bankAccounts[accountIndex].Savings}");
                }
                else
                {
                    Console.WriteLine("You don't have enough money");
                }
            }
            else if (userChoiceAccount == 2)
            {
                if (withdrawAmount <= bankAccounts[accountIndex].Salary)
                {
                    bankAccounts[accountIndex].Salary -= withdrawAmount;
                    Console.WriteLine($"Your new balance is {bankAccounts[accountIndex].Salary}");
                }
                else
                {
                    Console.WriteLine("You don't have enough money");
                }
            }
            else
            {
                Console.WriteLine("Invalid Option!");
            }
        }

        static void TransferBtwAccounts(int accountIndex)
        {
            Console.Write("To whom do you want to transfer?: ");
            string? transferTo = Console.ReadLine();

            int targetAccountIndex = GetAccountByUser(transferTo);

            bool accountTargetFound = targetAccountIndex != -1;
            if (accountTargetFound)
            {
                Console.Write($"\n\tselect type of account to transfer to:\n" +
                    $"\n\t1. Savings\n" +
                    $"\n\t2. Salary\n");

                int targetTypeAccount;
                int.TryParse(Console.ReadLine(), out targetTypeAccount);

                Console.Write("How much $$ do you want to transfer? ");
                string? transfer = Console.ReadLine();
                float transferAmount;
                float.TryParse(transfer, out transferAmount);

                Console.Write($"\n\tselect type of account to transfer from:\n" +
                    $"\n\t1. Savings\n" +
                    $"\n\t2. Salary\n");

                int srcTypeAccount;
                int.TryParse(Console.ReadLine(), out srcTypeAccount);

                if (srcTypeAccount == 1)
                {
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
                }
                else if (srcTypeAccount == 2)
                {
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
                }

                Console.WriteLine($"New Balance5: {bankAccounts[accountIndex].Savings}");
                Console.WriteLine($"New Balance: {bankAccounts[accountIndex].Salary}");
                Console.WriteLine($"New Balance: {bankAccounts[targetAccountIndex].Savings}");
                Console.WriteLine($"New Balance: {bankAccounts[targetAccountIndex].Salary}");
            }



        }
    }
}