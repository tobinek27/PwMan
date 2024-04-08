namespace PwMan;
using System.Security.Cryptography;
using System.Text;
// example user folder: $"{currentPath}/user_files/<user_nickname>"
public class User
{
    private string nickname;
    private string password;

    public string Nickname
    {
        get { return nickname; }
        set 
        {
            if (value.Length > 2 && value.Length < 17)
            {
                nickname = value;
            }
            else
            {
                throw new ArgumentException("Nickname must be longer than 2 characters and shorter than 17 characters.");
            }
        }
    }

    public string Password
    {
        get { return password; }
        set { password = value; }
    }

    public bool HasFile()
    {
        string currentPath = Directory.GetCurrentDirectory();
        string filePath = $"{currentPath}/user_files/{nickname}";
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

    public static bool HasLoginFile(string inputUsername)
    {
        string username = inputUsername.Trim();
        if (username.Length > 2 && username.Length < 17)
        {
            string filePath = $"{Directory.GetCurrentDirectory()}/user_logins/{username}.txt";
            return File.Exists(filePath);
        }
        
        return false;
    }

    public bool CreateFile()
    {
        string currentPath = Directory.GetCurrentDirectory();
        string filePath = $"{currentPath}/user_files/{nickname}";
        if (!File.Exists(filePath))
        {
            try
            {
                using (File.Create(filePath)) { }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Error creating file: {e.Message}");
            }
        }

        throw new Exception("File already exists.");
    }

    public static void CreateLoginFile(string username, string password)
    {
        string currentPath = Directory.GetCurrentDirectory();
        string filePath = $"{currentPath}/user_logins/{username}.txt";
        try
        {
            // Generate salt based on the username
            byte[] salt = GenerateSalt(username);

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
    }
    
    public User(string nickname, string password)
    {
        this.nickname = nickname;
        this.password = password;
    }
    
    static byte[] GenerateSalt(string username)
    {
        // Convert username to bytes using UTF-8 encoding
        byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
        return usernameBytes;
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