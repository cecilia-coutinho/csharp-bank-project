using System.IO;

namespace BankProject;
class Program
{
    static void Main(string[] args)
    {
        RunLoginSignUp();
    }

    static void RunLoginSignUp()
    {
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

    static void SignUpSystem()
    {
        //New user creation
        Console.Write("\nEnter a username: ");
        string? userName = Console.ReadLine();
        Console.Write("\nEnter a pin code: ");
        string? pinCode = Console.ReadLine();

        //file creation and storage data
        using (StreamWriter sw = new StreamWriter(File.Create(@".\my_file.txt")))
        {
            sw.Write(userName);
            sw.WriteLine(": " + pinCode);
            sw.Close();
        }
        Console.WriteLine("\nuser and pin code created with sucess!\n");
        Console.Read();
    }

    static void LoginSystem()
    {
        string? userName, pinCode, userName1, pinCode1 = string.Empty;
        Console.Write("\nEnter a username: ");
        userName = Console.ReadLine();
        Console.Write("\nEnter a pin code: ");
        pinCode = Console.ReadLine();

        using (StreamReader sr = new StreamReader(File.Open(@".\my_file.txt", FileMode.Open)))
        {
            userName1 = sr.ReadLine();
            pinCode1 = sr.ReadLine();
            sr.Close();
        }

        if (userName == userName1 && pinCode == pinCode1)
        {
            Console.WriteLine("Login succefull");
        }
        else
        {
            Console.WriteLine("Login Invalid.");
        }
        Console.Read();
    }
}
