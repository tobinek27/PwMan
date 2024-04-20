namespace PwMan;

using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public class User
{
    private string _username;
    private string? _password;
    private bool _loggedIn;

    public string Username
    {
        get => _username;
        set
        {
            if (value.Length > 2 && value.Length < 17)
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

    public string? Password
    {
        get => _password;
        set => _password = value;
    }

    public bool LoggedIn
    {
        get => _loggedIn;
        set => _loggedIn = value;
    }

    public bool Login(string password)
    {
        try
        {
            bool loginValue = PasswordMethods.ValidateLogin(password, Username);
            LoggedIn = loginValue;
            Console.WriteLine(
                $"login value: {loginValue}, loggedIn: {LoggedIn}, Username: {Username}, password: {password}");
            return LoggedIn;
        }
        catch (Exception e)
        {
            throw new Exception($"error logging in the user {Username}: {e.Message}");
        }
    }

    public bool HasFile()
    {
        string currentPath = Directory.GetCurrentDirectory();
        string filePath = $"{currentPath}/user_files/{Username}";
        return File.Exists(filePath);
    }

    public static bool HasFile(string inputUsername)
    {
        string username = inputUsername.Trim();
        if (username.Length > 2 && username.Length < 17)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}/user_files/{username}";
            return File.Exists(filePath);
        }

        return false;
    }

    public bool HasLoginFile()
    {
        string filePath = $"{Directory.GetCurrentDirectory()}/user_logins/{Username}.json";
        return File.Exists(filePath);
    }

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

    public bool CreateFile()
    {
        string currentPath = Directory.GetCurrentDirectory();
        string filePath = $"{currentPath}/user_files/{Username}";
        if (!File.Exists(filePath))
        {
            try
            {
                using (File.Create(filePath))
                {
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Error creating file: {e.Message}");
            }
        }

        throw new Exception("File already exists.");
    }

    /*public static void CreateLoginFile(string username, string password)
    {
        string currentPath = Directory.GetCurrentDirectory();
        string filePath = $"{currentPath}/user_logins/{username}.txt";
        try
        {
            // Generate salt based on the username
            byte[] salt = PasswordMethods.GenerateSalt(username);

            // Hash the password with the salt
            string hashedPassword = HashPassword(password, salt);

            // Save hashed password and salt to file
            File.WriteAllText(filePath, $"{hashedPassword}:{Convert.ToBase64String(salt)}");

            Console.WriteLine("Login file created successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating file: {e.Message}");
        }
    }*/

    public static void CreateLoginFile(string username, string password, string hash, string salt)
    {
        string directoryPath = $"{Directory.GetCurrentDirectory()}/user_logins/";
        Directory.CreateDirectory(directoryPath); // Create the directory if it doesn't exist

        // Create a UserLogin object to store password, hash, and salt
        HashSalt userLogin = new HashSalt
        {
            Password = password,
            Hash = hash,
            Salt = salt
        };

        // Serialize UserLogin object to JSON string
        string json = JsonConvert.SerializeObject(userLogin);

        // Write JSON string to a file named after the username
        string filePath = Path.Combine(directoryPath, $"{username}.json");
        File.WriteAllText(filePath, json);
    }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
        LoggedIn = false;
    }

    public User(string username)
    {
        Username = username;
        Password = null;
        LoggedIn = false;
    }

    /*static byte[] GenerateSalt(string username)
    {
        // Convert username to bytes using UTF-8 encoding
        byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
        return usernameBytes;
    }*/

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