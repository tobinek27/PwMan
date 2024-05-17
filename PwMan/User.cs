namespace PwMan;

using Newtonsoft.Json;

/// <summary>
/// Represents a user in the password management system.
/// </summary>
public class User
{
    private string? _username;
    public string Password { get; set; }
    public bool LoggedIn { get; set; }


    public string? Username
    {
        get => _username;
        set
        {
            if (value != null && value.Length > 2 && value.Length < 17)
            {
                _username = value;
            }
            else
            {
                throw new ArgumentException(
                    "Username must be longer than 2 characters and shorter than 17 characters.");
            }
        }
    }

    /// <summary>
    /// Checks if a login file exists for the given username.
    /// </summary>
    /// <param name="inputUsername">The username to check.</param>
    /// <returns>True if a login file exists; otherwise, false.</returns>
    public static bool HasLoginFile(string inputUsername)
    {
        string username = inputUsername.Trim();
        if (username.Length > 2 && username.Length < 17)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}/user_logins/{username}.json";
            return File.Exists(filePath);
        }

        return false;
    }

    /// <summary>
    /// Handles the login process for the user.
    /// </summary>
    /// <param name="usernameInput">The input username.</param>
    /// <param name="user">The User object to handle login for.</param>
    public static void HandleLogin(string usernameInput, User user)
    {
        string userInput = Program.CleanseInput(usernameInput);

        if (!HasLoginFile(userInput))
        {
            Program.StatusMessage = $"User '{userInput}' does not have a profile registered.";
            return;
        }

        Console.WriteLine($"Input user password for {userInput}: ");
        string passwordInput = Console.ReadLine();
        passwordInput = Program.CleanseInput(passwordInput);
        if (String.IsNullOrEmpty(passwordInput) || passwordInput.Length < 1)
        {
            Program.StatusMessage = $"Password {passwordInput} is either null, or too short.";
            return;
        }

        string filePath = $"{Directory.GetCurrentDirectory()}/user_logins/{userInput}.json";
        string json1 = File.ReadAllText(filePath);
        HashSalt storedHashSalt = HashSalt.FromJson(json1);
        bool isValidPassword = HashSalt.VerifyPassword(passwordInput, storedHashSalt.Hash, storedHashSalt.Salt);

        if (isValidPassword)
        {
            user.LoggedIn = true;
            Program.StatusMessage = $"User {userInput} logged in successfully.";
        }
        else
        {
            user.LoggedIn = false;
            Program.StatusMessage = $"Invalid password {passwordInput} for user '{userInput}'.";
        }

        user.Username = userInput;
        user.Password = passwordInput;
    }

    /// <summary>
    /// Handles the registration process for a new user.
    /// </summary>
    /// <param name="usernameToRegister">The username to register.</param>
    public static void HandleRegistration(string usernameToRegister)
    {
        usernameToRegister = Program.CleanseInput(usernameToRegister);
        if (HasLoginFile(usernameToRegister))
        {
            Console.WriteLine($"Sorry, but the username '{usernameToRegister}' is already taken");
            Program.StatusMessage = $"Registration failed, username '{usernameToRegister}' is already taken";
            return;
        }

        string inputPassword = "";
        while (inputPassword.ToLower() != "q")
        {
            Console.WriteLine(
                $"Input a password to be used for {usernameToRegister}: (8-256 characters, 'q' to cancel).");
            inputPassword = Console.ReadLine();
            inputPassword = Program.CleanseInput(inputPassword);

            if (inputPassword.Length < 8 || inputPassword.Length > 256)
            {
                Console.WriteLine("Password length should be between 8 and 256 characters.");
                continue;
            }

            int saltSize = 16;
            HashSalt hashSalt1 = HashSalt.GenerateSaltedHash(saltSize, inputPassword);
            CreateLoginFile(usernameToRegister, hashSalt1.Hash, hashSalt1.Salt);
            Program.StatusMessage = $"User account {usernameToRegister} created successfully.";
            return;
        }

        Console.WriteLine("Registration cancelled.");
        Program.StatusMessage = $"Registration of user {usernameToRegister} cancelled successfully.";
    }

    /// <summary>
    /// Creates a login file for the user.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="hash">The hashed password.</param>
    /// <param name="salt">The salt used for hashing.</param>
    private static void CreateLoginFile(string username, string hash, string salt)
    {
        string directoryPath = $"{Directory.GetCurrentDirectory()}/user_logins/";
        Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist

        HashSalt userLogin = HashSalt.CreateHashSalt(hash, salt);

        string json = JsonConvert.SerializeObject(userLogin);

        string filePath = Path.Combine(directoryPath, $"{username}.json");
        File.WriteAllText(filePath, json);
    }

    /// <summary>
    /// Gets the file path for the user's password file.
    /// </summary>
    /// <returns>The file path.</returns>
    public string GetPwFilePath()
    {
        string directory = $"{Directory.GetCurrentDirectory()}/user_files/";
        string filePath = $"{directory}{Username}.json";

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }

        return filePath;
    }

    /// <summary>
    /// Fetches the passwords of the user.
    /// </summary>
    /// <returns>A list of Password objects.</returns>
    public List<Password> FetchUserPasswords()
    {
        return PwMan.Password.ReadJson(GetPwFilePath());
    }

    /// <summary>
    /// Displays the passwords of the user.
    /// </summary>
    public void DisplayPasswords()
    {
        Console.Clear();
        Console.WriteLine($"Fetching passwords of the user: {Username}");
        List<Password> retrievedPasswords = PwMan.Password.ReadJson(this.GetPwFilePath());
        foreach (var password in retrievedPasswords)
        {
            Console.WriteLine($"Tag: {password.Tag}, Password: {password.PasswordValue}");
        }

        Console.WriteLine("End of user's saved passwords\r\n");
        Program.StatusMessage = $"User passwords displayed successfully.";

        Console.WriteLine("Press 'q' to return to the main menu.");
        while (Console.ReadKey(true).Key != ConsoleKey.Q)
        {
            // wait until 'q' is pressed
        }
    }

    /// <summary>
    /// Displays the passwords of the user for deletion.
    /// </summary>
    public void DisplayPasswordsForDeletion()
    {
        Console.Clear();
        Console.WriteLine($"Fetching passwords of the user: {Username}");
        List<Password> retrievedPasswords = PwMan.Password.ReadJson(GetPwFilePath());
        foreach (var password in retrievedPasswords)
        {
            Console.WriteLine($"Tag: {password.Tag}, Password: {password.PasswordValue}");
        }

        Console.WriteLine("End of user's saved passwords\r\n");
        Program.StatusMessage = $"User passwords displayed successfully.";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    public User()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class with the specified username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    public User(string username, string password)
    {
        Username = username;
        Password = password;
        LoggedIn = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class with the specified username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    public User(string username)
    {
        Username = username;
        Password = null;
        LoggedIn = false;
    }
}