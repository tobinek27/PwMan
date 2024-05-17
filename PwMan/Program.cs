using System.Text.RegularExpressions;

namespace PwMan;

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
                Console.WriteLine("If a user with such name already exists, you will be prompted to log in.");
                Console.WriteLine("(Enter 'q' to quit)");
                Console.WriteLine("(keep in mind that usernames need to begin with a letter)");
                string userInput = Console.ReadLine();
                userInput = CleanseInput(userInput);

                if (userInput.ToLower() == "q")
                {
                    Console.WriteLine("Terminating the program...");
                    Console.WriteLine(GetRandomGoodbyePhrase());
                    Environment.Exit(1);
                    break;
                }

                if (String.IsNullOrEmpty(userInput) || !char.IsLetter(userInput[0]) || userInput.Length <= 3)
                {
                    StatusMessage = $"Invalid input: {userInput}. Please, try again.";
                    continue;
                }

                if (!User.HasLoginFile(userInput))
                {
                    User.HandleRegistration(userInput);
                }
                else if (User.HasLoginFile(userInput))
                {
                    User.HandleLogin(userInput, currentUser);
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
                        currentUser.DisplayPasswords();
                        break;
                    case "generate": // generate a password
                        Password.GeneratePassword(currentUser);
                        break;
                    case "delete": // delete a password based on the input tag
                        Password.DeletePassword(currentUser);
                        break;
                    case "search": // search for passwords based on the input tag
                        Password.SearchForPasswords(currentUser);
                        break;
                    case "logout": // log the user out
                        Console.Clear();
                        string recentlyLoggedOutUser = currentUser.Username;
                        currentUser.LoggedIn = false;
                        StatusMessage = $"User {recentlyLoggedOutUser} logged out successfully.";
                        break;
                    case "q": // terminate the program
                        Console.Clear();
                        Console.WriteLine("Terminating the program...");
                        Console.WriteLine(GetRandomGoodbyePhrase());
                        currentUser.LoggedIn = false;
                        Environment.Exit(1);
                        break;
                    default: // invalid input
                        Console.WriteLine("Invalid input");
                        StatusMessage = "Invalid input.";
                        continue;
                }
            }
        }
    }

    /// <summary>
    /// Returns a random goodbye phrase from a predefined set of phrases.
    /// </summary>
    /// <returns>A random goodbye phrase as a string.</returns>
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

    /// <summary>
    /// Displays the user menu with various options.
    /// </summary>
    /// <param name="username">The username of the current user.</param>
    private static void DisplayUserMenu(string username)
    {
        Console.WriteLine($"Welcome back, {username}!");
        Console.WriteLine($"[{StatusMessage}]");
        Console.WriteLine("generate - generates a safe password");
        Console.WriteLine("delete - deletes a password by tag");
        Console.WriteLine("display - displays all passwords saved by the user");
        Console.WriteLine("search - searches for passwords by tag");
        Console.WriteLine("logout - logs out the current user");
        Console.WriteLine("q - terminates the program");
    }

    /// <summary>
    /// Validates the given tag string to ensure it meets the required criteria.
    /// </summary>
    /// <param name="tag">The tag string to validate.</param>
    /// <returns>
    /// True if the tag is not null or empty, does not exceed 64 characters,
    /// and contains only alphanumeric characters and forward slashes; otherwise, false.
    /// </returns>
    public static bool TagIsValid(string tag)
    {
        return !(string.IsNullOrEmpty(tag) || tag.Length > 64) && Regex.IsMatch(tag, @"^[a-zA-Z0-9/]*$");
    }

    /// <summary>
    /// Cleanses the input string by removing excessive whitespace and trimming leading and trailing whitespace.
    /// </summary>
    /// <param name="input">The input string to cleanse.</param>
    /// <returns>The cleansed input string.</returns>
    public static string CleanseInput(string input)
    {
        input = Regex.Replace(input, @"\s+", "");
        return input.Trim();
    }
}