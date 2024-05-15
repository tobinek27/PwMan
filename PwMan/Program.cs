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
                userInput = CleanseInput(userInput);

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
            }
            else
            {
                DisplayUserMenu(currentUser.Username);

                string choice = Console.ReadLine();
                choice = CleanseInput(choice);

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
                        while (true)
                        {
                            Console.WriteLine("input a desired password length: (default=64) or enter 'q' to quit");
                            string input = Console.ReadLine();
                            input = CleanseInput(input);

                            if (input.ToLower() == "q")
                            {
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
                                        tag = CleanseInput(tag);
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
                        currentUser.LoggedIn = false;
                        Environment.Exit(1);
                        break;
                    default: // invalid input
                        Console.WriteLine("invalid input");
                        continue;
                }
            }
        }
    }

    private static void HandleLogin(string usernameInput, User user)
    {
        string userInput = usernameInput.Trim();

        if (!User.HasLoginFile(userInput))
        {
            StatusMessage = $"user '{userInput}' does not have a profile registered";
            return;
        }

        Console.WriteLine($"input user password for {userInput}: ");
        string passwordInput = Console.ReadLine();
        passwordInput = CleanseInput(passwordInput);
        if (String.IsNullOrEmpty(passwordInput) || passwordInput.Length < 1)
        {
            StatusMessage = $"password {passwordInput} is either null, or too short";
            return;
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

    private static string GetRandomGoodbyePhrase()
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

    private static void HandleRegistration(string usernameToRegister)
    {
        if (User.HasLoginFile(usernameToRegister))
        {
            Console.WriteLine($"sorry, but the username '{usernameToRegister}' is already taken");
            StatusMessage = $"registration failed, username '{usernameToRegister}' is already taken";
            return;
        }

        string inputPassword = "";
        while (inputPassword.ToLower() != "q")
        {
            Console.WriteLine(
                $"input a password to be used for {usernameToRegister}: (8-256 characters, 'q' to cancel)");
            inputPassword = Console.ReadLine();
            inputPassword = CleanseInput(inputPassword);

            if (inputPassword.Length < 8 || inputPassword.Length > 256)
            {
                Console.WriteLine("password length should be between 8 and 256 characters");
                continue;
            }

            int saltSize = 16;
            HashSalt hashSalt1 = HashSalt.GenerateSaltedHash(saltSize, inputPassword);
            User.CreateLoginFile(usernameToRegister, hashSalt1.Hash, hashSalt1.Salt);
            StatusMessage = $"user account {usernameToRegister} created successfully";
            return;
        }

        Console.WriteLine("registration cancelled");
        StatusMessage = $"registration of user {usernameToRegister} cancelled successfully";
    }

    private static void DisplayUserMenu(string username)
    {
        Console.WriteLine($"Welcome back, {username}!");
        Console.WriteLine($"[{StatusMessage}]");
        Console.WriteLine("generate - generates a safe password");
        Console.WriteLine("display - displays all passwords saved by the user");
        Console.WriteLine("search - searches for passwords by tag");
        Console.WriteLine("logout - logs out the current user");
        Console.WriteLine("q - terminates the program");
    }

    private static void DisplaySearchResults(List<Password> searchResults, string searchTag)
    {
        if (searchResults.Count > 0)
        {
            Console.Clear();
            Console.WriteLine($"Found {searchResults.Count} password(s) with tag '{searchTag}':");
            foreach (var password in searchResults)
            {
                Console.WriteLine($"Tag: {password.Tag}, Password: {password.PasswordValue}");
            }

            return;
        }

        Console.Clear();
        Console.WriteLine($"no passwords found with tag '{searchTag}'");
    }

    private static void SearchForPasswords(User currentUser)
    {
        Console.Clear();
        string searchTag = "";
        while (searchTag.ToLower() != "q")
        {
            Console.WriteLine("Enter a tag to search for (type 'q' to quit):");
            searchTag = Console.ReadLine();
            searchTag = CleanseInput(searchTag);

            List<Password> passwordsOfUser = Password.ReadJson(currentUser.GetPwFilePath());
            List<Password> searchResults = Password.SearchPasswords(passwordsOfUser, searchTag);

            DisplaySearchResults(searchResults, searchTag);
        }
    }

    private static bool TagIsValid(string tag)
    {
        if (string.IsNullOrEmpty(tag) || tag.Length > 64 || tag == "q")
        {
            return false;
        }

        return Regex.IsMatch(tag, @"^[a-zA-Z0-9/]*$");
    }

    private static string CleanseInput(string input)
    {
        input = Regex.Replace(input, @"\s+", " ");
        return input.Trim();
    }
}