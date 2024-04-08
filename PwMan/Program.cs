namespace PwMan;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to PwMan");
        User newUser = new User("fawfawf", "aaaa");
        Console.WriteLine(newUser.Nickname);
        Console.WriteLine(newUser.Password);
        
        // Check if the directory doesn't exist
        //string currentPath = Directory.GetCurrentDirectory();
        //string folderPath = $"{currentPath}/user_files/";
        //Console.WriteLine(currentPath);
        //Console.WriteLine(folderPath);
        
        //bool test = newUser.HasFile();
        //Console.WriteLine(test);
        //Console.WriteLine(newUser.CreateFile());
        /*if (!Directory.Exists(folderPath))
        {
            // Create the directory
         Directory.CreateDirectory(folderPath);
         Console.WriteLine("Folder created successfully.");
        }
        else
        {
          Console.WriteLine("Folder already exists.");
        }*/
        string testingPassword01 = GeneratePassword(10);
        string testingPassword64 = GeneratePassword();
        Console.WriteLine(testingPassword01);
        Console.WriteLine(testingPassword64);
        Console.WriteLine("please make a choice");
        Console.WriteLine("1 - load user passwords");
        Console.WriteLine("2 - sign up an account");
        int userInput = Convert.ToInt32(Console.ReadLine());
        switch (userInput)
        {
            case 1: // load user passwords
                Console.WriteLine("Please, input username:");
                string usernameInput = Console.ReadLine();
                if (User.HasFile(usernameInput))
                {
                    Console.WriteLine($"Input user password for {usernameInput}: ");
                    string passwordFromInput = Console.ReadLine();
                    Console.WriteLine(passwordFromInput);
                }
                else
                {
                    Console.WriteLine("AAAAAAA");
                }
                break;
            case 2: // sign up a new account
                Console.WriteLine("input a username to register");
                string usernameToRegister = Console.ReadLine();
                if (User.HasLoginFile(usernameToRegister))
                {
                    Console.WriteLine($"sorry, but the username '{usernameToRegister}' is already taken\r\nShutting down");
                    break;
                }
                Console.WriteLine($"input a password to be used for {usernameToRegister}");
                string inputPassword = Console.ReadLine();
                byte[] salt = GenerateSalt(usernameToRegister);
                string hashedPassword = HashPassword(inputPassword, salt);
                Console.WriteLine($"hashed password: {hashedPassword}");
                Console.WriteLine($"salt: {Convert.ToBase64String(salt)}");
                User.CreateLoginFile(usernameToRegister, hashedPassword);
                break;
        }
    }

    private static string GeneratePassword(int passwordLength = 64)
    {
        // const validCharacters contains all characters that could be used in the password generation process
        const string validCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
        StringBuilder password = new StringBuilder();
        Random random = new Random();

        for (int i = 0; i < passwordLength; i++)
        {
            password.Append(validCharacters[random.Next(validCharacters.Length)]);
        }

        return password.ToString();
    }
    
    static byte[] GenerateSalt(string username)
    {
        // Convert username to bytes using UTF-8 encoding
        byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
        return usernameBytes;
    }
    
    static string HashPassword(string password, byte[] salt)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] combinedBytes = Encoding.UTF8.GetBytes(password + Convert.ToBase64String(salt));
            byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashedBytes);
        }
    }
}