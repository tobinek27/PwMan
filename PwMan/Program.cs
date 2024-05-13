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
                Console.WriteLine("Please enter a username to begin the registration process");
                Console.WriteLine("if a user with such name already exists, you will be prompted to log in");
                Console.WriteLine("(Enter 'q' to quit)");
                Console.WriteLine("(keep in mind that usernames need to begin with a letter)");
                string userInput = Console.ReadLine();

                if (String.IsNullOrEmpty(userInput) || !char.IsLetter(userInput[0]) || userInput.Length <= 3)
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
                    case "display": // display user's saved passwords
                        Console.Clear();
                        Console.WriteLine($"fetching passwords of the user: {currentUser.Username}");
                        List<Password> retrievedPasswords = Password.ReadJson(currentUser.GetPwFilePath());
                        foreach (var password in retrievedPasswords)
                        {
                            Console.WriteLine($"tag: {password.Tag}, Password: {password.PasswordValue}");
                        }

                        Console.WriteLine("end of user's saved passwords\r\n");
                        break;
                    case "generate": // generate a password
                        Console.Clear();
                        Console.WriteLine("input a desired password length: (default=64)");
                        string input = Console.ReadLine();

                        if ((int.TryParse(input, out int passwordLength) && passwordLength <= 256) ||
                            string.IsNullOrEmpty(input))
                        {
                            Console.WriteLine("Generating a password");
                            if (string.IsNullOrEmpty(input))
                            {
                                passwordLength = 64;
                            }

                            string password = PasswordMethods.GeneratePassword(passwordLength);
                            Console.WriteLine(password);

                            Console.WriteLine("Do you wish to save the password?");
                            Console.WriteLine("[y/n]");
                            var keyInfo = Console.ReadKey();
                            char userInput = char.ToLower(keyInfo.KeyChar);
                            if (userInput == 'y' || keyInfo.Key == ConsoleKey.Enter)
                            {
                                Console.WriteLine("\r\nPlease, provide me with a tag:");
                                Console.WriteLine("(Each password is saved alongside a tag." +
                                                  "The tag makes it easily distinguishable from other passwords)");

                                string tag = Console.ReadLine();
                                if (string.IsNullOrEmpty(tag) || tag.Length > 64)
                                {
                                    Console.WriteLine(
                                        $"Invalid input for tag: {tag}. The tag can't be empty or " +
                                        $"longer than 64 chars");
                                    //return;
                                    break;
                                }

                                // Save the password alongside the tag
                                Password passwordToSave = new Password(tag, password);
                                passwordToSave.WriteToJson(currentUser.GetPwFilePath());
                            }
                            else if (userInput == 'n')
                            {
                                Console.WriteLine("\r\nokay, I'm not going to save the password then...");
                            }
                        }
                        else
                        {
                            Console.WriteLine("invalid input provided, please enter a valid length (16-256)");
                            //return;
                        }

                        break;
                    case "delete": // delete a password based on the input tag
                        break;
                    case "search":
                        SearchForPasswords(currentUser);
                        break;
                    case "logout": // logout the user
                        Console.Clear();
                        string recentlyLoggedOutUser = currentUser.Username;
                        currentUser.LoggedIn = false;
                        Console.WriteLine($"user {recentlyLoggedOutUser} logged out successfully");
                        break;
                    case "q": // terminate the program
                        Console.Clear();
                        Console.WriteLine("terminating the program...");
                        Environment.Exit(1);
                        break;
                    default: // invalid input
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

        if (String.IsNullOrEmpty(inputPassword) || inputPassword.Length < 3)
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
        Console.WriteLine("Welcome back, " + username + "!");
        Console.WriteLine("generate - generates a safe password");
        Console.WriteLine("display - displays all passwords saved by the user");
        Console.WriteLine("search - searches for passwords by tag");
        Console.WriteLine("logout - logs out the current user");
        Console.WriteLine("q - terminates the program");
    }

    static void DisplaySearchResults(List<Password> searchResults, string searchTag)
    {
        if (searchResults.Count > 0)
        {
            Console.WriteLine($"Found {searchResults.Count} password(s) with tag '{searchTag}':");
            foreach (var password in searchResults)
            {
                Console.WriteLine($"Tag: {password.Tag}, Password: {password.PasswordValue}");
            }
        }
        else
        {
            Console.WriteLine($"No passwords found with tag '{searchTag}'.");
        }
    }

    static void SearchForPasswords(User currentUser)
    {
        Console.Clear();
        Console.WriteLine("Enter a tag to search for:");
        string searchTag = Console.ReadLine();

        if (string.IsNullOrEmpty(searchTag))
        {
            Console.WriteLine("Invalid input for tag.");
            return;
        }

        List<Password> passwordsOfUser = Password.ReadJson(currentUser.GetPwFilePath());
        List<Password> searchResults = Password.SearchPasswords(passwordsOfUser, searchTag);

        DisplaySearchResults(searchResults, searchTag);
    }
}