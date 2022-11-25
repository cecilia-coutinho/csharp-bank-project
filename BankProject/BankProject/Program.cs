using System.IO;
using BankProject; //to access the objects from the AppDataContext.cs
using System.Security.Cryptography; //to use the SHA method
using System.Text;
using static System.Console;
using Microsoft.IdentityModel.Tokens;

namespace BankProject;
class Program
{
    static void Main(string[] args)
    {
        RunLoginSignUp();
    }

    static void RunLoginSignUp()
    {
        //Clear();
        string welcome = "\n\tWelcome to the Bank XXXX!\n";

        bool runMenu = true;
        while (runMenu)
        {
            Console.WriteLine(welcome.ToUpper());
            Console.WriteLine("\tPlease select one of the options below:");
            Console.WriteLine("\n\t1. Login\n" +
                "\n\t2. Sign Up\n");
            Console.Write("\t Select menu: ");
            int menuChoice;
            int.TryParse(Console.ReadLine(), out menuChoice);

            switch (menuChoice)
            {
                case 1:
                    LoginSystem();
                    break;
                case 2:
                    SignUpSystem();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\n\tChoose 1-2 from the menu\n");
                    Console.ResetColor();
                    break;
            }
        }
    }

    static void LoginSystem()
    {
        Clear();
        WriteLine("===========LOGIN===========");
        Console.Write("\nEnter a username: ");
        string? userName = Console.ReadLine();
        Console.Write("\nEnter a pin code: ");
        string? password = Console.ReadLine();

        //set-map the user to DB
        using AppDataContext context = new AppDataContext(); //create new instance

        //to check user in db
        var userFound = context.Users.Any(user => user.Name == userName); //to return a bool

        if (userFound)
        {
            var loginUser = context.Users.FirstOrDefault(user => user.Name == userName); //to access an get/return the 1st user with the name

            if (password != null && loginUser != null)
            {
                //if the user pass is the same as stored
                if (HashPassword($"{password}{loginUser.Salt}") == loginUser.Password)
                {
                    Clear();
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Login Sucessful");
                    Console.ResetColor();
                    Console.WriteLine("click enter to return to menu");
                    ReadLine();
                }
                else
                {
                    InvalidLogin();
                }
                Clear();
            }
            else
            {
                InvalidLogin();
            }
        }
        else
        {
            InvalidLogin();
        }

    }

    static void SignUpSystem()
    {
        Clear();
        WriteLine("===========SIGN UP===========");
        Console.Write("\nEnter a username: ");
        string? userName = Console.ReadLine();
        Console.Write("\nEnter a pin code: ");
        string? password = Console.ReadLine();

        if (userName != null && password != null && !userName.IsNullOrEmpty() && !password.IsNullOrEmpty())
        {
            //set-map the user to DB
            using AppDataContext context = new AppDataContext(); //create new instance

            var saltedPassword = DateTime.Now.ToString();
            var hashedPassword = HashPassword($"{password}{saltedPassword}"); //to hash the pass

            //to create a new user to db
            context.Users.Add(new User() { Name = userName, Password = hashedPassword, Salt = saltedPassword });
            context.SaveChanges();

            bool isRunning = true;
            while (isRunning)
            {
                Clear();
                ForegroundColor = ConsoleColor.DarkGreen;
                WriteLine("Sucessfull Registration!");
                Console.ResetColor();
                Console.WriteLine("[E] Exit");

                if (ReadKey().Key == ConsoleKey.B)
                {
                    RunLoginSignUp();
                }
                isRunning = false;
            }
        }
        else
        {
            Clear();
            ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Registration failed");
            Console.ResetColor();
            Console.WriteLine("click enter to return to menu");
            ReadLine();
        }

    }

    static string HashPassword(string password)
    {
        SHA256 hash = SHA256.Create(); //create instance off sha class
        var passwordBytes = Encoding.Default.GetBytes(password); //take string to return an array of bytes
        var hashedPassword = hash.ComputeHash(passwordBytes); //to return array of bytes
        return Convert.ToHexString(hashedPassword); //to convert the array of bytes to string encoded with hex characters
    }

    static void InvalidLogin()
    {
        Clear();
        ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("Login Invalid");
        Console.ResetColor();
        Console.WriteLine("click enter to return to menu");
        ReadLine();
    }

}
