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
            /*case 1: // load user passwords
                Console.WriteLine("Please, input username:");
                string usernameInput = Console.ReadLine();
                User user1 = new User(usernameInput);
                if (user1.HasLoginFile())
                {
                    Console.WriteLine($"Input user password for {usernameInput}: ");
                    string passwordInput = Console.ReadLine();
                    Console.WriteLine(passwordInput);
                    // implement a login system
                    //
                    Console.WriteLine($"user login status: {PasswordMethods.ValidateLogin(passwordInput, usernameInput)}");
                    Console.WriteLine("awgawgwagagwwga");
                    Console.WriteLine(user1.Login(passwordInput));
                    Console.WriteLine($"password for {user1.Username}: {PasswordMethods.FetchPassword(user1.Username)}");
                }
                else
                {
                    Console.WriteLine($"your user '{usernameInput}' does not have a profile registered");
                    Console.WriteLine("shutting down");
                    Environment.Exit(1);
                }
                break;*/
            case 1: // load user passwords
                Console.WriteLine("Please, input username:");
                string usernameInput = Console.ReadLine();
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
                    Console.WriteLine($"User login status: {isValidPassword}");

                    if (isValidPassword)
                    {
                        user1.LoggedIn = true;
                        Console.WriteLine($"User '{user1.LoggedIn}' logged in successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Invalid password for user '{usernameInput}'.");
                    }
                }
                else
                {
                    Console.WriteLine($"User '{usernameInput}' does not have a profile registered.");
                    Console.WriteLine("Shutting down.");
                    Environment.Exit(1);
                }
                break;
            case 2: // sign up a new account
                Console.WriteLine("Input a username to register:");
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

            case 3: // trying out json passwords
                // Example usage
                int saltSize = 16; // Choose your salt size
                string password = "examplePassword"; // Replace with the password you want to hash

                // Generate a salted hash for the password
                HashSalt hashSalt = HashSalt.GenerateSaltedHash(saltSize, password);

                // Serialize HashSalt object to JSON string
                string json = hashSalt.ToJson();

                // Write JSON string to a file (example.json)
                File.WriteAllText("example.json", json);

                // Read JSON string from the file (example.json)
                string jsonFromFile = File.ReadAllText("example.json");

                // Deserialize HashSalt object from JSON string
                HashSalt hashSaltFromFile = HashSalt.FromJson(jsonFromFile);

                // Verify the entered password against the stored hash and salt
                bool isPasswordValid = HashSalt.VerifyPassword(password, hashSaltFromFile.Hash, hashSaltFromFile.Salt);

                // Output the result
                if (isPasswordValid)
                {
                    Console.WriteLine("Password is valid!");
                }
                else
                {
                    Console.WriteLine("Password is invalid!");
                }

                break;
        }
    }
    
}