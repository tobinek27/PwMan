using System.IO;
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

            //HashSalt hashSalt = new HashSalt { Password = hashedPassword, Salt = salt };
            //HashSalt hashSalt = new HashSalt(hashedPassword, salt);
            HashSalt hashSalt = CreateHashSalt(hashedPassword, salt);
            return hashSalt;
        }
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
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

    public static HashSalt CreatePasswordSalt(string password, string salt)
    {
        return new HashSalt { Password = password, Salt = salt };
    }

    public static HashSalt CreateHashSalt(string hash, string salt)
    {
        return new HashSalt { Hash = hash, Salt = salt };
    }
}