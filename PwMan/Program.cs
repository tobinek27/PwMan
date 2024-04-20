namespace PwMan;

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to PwMan!");
        
        Console.WriteLine("please make a choice");
        Console.WriteLine("1 - load user passwords");
        Console.WriteLine("2 - sign up an account");
        int userInput = Convert.ToInt32(Console.ReadLine());
        switch (userInput)
        {
            case 1: // load user passwords
                Console.WriteLine("Please, input username:");
                string usernameInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(usernameInput))
                {
                    Console.WriteLine("username can't be empty or whitespace");
                    return;
                }
                usernameInput = usernameInput.Trim();
                User user1 = new User(usernameInput);
                if (user1.HasLoginFile())
                {
                    Console.WriteLine($"Input user password for {usernameInput}: ");
                    string passwordInput = Console.ReadLine();
        
                    // Retrieve the stored password information from the JSON file
                    string filePath = $"{Directory.GetCurrentDirectory()}/user_logins/{usernameInput}.json";
                    string json1 = File.ReadAllText(filePath);
                    HashSalt storedHashSalt = HashSalt.FromJson(json1);
        
                    // Validate the entered password against the stored hash and salt
                    bool isValidPassword = HashSalt.VerifyPassword(passwordInput, storedHashSalt.Hash, storedHashSalt.Salt);
        
                    // Output the login status
                    Console.WriteLine($"user login status: {isValidPassword}");

                    if (isValidPassword)
                    {
                        user1.LoggedIn = true;
                        Console.WriteLine($"user '{user1.LoggedIn}' logged in successfully");
                    }
                    else
                    {
                        user1.LoggedIn = false;
                        Console.WriteLine($"invalid password for user '{usernameInput}'");
                    }
                }
                else
                {
                    Console.WriteLine($"user '{usernameInput}' does not have a profile registered");
                    Console.WriteLine("shutting down");
                    Environment.Exit(1);
                }
                break;
            case 2: // sign up a new account
                Console.WriteLine("input a username to register:");
                string usernameToRegister = Console.ReadLine();
                if (User.HasLoginFile(usernameToRegister))
                {
                    Console.WriteLine($"Sorry, but the username '{usernameToRegister}' is already taken.\r\nShutting down.");
                    break;
                }
                Console.WriteLine($"Input a password to be used for {usernameToRegister}:");
                string inputPassword = Console.ReadLine();

                int saltSize1 = 16; // Choose your salt size
                HashSalt hashSalt1 = HashSalt.GenerateSaltedHash(saltSize1, inputPassword);

                // Create login file for the new user
                User.CreateLoginFile(usernameToRegister, inputPassword, hashSalt1.Password, hashSalt1.Salt);
                Console.WriteLine($"User '{usernameToRegister}' registered successfully.");
                break;
        }
    }
    
}