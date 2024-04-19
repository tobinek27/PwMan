namespace PwMan;
using System.Text;
using System.Security.Cryptography;

public class PasswordMethods
{
    public static string GeneratePassword(int passwordLength = 64)
    {
        // const validCharacters contains all characters that could be used in the password generation process
        const string validCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+";
        StringBuilder password = new StringBuilder();
        Random random = new Random();

        for (int i = 0; i < passwordLength; i++)
        {
            password.Append(validCharacters[random.Next(validCharacters.Length)]);
        }

        return password.ToString();
    }
    
    public static byte[] GenerateSalt(string username)
    {
        // Convert username to bytes using UTF-8 encoding
        byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
        return usernameBytes;
    }
    
    public static string HashPassword(string password, byte[] salt)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] combinedBytes = Encoding.UTF8.GetBytes(password + Convert.ToBase64String(salt));
            byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashedBytes);
        }
    }
    
    public static bool ValidateLogin(string passwordInput, string usernameInput)
    {
        // hash and salt input password
        byte[] salt = PasswordMethods.GenerateSalt(usernameInput);
        string passwordInputHashed = PasswordMethods.HashPassword(passwordInput, salt);
        
        // now retrieve the user's password from user_logins
        string filePath = $"{Directory.GetCurrentDirectory()}/user_logins/{usernameInput}.txt";
        string readText = File.ReadAllText(filePath, Encoding.UTF8);
        return passwordInputHashed == readText;
    }
}