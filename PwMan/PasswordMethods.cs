namespace PwMan;

using System.Text;

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
}