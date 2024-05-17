using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public class HashSalt
{
    public string? Hash { get; set; }
    public string? Salt { get; set; }
    public string? Password { get; set; }


    public static HashSalt GenerateSaltedHash(int size, string password)
    {
        var saltBytes = new byte[size];
        var random = new Random();
        random.NextBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);

        using (var sha256 = SHA256.Create())
        {
            byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
            var hashedPassword = Convert.ToBase64String(hashedBytes);

            HashSalt hashSalt = CreateHashSalt(hashedPassword, salt);
            return hashSalt;
        }
    }

    public static HashSalt FromJson(string json)
    {
        return JsonConvert.DeserializeObject<HashSalt>(json);
    }

    public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
    {
        var saltBytes = Convert.FromBase64String(storedSalt);

        using (var sha256 = SHA256.Create())
        {
            byte[] combinedBytes = Encoding.UTF8.GetBytes(enteredPassword + Convert.ToBase64String(saltBytes));
            byte[] hashedBytes = sha256.ComputeHash(combinedBytes);

            var generatedHash = Convert.ToBase64String(hashedBytes);

            return generatedHash == storedHash;
        }
    }

    private HashSalt()
    {
    }

    public static HashSalt CreateHashSalt(string hash, string salt)
    {
        return new HashSalt { Hash = hash, Salt = salt };
    }
}