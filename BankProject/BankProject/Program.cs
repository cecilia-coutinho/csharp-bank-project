using Bytescout.Spreadsheet;
using System.Drawing;
using System.Globalization;
using System.Security;
using System.Speech.Synthesis.TtsEngine;
using static System.Console; //to simplify Console references

namespace BankProject
{
    internal class Program
    {
        // users data declared as a static variable of the class
        // can be accessed from any method or class within the namespace
        static BankUser[] bankUsers = {
            new BankUser("maria", "1234", new List<BankAccount>{new BankAccount("Savings", 1000), new BankAccount("Salary", 100) }),
            new BankUser("pedro", "1234", new List<BankAccount>{new BankAccount("Savings", 100) }),
            new BankUser("kalle", "1234", new List<BankAccount>{new BankAccount("Savings", 100) }),
            new BankUser("johan", "1234", new List < BankAccount >{new BankAccount("Salary", 0) }),
            new BankUser("matias", "1234", new List < BankAccount >{new BankAccount("Savings", 0), new BankAccount("Salary", 0) }) };

        static int GetBankUserIndexByUserName(string? user)
        {
            //to get index of array of each user
            for (int i = 0; i < bankUsers.Length; i++)
            {
                if (bankUsers[i].User == user)
                {
                    return i;
                }
            }

            return -1; //invalid index (if user not found)
        }

        static void Main(string[] args)
        {
            //check if the file exists, to load it
            if (File.Exists(@".\writeExcelOutput.xlsx"))
            {
                LoadSpreadsheetData(); //check the data from the file that saves it (that way modified data is not reset once the program is restarted)
            }
            else
            {
                SaveSpreadsheet(); //get and save data to spreadsheet
            }

            RunSystem();
            SaveSpreadsheet(); //save modified data to spreadsheet
        }

        static void RunSystem()
        {
            Clear();
            PrintWelcome(); //welcome text
            Console.Write("\n\tUser: \n\t");
            string? user = Console.ReadLine()?.ToLower(); //get username input

            int userIndex = GetBankUserIndexByUserName(user); //get user index

            bool accountFound = userIndex != -1; //check if exists (return true if user index is valid)

            if (accountFound)
            {
                //if the user is found and not locked with failed password attempts:
                if (bankUsers[userIndex].LockAccountDateTime.AddMinutes(3) < DateTime.Now)
                {
                    CheckPassAndRunAccount(user, userIndex); //check passwaord before run program with menu options
                }
                else
                {
                    /*
                     * if there were failed attempts
                     * Block new login attempt after 3 minutes
                    */
                    Clear();
                    TimeSpan lockCounter = bankUsers[userIndex].LockAccountDateTime.AddMinutes(3) - DateTime.Now;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n\tMax failed attempts.\n".ToUpper());
                    Console.ResetColor();
                    Console.WriteLine($"\n\tPlease, wait! You can login again after {SetTimeFormat(lockCounter)}\n");
                    ReadLine();
                    RunSystem();
                }
            }
            else
            {
                //if the user is NOT found 
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tInvalid username\n".ToUpper());
                Console.ResetColor();
                Console.WriteLine($"\n\tPress ENTER to try again.\n");
                Console.ReadLine();
                RunSystem();
            }
        }

        static string SetTimeFormat(TimeSpan t)
        {
            //to format remaining time shown in the failed attempts message when user try login before 3 min
            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                (int)t.TotalHours,
                t.Minutes,
                t.Seconds);
        }

        static void PrintWelcome()
        {
            //welcome message
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n\t============WELCOME TO BANK XZ!============\n");
            Console.ResetColor();

        }
        static Spreadsheet SaveSpreadsheet()
        {
            // Create new Spreadsheet
            Spreadsheet document = new Spreadsheet();
            document.Workbook.DefaultFont = new SpreadsheetFont("Arial", 10); //font format

            // Add new worksheet
            Worksheet sheet = document.Workbook.Worksheets.Add("my_excel"); //create new tab in the document

            int startRow = 0; //rows counter

            //set titles in the sheet
            SetSpreadsheetTitles(sheet, startRow);

            //get and set data
            PrintWorksheetUserData(sheet, startRow);

            //check if the file exists, replace and save it
            if (File.Exists(@".\writeExcelOutput.xlsx"))
            {
                File.Delete(@".\writeExcelOutput.xlsx");
            }
            document.SaveAs(@".\writeExcelOutput.xlsx");

            // AutoFit all columns
            sheet.Columns[0].AutoFit();
            sheet.Columns[1].AutoFit();
            sheet.Columns[2].AutoFit();
            sheet.Columns[3].AutoFit();

            document.Close();

            return document;
        }

        private static void AddAllBorders(Cell cell)
        {
            //add style to spreadsheet
            cell.LeftBorderStyle = Bytescout.Spreadsheet.Constants.LineStyle.Thin;
            cell.RightBorderStyle = Bytescout.Spreadsheet.Constants.LineStyle.Thin;
            cell.TopBorderStyle = Bytescout.Spreadsheet.Constants.LineStyle.Thin;
            cell.BottomBorderStyle = Bytescout.Spreadsheet.Constants.LineStyle.Thin;
        }

        static void PrintWorksheetUserData(Worksheet sheet, int startRow)
        {
            //add data to spreadsheet
            for (int i = 0; i < bankUsers.Length; i++)
            {
                //add user index
                sheet.Cell((++startRow), 0).Value = GetBankUserIndexByUserName(bankUsers[i].User);
                AddAllBorders(sheet.Cell(startRow, 0));

                //add username
                sheet.Cell(startRow, 1).Value = bankUsers[i].User;
                sheet.Cell(startRow, 1).AlignmentHorizontal = Bytescout.Spreadsheet.Constants.AlignmentHorizontal.Right;
                AddAllBorders(sheet.Cell(startRow, 1));

                //add userPass
                sheet.Cell((startRow), 2).Value = bankUsers[i].Password;
                sheet.Cell(startRow, 2).AlignmentHorizontal = Bytescout.Spreadsheet.Constants.AlignmentHorizontal.Right;
                AddAllBorders(sheet.Cell(startRow, 2));

                // add bankAccount Data (balance of each account type)
                for (int j = 0; j < bankUsers[i].BankAccounts.Count; j++)
                {
                    string accountType = bankUsers[i].BankAccounts[j].AccountType;
                    double balance = bankUsers[i].BankAccounts[j].Balance;

                    if (accountType.Contains("Savings"))
                    {
                        // convert balance in 2-digits
                        double twoDigitsBalance;
                        double.TryParse(String.Format("{0:0.00}", balance), out twoDigitsBalance);
                        sheet.Cell((startRow), 3).Value = twoDigitsBalance;

                        sheet.Cell(startRow, 3).AlignmentHorizontal = Bytescout.Spreadsheet.Constants.AlignmentHorizontal.Right;
                        AddAllBorders(sheet.Cell(startRow, 3));
                    }
                    if (accountType.Contains("Salary"))
                    {
                        // convert balance in 2-digits
                        double twoDigitsBalance;
                        double.TryParse(String.Format("{0:0.00}", balance), out twoDigitsBalance);
                        sheet.Cell((startRow), 4).Value = twoDigitsBalance;
                        sheet.Cell(startRow, 4).AlignmentHorizontal = Bytescout.Spreadsheet.Constants.AlignmentHorizontal.Right;
                        AddAllBorders(sheet.Cell(startRow, 4));
                    }
                }
            }
        }

        static void SetSpreadsheetTitles(Worksheet sheet, int row)
        {
            // to set titles to the spreadsheet
            sheet.Cell((row), 0).Value = $"UserIndex".ToUpper();
            AddAllBorders(sheet.Cell(row, 0));
            sheet.Cell(row, 0).Font = new Font("Arial", 11, FontStyle.Bold);

            sheet.Cell((row), 1).Value = $"Username".ToUpper();
            AddAllBorders(sheet.Cell(row, 1));
            sheet.Cell(row, 1).Font = new Font("Arial", 11, FontStyle.Bold);

            sheet.Cell((row), 2).Value = $"UserPass".ToUpper();
            AddAllBorders(sheet.Cell(row, 2));
            sheet.Cell(row, 2).Font = new Font("Arial", 11, FontStyle.Bold);

            sheet.Cell((row), 3).Value = $"Savings".ToUpper();
            AddAllBorders(sheet.Cell(row, 3));
            sheet.Cell(row, 3).Font = new Font("Arial", 11, FontStyle.Bold);

            sheet.Cell((row), 4).Value = $"Salary".ToUpper();
            AddAllBorders(sheet.Cell(row, 4));
            sheet.Cell(row, 4).Font = new Font("Arial", 11, FontStyle.Bold);
        }

        static void LoadSpreadsheetData()
        {
            //to read document and get updated data from spreadsheet

            Spreadsheet document = new Spreadsheet();
            document.LoadFromFile("writeExcelOutput.xlsx");

            // Get first worksheet
            Worksheet sheet = document.Workbook.Worksheets[0];

            //iterate with spreadsheet rows
            for (int rowIndex = 1; rowIndex < sheet.NotEmptyRowMax; rowIndex++)
            {
                //iterate with spreadsheet columns
                for (int columnIndex = 0; columnIndex < sheet.NotEmptyRowMax + 1; columnIndex++)
                {
                    // get data from titles
                    string columnTitle = sheet.Cell(0, columnIndex).ValueAsString;

                    //to get updated data from spreadsheet
                    if (columnTitle == "USERNAME")
                    {
                        bankUsers[rowIndex - 1].User = sheet.Cell(rowIndex, columnIndex).ValueAsString;
                        bankUsers[rowIndex - 1].BankAccounts.Clear();
                    }
                    else if (columnTitle == "USERPASS")
                    {
                        bankUsers[rowIndex - 1].Password = sheet.Cell(rowIndex, columnIndex).ValueAsString;
                    }
                    else if (columnTitle == "SAVINGS")
                    {
                        bool hasSavings = sheet.Cell(rowIndex, columnIndex).ValueAsString.Length > 0;
                        if (hasSavings)
                        {
                            double savingsBalance = sheet.Cell(rowIndex, columnIndex).ValueAsDouble;
                            bankUsers[rowIndex - 1].BankAccounts.Add(new BankAccount("Savings", savingsBalance));
                        }
                    }
                    else if (columnTitle == "SALARY")
                    {
                        bool hasSalary = sheet.Cell(rowIndex, columnIndex).ValueAsString.Length > 0;
                        if (hasSalary)
                        {
                            double salaryBalance = sheet.Cell(rowIndex, columnIndex).ValueAsDouble;
                            bankUsers[rowIndex - 1].BankAccounts.Add(new BankAccount("Salary", salaryBalance));
                        }
                    }
                }
            }
            document.Dispose();
        }

        private static SecureString MaskInputString()
        {
            Console.WriteLine("\n\tPassword: \n\t");
            SecureString password = new SecureString();
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(true);
                //to show each char as an "*"
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    password.AppendChar(keyInfo.KeyChar);
                    Console.Write("*");
                }
                //to remove when backspacing
                else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.RemoveAt(password.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (keyInfo.Key != ConsoleKey.Enter);
            {
                return password;
            }

        }
        static void CheckPassAndRunAccount(string? user, int userIndex)
        {
            // to check password before run menu
            int maxInvalidPassAttempts = 3; //max number of failed attempts
            int countPassAttempts; //failed attempts counter

            //while the user attempts are less than max failed attempts
            for (countPassAttempts = 1; countPassAttempts <= maxInvalidPassAttempts; countPassAttempts++)
            {
                Clear();

                PrintWelcome();
                SecureString pass = MaskInputString();
                string password = new System.Net.NetworkCredential(string.Empty, pass).Password;

                //check if password is correct. if so, run menu options
                if (bankUsers[userIndex].Password == password)
                {
                    RunMenu(user, password, userIndex);
                    break;
                }
                else
                {
                    //max attempts failed password
                    //user can try again if failed attempts are less than 3
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
                        //block user from loggin for 3 minutes after 3 failed attempts
                        //get time from last failed attempts after 3 times failed to block user
                        bankUsers[userIndex].LockAccountDateTime = DateTime.Now;

                        //show message to the user
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n\tInvalid password!\n".ToUpper());
                        Console.ResetColor();
                        Console.WriteLine($"\n\tFailed Attempts: {countPassAttempts}. You can try again after 3 minutes");
                        Console.ReadLine();
                        RunSystem();
                    }
                }
            }
        }


        static void RunMenu(string? user, string? pass, int userIndex)
        {
            //to run menu options
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
                    "\n\t6. Money Exchange Simulation\n" +
                    "\n\t7. Change Password\n" +
                    "\n\t8. Log Out\n");
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
                        SaveSpreadsheet();
                        GoBackMenuOptions();
                        break;
                    case 3:
                        TransferBtwAccounts(userIndex);
                        SaveSpreadsheet();
                        GoBackMenuOptions();
                        break;
                    case 4:
                        WithdrawMoney(userIndex);
                        SaveSpreadsheet();
                        GoBackMenuOptions();
                        break;
                    case 5:
                        CreateNewAccount(userIndex);
                        SaveSpreadsheet();
                        GoBackMenuOptions();
                        break;
                    case 6:
                        /*
                         * only does a simulation with a fixed exchange rate
                         * The method doesn't do anything directly in the user account
                        */
                        MenuCurrencyConverterSimulation();
                        break;
                    case 7:
                        ChangePassword(userIndex);
                        GoBackMenuOptions();
                        break;
                    case 8:
                        SaveSpreadsheet(); //save data in the database when logout
                        Console.WriteLine("\n\tThanks for your visit!");
                        Thread.Sleep(1000);

                        RunSystem(); //come back to login option
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
            // to see accounts the user has and money amount
            Clear();
            Console.WriteLine("\n=============BALANCE=============");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($"\n\tHi {bankUsers[userIndex].User.ToUpper()}!\n");

            //to get user and bank account
            for (int i = 0; i < bankUsers[userIndex].BankAccounts.Count; i++)
            {
                ///get balance accounts and types
                var accountBalance = bankUsers[userIndex].BankAccounts[i].Balance;
                var accountType = bankUsers[userIndex].BankAccounts[i].AccountType;

                //show balance accounts and types
                //print balance in 2-digits for currency type
                Console.WriteLine($"\n\t{accountType}: {accountBalance.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}\n");

            }
            Console.ResetColor();
        }
        static void MakeDeposit(int userIndex)
        {
            //to make a deposit
            Clear();
            WriteLine("\n\t****Please, type enter in 2-digit format, ex: 00,00 ****\n" +
                "\n\t****Note: if you enter more than 2 digits it will be converted to 2 digits****\n");
            ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\tHow much $$ do you want to deposit?\n");
            Write("\n\t======> ");
            ResetColor();
            string? deposit = Console.ReadLine();
            int? decimalPointIndex = deposit?.IndexOf('.');

            if (decimalPointIndex == -1)
            {
                double depositAmount = 0;

                try
                {
                    if (deposit != null)
                    {
                        depositAmount = Convert.ToDouble(deposit, CultureInfo.CreateSpecificCulture("sv-SE"));
                    }
                }
                catch
                {
                    depositAmount = 0;
                }

                if (depositAmount <= 0)
                {
                    NegativeAmount(); //show message if the amount has a negative value
                }
                else
                {

                    int userChoiceAccount = SelectAccount(userIndex); //to select type of account to make deposit
                    bool userChoiceAccountIsValid = userChoiceAccount != -1; //boolean to check option input for account's type

                    if (userChoiceAccountIsValid)
                    {
                        // to add the amount in the acount
                        bankUsers[userIndex].BankAccounts[userChoiceAccount].Balance += depositAmount;
                        ViewAccountsAndBalance(userIndex);
                    }
                }
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("\n\tPlease enter an amount in whole SEK or with cents\n" +
                    "\n\tEx: 100 or 100,54\n");
                ResetColor();
                WriteLine("\n\t Press ENTER to try again");
                ReadLine();
                MakeDeposit(userIndex);
            }
        }

        static void WithdrawMoney(int userIndex)
        {
            // to withdraw money
            Clear();
            WriteLine("\n\t****Please, enter in whole SEK format, ex: 00 **** \n" +
                "\n\t****Note:it's not possible withdraw cents****\n");
            ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\tHow much $$ do you want to withdraw?");
            Write("\n\tType Amount ======> ");
            ResetColor();

            string? withdraw = Console.ReadLine();


            int withdrawAmount;
            int.TryParse(withdraw, out withdrawAmount);

            if (withdrawAmount <= 0)
            {
                NegativeAmount(); //show message if the amount has a negative value
                ForegroundColor = ConsoleColor.Red;
                WriteLine("\n\tNOTE: YOU CAN NOT WITHDRAW CENTS ONLY THE WHOLE SEK, Ex: 100");
                ResetColor();
            }
            else
            {
                int userChoiceAccount = SelectAccount(userIndex); //menu options with accounts' type
                bool userChoiceAccountIsValid = userChoiceAccount != -1; //boolean to check option input for account's type


                if (userChoiceAccountIsValid)
                {
                    Console.Write("\n\tENTER your Password: \n\t");
                    string? password = Console.ReadLine(); //get pass to confirm transaction

                    // check pass
                    if (bankUsers[userIndex].Password == password)
                    {
                        //to add the amount in the acount
                        var accountBalance = bankUsers[userIndex].BankAccounts[userChoiceAccount].Balance;

                        if (withdrawAmount <= accountBalance)
                        {
                            bankUsers[userIndex].BankAccounts[userChoiceAccount].Balance -= withdrawAmount;
                            ViewAccountsAndBalance(userIndex);
                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n\tYou don't have enough money".ToUpper()); // if the amount is higher than money that user has
                            ResetColor();
                            WriteLine("\n\tPress ENTER to see your balance");
                            ReadLine();
                            ViewAccountsAndBalance(userIndex);
                        }
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n\tNOT ALLOWED! Invalid password"); // if the amount is higher than money that user has
                        ResetColor();
                    }
                }
            }


        }

        static void TransferBtwAccounts(int userIndex)
        {
            //to transfer between different accounts and users
            Clear();
            Console.Write("\n\tTo whom do you want to transfer?: ");
            string? transferTo = Console.ReadLine(); //input to get username target to transfer to

            int targetUserIndex = GetBankUserIndexByUserName(transferTo); //get input/target index
            bool accountTargetFound = targetUserIndex != -1; //check if input/target index exists

            if (accountTargetFound)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n\tselect type of account to transfer from:\n");
                Console.ResetColor();

                int userChoiceAccount = SelectAccount(userIndex); //options with account types
                bool userChoiceAccountIsValid = userChoiceAccount != -1; //boolean to check option input for account's type

                if (userChoiceAccountIsValid)
                {
                    ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n\tselect type of account to transfer to:\n");
                    ResetColor();

                    int targetUserChoiceAccount = SelectAccount(targetUserIndex); //options with target account types
                    bool invalidUserChoiceAccount = targetUserChoiceAccount == -1; //check if not exists

                    if (!invalidUserChoiceAccount)
                    {
                        //if exists
                        Clear();
                        WriteLine("\n\tPlease, type enter in 2-digit format, ex: 00,00\n" +
                            "\n\t****Note: if you enter more than 2 digits it will be converted to 2 digits****\n");
                        ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n\tHow much $$ do you want to transfer?");
                        Write("\n\tType Amount ======> ");
                        ResetColor();
                        string? transfer = Console.ReadLine();
                        int? decimalPointIndex = transfer?.IndexOf('.');

                        if (decimalPointIndex == -1)
                        {
                            double transferAmount = 0;

                            try
                            {
                                if (transfer != null)
                                {
                                    transferAmount = double.Parse(transfer, CultureInfo.CreateSpecificCulture("sv-SE"));
                                }
                            }
                            catch
                            {
                                transferAmount = 0;
                            }


                            if (transferAmount <= 0)
                            {
                                NegativeAmount(); //message for negative amount
                            }
                            else if (bankUsers[userIndex].BankAccounts[userChoiceAccount].Balance < transferAmount)
                            {
                                //message when amount is higher than money available
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("\n\tERROR! Not allowed. You don't have enough money");
                                ResetColor();
                                Console.WriteLine("\n\tPress ENTER to see your Balance");
                                Console.ReadLine();
                                ViewAccountsAndBalance(userIndex);

                            }
                            else
                            {
                                /* enough amount money available in the account
                                 * add pin to confirm transaction
                                 */

                                Console.Write("\n\tENTER your Password: \n\t");
                                string? password = Console.ReadLine();

                                //check pass
                                if (bankUsers[userIndex].Password == password)
                                {
                                    Clear();
                                    bool invalidTargetUserChoiceAccount = targetUserChoiceAccount == -1;

                                    if (!invalidTargetUserChoiceAccount)
                                    {
                                        //update account values in user and target
                                        bankUsers[userIndex].BankAccounts[userChoiceAccount].Balance -= transferAmount;
                                        bankUsers[targetUserIndex].BankAccounts[targetUserChoiceAccount].Balance += transferAmount;

                                        ViewAccountsAndBalance(userIndex); //show user money updated

                                        ////PRINT TO CHECK TRANSFER (both users)
                                        //for (int i = 0; i < bankUsers[userIndex].BankAccounts.Count; i++)
                                        //{
                                        //    //show balance accounts and types
                                        //    var accountBalance = bankUsers[userIndex].BankAccounts[i].Balance;
                                        //    var accountType = bankUsers[userIndex].BankAccounts[i].AccountType;

                                        //    var targetAccountBalance = bankUsers[targetUserIndex].BankAccounts[i].Balance;
                                        //    var targetAccountType = bankUsers[targetUserIndex].BankAccounts[i].AccountType;

                                        //    Console.WriteLine($"\n\tHi {bankUsers[userIndex].User.ToUpper()}!\n");
                                        //    Console.WriteLine($"\n\t{accountType}: {accountBalance.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}\n");

                                        //    Console.WriteLine($"\n\t{bankUsers[targetUserIndex].User.ToUpper()} Balance:\n");
                                        //    Console.WriteLine($"\n\t{targetAccountType}: {targetAccountBalance.ToString("c2", CultureInfo.CreateSpecificCulture("sv-SE"))}\n");
                                        //}
                                    }
                                }
                                else
                                {
                                    //wrong password
                                    ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("\n\tNOT ALLOWED! Invalid password");
                                    ResetColor();
                                }
                            }
                        }
                        else
                        {
                            ForegroundColor = ConsoleColor.Red;
                            WriteLine("\n\tPlease enter an amount in whole SEK or with cents\n" +
                                "\n\tEx: 100 or 100,54\n");
                            ResetColor();
                            WriteLine("\n\t Press ENTER to try again");
                            ReadLine();
                            TransferBtwAccounts(userIndex);
                        }
                    }
                }
            }
            else
            {
                //wrong target
                ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tInvalid User".ToUpper());
                ResetColor();
            }
        }

        static void CreateNewAccount(int userIndex)
        {
            //to create a new account
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
                        InvalidOption(); //message for invalid option input
                    }
                }
                else
                {
                    //when user has all the accounts already
                    Clear();
                    ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n\t You already have all available accounts\n");
                    ResetColor();
                    break;
                }
            }
        }

        static void ChangePassword(int userIndex)
        {
            // to change password
            Clear();
            Console.Write("\n\tType your new Password: ");
            string? passwordNewOne = Console.ReadLine(); //get new pass
            if (passwordNewOne != null && passwordNewOne != "")
            {
                //to update data
                bankUsers[userIndex].Password = passwordNewOne;
                SaveSpreadsheet();
                ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n\tthe password was successfully changed!\n".ToUpper());
                ResetColor();
            }
            else
            {
                //message to catch a null/empty input
                ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n\tERROR! Password could not be changed");
                ResetColor();
            }
        }

        static bool MenuCurrencyConverterSimulation()
        {
            /*
             * only does a simulation with a fixed exchange rate
             * The method doesn't do anything directly in the user account
            */

            Clear();
            Console.WriteLine("\n\tTo which currency do you want to convert?\n\tPlease select one of the options below:");
            Console.WriteLine("\n\t1. From SEK to Dollar\n" +
                "\n\t2. From SEK to Euro\n" +
                "\n\t3. Go Back to menu\n");
            Console.Write("\t Select menu: ");

            int menuChoice;
            int.TryParse(Console.ReadLine(), out menuChoice);

            switch (menuChoice)
            {
                case 1:
                    GetCurrencyConvertToDollar();
                    GoBackMenuOptions();
                    return true;
                case 2:
                    GetCurrencyConvertToEuro();
                    GoBackMenuOptions();
                    return true;
                case 3:
                    return false;
                default:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n\tPlease, choose 1-3 from the menu\n");
                    Console.ResetColor();
                    ReadLine();
                    MenuCurrencyConverterSimulation();
                    return true;
            }
        }

        static void GetCurrencyConvertToDollar()
        {
            //calling method from a class and setting variables
            Clear();
            Console.Write("\n\tHow much SEK do you want to Convert: ");
            double currencyAmount;
            double.TryParse(Console.ReadLine(), out currencyAmount);
            ConvertToDollar convertToDollar = new ConvertToDollar(currencyAmount);
            Console.WriteLine(convertToDollar.PrintCurrencyConverter(currencyAmount));
        }

        static void GetCurrencyConvertToEuro()
        {
            //calling method from a class and setting variables
            Clear();
            Console.Write("\n\tHow much SEK do you want to Convert: ");
            double currencyAmount;
            double.TryParse(Console.ReadLine(), out currencyAmount);
            ConvertToEuro convertToEuro = new ConvertToEuro(currencyAmount);
            Console.WriteLine(convertToEuro.PrintCurrencyConverter(currencyAmount));
        }

        static void GoBackMenuOptions()
        {
            //option to go back to the main menu
            Console.WriteLine("\n\tPress ENTER to go back to the menu.\n");
            Console.ReadLine();
        }
        static void InvalidOption()
        {
            // message for invalid option
            ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\tERROR: Invalid Option!".ToUpper());
            ResetColor();
        }

        static void NegativeAmount()
        {
            //message for negative amount input
            ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n\tERROR! Transaction not allowed." +
                "\n\tThe Amount needs to be valid or higher than 0,00".ToUpper());
            ResetColor();
        }

        static int SelectAccount(int userIndex)
        {
            //menu option to show account types
            Console.WriteLine($"\n\tselect account:\n");

            //to get account types
            for (int i = 0; i < bankUsers[userIndex].BankAccounts.Count; i++)
            {
                var accountType = bankUsers[userIndex].BankAccounts[i].AccountType;
                var menuNumber = i;

                //to start menu choices with option 1 instead of 0
                Console.WriteLine($"\n\t{menuNumber + 1}. {accountType}\n");

            }
            var goBackOption = GetGoBackOption(userIndex); //option to go back

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
            //to go back when showing menu account's option
            return bankUsers[userIndex].BankAccounts.Count;
        }
    }
}
