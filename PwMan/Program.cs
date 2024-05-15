using System.Text.RegularExpressions;

namespace PwMan;

using System;
using System.IO;

class Program
{
    private static string statusMessage;

    public static string StatusMessage { get; set; }

    public static void Main(string[] args)
    {
        User currentUser = new User();

        while (true)
        {
            if (currentUser.LoggedIn != true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to PwMan!");
                Console.WriteLine($"[{StatusMessage}]");
                Console.WriteLine("Please enter a username to begin the registration process");
                Console.WriteLine("if a user with such name already exists, you will be prompted to log in");
                Console.WriteLine("(Enter 'q' to quit)");
                Console.WriteLine("(keep in mind that usernames need to begin with a letter)");
                string userInput = Console.ReadLine();

                if (userInput.ToLower() == "q")
                {
                    Console.WriteLine("terminating the program...");
                    Console.WriteLine(GetRandomGoodbyePhrase());
                    Environment.Exit(1);
                    break;
                }

                if (String.IsNullOrEmpty(userInput) || !char.IsLetter(userInput[0]) || userInput.Length <= 3)
                {
                    StatusMessage = $"invalid input: {userInput}, please, try again";
                    continue;
                }

                if (!User.HasLoginFile(userInput))
                {
                    HandleRegistration(userInput);
                }
                else if (User.HasLoginFile(userInput))
                {
                    HandleLogin(userInput, currentUser);
                }
                /*else
                {
                    Console.WriteLine("invalid command input, terminating...");
                    Environment.Exit(1);
                }*/
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
                    /*case "generate": // generate a password
                        Console.Clear();
                        Console.WriteLine("input a desired password length: (default=64)");
                        string input = Console.ReadLine();

                        if ((int.TryParse(input, out int passwordLength) && passwordLength >= 8 &&
                             passwordLength <= 256) ||
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
                                        $"invalid input for tag: {tag}. The tag can't be empty or " +
                                        $"longer than 64 chars");
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
                        }

                        break;*/
                    case "generate": // generate a password
                        while (true)
                        {
                            Console.WriteLine("input a desired password length: (default=64) or enter 'q' to quit");
                            string input = Console.ReadLine().Trim();

                            if (input.ToLower() == "q")
                            {
                                // User wants to quit
                                break;
                            }

                            if (int.TryParse(input, out int passwordLength) && passwordLength >= 8 &&
                                passwordLength <= 256 || string.IsNullOrEmpty(input))
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
                                if (userInput != 'n')
                                {
                                    string tag;
                                    do
                                    {
                                        Console.Clear();
                                        Console.WriteLine("'q' to exit");
                                        Console.WriteLine("tag may contain alphanumeric characters (and a '/')");
                                        Console.WriteLine("please enter a tag:");
                                        tag = Console.ReadLine();
                                    } while (!TagIsValid(tag));

                                    Password passwordToSave = new Password(tag, password);
                                    if (passwordToSave.WriteToJson(currentUser.GetPwFilePath()))
                                    {
                                        Console.WriteLine("Password saved successfully.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to save the password.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("okay, not saving the password");
                                }
                            }
                            else
                            {
                                Console.WriteLine("invalid password provided, please enter a valid length (8-256)");
                            }
                        }

                        break;
                    case "delete": // delete a password based on the input tag
                        break;
                    case "search": // search for passwords based on the input tag
                        SearchForPasswords(currentUser);
                        break;
                    case "logout": // logout the user
                        Console.Clear();
                        string recentlyLoggedOutUser = currentUser.Username;
                        currentUser.LoggedIn = false;
                        StatusMessage = $"user {recentlyLoggedOutUser} logged out successfully";
                        break;
                    case "q": // terminate the program
                        Console.Clear();
                        Console.WriteLine("terminating the program...");
                        Console.WriteLine(GetRandomGoodbyePhrase());
                        Environment.Exit(1);
                        break;
                    default: // invalid input
                        Console.WriteLine("invalid input");
                        continue;
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

            if (isValidPassword)
            {
                user.LoggedIn = true;
                StatusMessage = $"user {userInput} logged in successfully";
            }
            else
            {
                user.LoggedIn = false;
                StatusMessage = $"invalid password {passwordInput} for user '{userInput}'";
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

    static string GetRandomGoodbyePhrase()
    {
        Random random = new Random();
        string[] phrases =
        {
            "Catch you later!",
            "Farewell, friend!",
            "Until we meet again!",
            "Adios amigo!",
            "Bye-bye for now!",
            "Take care and goodbye!",
            "See you later, alligator!",
            "In a while, crocodile!",
            "Keep smiling, see you soon!",
            "Goodbye, have a great day!"
        };
        int index = random.Next(phrases.Length);
        return phrases[index];
    }

    static void HandleRegistration(string usernameToRegister)
    {
        if (User.HasLoginFile(usernameToRegister))
        {
            Console.WriteLine($"Sorry, but the username '{usernameToRegister}' is already taken.");
            return;
        }

        string inputPassword = "";
        while (inputPassword.ToLower() != "q")
        {
            Console.WriteLine(
                $"input a password to be used for {usernameToRegister}: (3-256 characters, 'q' to cancel)");
            inputPassword = Console.ReadLine();

            if (inputPassword.Length < 3 || inputPassword.Length > 256)
            {
                Console.WriteLine("Password length should be between 3 and 256 characters.");
                continue;
            }

            int saltSize = 16;
            HashSalt hashSalt1 = HashSalt.GenerateSaltedHash(saltSize, inputPassword);
            User.CreateLoginFile(usernameToRegister, hashSalt1.Hash, hashSalt1.Salt);
            Console.WriteLine($"User account '{usernameToRegister}' registered successfully.");
            return;
        }

        Console.WriteLine("Registration canceled.");
        StatusMessage = $"registration of user {usernameToRegister} cancelled successfully";
    }


    static void DisplayUserMenu(string username)
    {
        Console.WriteLine($"Welcome back, {username}!");
        Console.WriteLine($"[{StatusMessage}]");
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
            Console.Clear();
            Console.WriteLine($"Found {searchResults.Count} password(s) with tag '{searchTag}':");
            foreach (var password in searchResults)
            {
                Console.WriteLine($"Tag: {password.Tag}, Password: {password.PasswordValue}");
            }
        }
        else
        {
            Console.Clear();
            Console.WriteLine($"No passwords found with tag '{searchTag}'.");
        }
    }

    static void SearchForPasswords(User currentUser)
    {
        Console.Clear();
        while (true)
        {
            //Console.Clear();
            Console.WriteLine("Enter a tag to search for (type 'q' to quit):");
            string searchTag = Console.ReadLine();

            if (searchTag.ToLower() == "q")
            {
                Console.WriteLine("quitting search...");
                Console.Clear();
                return;
            }

            if (string.IsNullOrEmpty(searchTag))
            {
                Console.WriteLine("invalid input for tag");
                Console.WriteLine("press any key to continue...");
                Console.ReadKey();
                Console.Clear();
                continue;
            }

            List<Password> passwordsOfUser = Password.ReadJson(currentUser.GetPwFilePath());
            List<Password> searchResults = Password.SearchPasswords(passwordsOfUser, searchTag);

            DisplaySearchResults(searchResults, searchTag);
        }
    }

    static bool TagIsValid(string tag)
    {
        if (string.IsNullOrEmpty(tag) || tag.Length > 64 || tag == "q")
        {
            return false;
        }

        return Regex.IsMatch(tag, @"^[a-zA-Z0-9/]*$");
    }
}