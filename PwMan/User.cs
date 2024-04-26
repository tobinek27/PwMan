namespace PwMan;

using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

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

    public User()
    {
        
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