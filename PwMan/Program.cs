namespace PwMan;
using System;
using System.IO;


class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to PwMan!");
        
        Console.WriteLine("list of commands:");
        Console.WriteLine("you can log in by typing out a username");
        Console.WriteLine("or you can input 'register' to sign up a new user!");
        string userInput = Console.ReadLine();
        switch (userInput)
        {
            case not null when (userInput != "register" && userInput.Length > 2 && userInput.Length < 17):
                userInput = userInput.Trim();
                User user1 = new User(userInput);
                if (user1.HasLoginFile())
                {
                    Console.WriteLine($"Input user password for {userInput}: ");
                    string passwordInput = Console.ReadLine();
                    if (String.IsNullOrEmpty(passwordInput) || passwordInput.Length < 1)
                    {
                        Console.WriteLine("password is either null, or too short!\r\nterminating");
                        break;
                    }
        
                    // Retrieve the stored password information from the JSON file
                    string filePath = $"{Directory.GetCurrentDirectory()}/user_logins/{userInput}.json";
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
                        Console.WriteLine($"invalid password {passwordInput} for user '{userInput}'");
                    }
                }
                else
                {
                    Console.WriteLine($"user '{userInput}' does not have a profile registered");
                    Console.WriteLine("shutting down");
                    Environment.Exit(1);
                }
                break;
            case "register": // sign up a new account
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
            default:
                Console.WriteLine("invalid command input, terminating");
                Environment.Exit(1);
                break;
        }
    }
    
}