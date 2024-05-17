namespace PwMan;

using System.Text;

/// <summary>
/// Contains method(s) for password manipulation.
/// </summary>
public class PasswordMethods
{
    /// <summary>
    /// Generates a random password with the specified length using a set of valid characters.
    /// </summary>
    /// <param name="passwordLength">The length of the password to generate. Default is 64.</param>
    /// <returns>A randomly generated password as a string.</returns>
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