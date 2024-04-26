namespace PwMan;
using System;
using System.IO;


class Program
{
    public static void Main(string[] args)
        { 
            User currentUser = new User();

            while (true)
            {
                if (currentUser.LoggedIn != true)
                {
                    Console.Clear();
                    Console.WriteLine("Welcome to PwMan!");
                    Console.WriteLine("Enter a username to log in, or type 'register' to sign up. (Enter 'q' to quit)");
                    Console.WriteLine("(keep in mind that usernames need to begin with a letter)");
                    string userInput = Console.ReadLine();

                    if (String.IsNullOrEmpty(userInput) || !char.IsLetter(userInput[0]) || userInput.Length < 3)
                    {
                        Console.WriteLine("invalid input\r\nterminating...");
                        break;
                    }
                    
                    if (userInput.ToLower() == "q")
                    {
                        Console.WriteLine("terminating the program...");
                        Environment.Exit(1);
                        break;
                    }
                    
                    if (!User.HasLoginFile(userInput))
                    {
                        HandleRegistration(userInput);
                    }
                    else if (User.HasLoginFile(userInput))
                    {
                        HandleLogin(userInput, currentUser);
                    }
                    else
                    {
                        Console.WriteLine("invalid command input, terminating...");
                        Environment.Exit(1);
                    }
                }
                else
                {
       
                    DisplayUserMenu(currentUser.Username);
                    
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "displaypw": // display user's saved passwords
                            
                            break;
                        case "generatepw": // generate a new password
                            Console.WriteLine("input a desired password length: (default=64)");
                            string input = Console.ReadLine();
                            if (string.IsNullOrEmpty(input))
                            {
                                string password = PasswordMethods.GeneratePassword();
                                Console.WriteLine(password);
                            }
                            else if (int.TryParse(input, out int passwordLength) && passwordLength >= 16 && passwordLength <= 256)
                            {
                                Console.WriteLine("generating a password");
                                string password = PasswordMethods.GeneratePassword(passwordLength);
                                Console.WriteLine(password);
                            }
                            else
                            {
                                Console.WriteLine("invalid input provided, please enter a valid length (16-256)");
                                return;
                            }
                            Console.WriteLine("Do you wish to save the password?");
                            Console.WriteLine("[y/n]");
                            var keyInfo = Console.ReadKey();
                            char userInput = char.ToLower(keyInfo.KeyChar);
                            if (userInput == 'n')
                            {
                                Console.WriteLine("\r\nokay, I'm not going to save the password then...");
                            }
                            else if (userInput == 'y')
                            {
                                Console.WriteLine("\r\nPlease, provide me with a tag:");
                                Console.WriteLine("(Each password is saved alongside a tag. The tag makes it easily " +
                                                  "distinguishable from other passwords)");
                            }
                            break;
                        case "logout": // logout the user
                            currentUser.LoggedIn = false;
                            Console.WriteLine("logged out successfully");
                            break;
                        case "q":
                            Console.WriteLine("terminating the program...");
                            Environment.Exit(1);
                            break;
                        default:
                            Console.WriteLine("invalid input");
                            break;
                    }
                }
            }
        }

    static void HandleLogin(string usernameInput, User user)
    {
        string userInput = usernameInput.Trim();
        if (User.HasLoginFile(userInput))
        {
            Console.WriteLine($"Input user password for {userInput}: ");
            string passwordInput = Console.ReadLine();
            if (String.IsNullOrEmpty(passwordInput) || passwordInput.Length < 1)
            {
                Console.WriteLine("password is either null, or too short!\r\nterminating...");
                Environment.Exit(1);
            }
        
            string filePath = $"{Directory.GetCurrentDirectory()}/user_logins/{userInput}.json";
            string json1 = File.ReadAllText(filePath);
            HashSalt storedHashSalt = HashSalt.FromJson(json1);
            bool isValidPassword = HashSalt.VerifyPassword(passwordInput, storedHashSalt.Hash, storedHashSalt.Salt);
            Console.WriteLine($"user {user.Username} login status: {isValidPassword}");

            if (isValidPassword)
            {
                user.LoggedIn = true;
                Console.WriteLine($"user '{user.Username}' logged in successfully");
            }
            else
            {
                user.LoggedIn = false;
                Console.WriteLine($"invalid password {passwordInput} for user '{userInput}'");
            }
            user.Username = userInput;
            user.Password = passwordInput;
        }
        else
        {
            Console.WriteLine($"user '{userInput}' does not have a profile registered");
            Console.WriteLine("shutting down");
            Environment.Exit(1);
        }
    }

    static void HandleRegistration(string usernameToRegister)
    {
        if (User.HasLoginFile(usernameToRegister))
        {
            Console.WriteLine($"sorry, but the username '{usernameToRegister}' is already taken.\r\nterminating...");
            return;
        }
        Console.WriteLine("no user with such name detected, registering a new user...");
        Console.WriteLine($"input a password to be used for {usernameToRegister}:");
        string inputPassword = Console.ReadLine();

        if (String.IsNullOrEmpty(inputPassword) || inputPassword.Length <= 3)
        {
            Console.WriteLine($"sorry, but the password {inputPassword} appears to be too short\r\nterminating...");
            return;
        }

        int saltSize = 16;
        HashSalt hashSalt1 = HashSalt.GenerateSaltedHash(saltSize, inputPassword);
        User.CreateLoginFile(usernameToRegister, inputPassword, hashSalt1.Password, hashSalt1.Salt);
        Console.WriteLine($"user '{usernameToRegister}' registered successfully");
    }

    static void DisplayUserMenu(string username)
    {
        //Console.Clear();
        Console.WriteLine("Welcome back, " + username + "!");
        Console.WriteLine("generatepw - generate a safe password");
        Console.WriteLine("logout - log out the current user");
        Console.WriteLine("q - terminate the program");
    }
}