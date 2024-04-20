using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public class HashSalt
{
    public string Hash { get; set; }
    public string Salt { get; set; }
    public string Password { get; set; }
    
    /*public static HashSalt GenerateSaltedHash(int size, string password)
    {
        var saltBytes = new byte[size];
        var random = new Random();
        random.NextBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);

        using (var sha256 = SHA256.Create())
        {
            byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
            var hashPassword = Convert.ToBase64String(hashedBytes);

            HashSalt hashSalt = new HashSalt { Hash = hashPassword, Salt = salt };
            return hashSalt;
        }
    }*/

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

            HashSalt hashSalt = new HashSalt { Password = hashedPassword, Salt = salt };
            return hashSalt;
        }
    }
    
    // Method to serialize HashSalt object to JSON string
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    // Method to deserialize HashSalt object from JSON string
    public static HashSalt FromJson(string json)
    {
        return JsonConvert.DeserializeObject<HashSalt>(json);
    }
    
    public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
    {
        // Convert the stored salt from base64 string to byte array
        var saltBytes = Convert.FromBase64String(storedSalt);

        // Create an instance of the SHA-256 hash algorithm
        using (var sha256 = SHA256.Create())
        {
            // Compute the hash of the combined password and salt bytes using SHA-256
            byte[] combinedBytes = Encoding.UTF8.GetBytes(enteredPassword + Convert.ToBase64String(saltBytes));
            byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
    
            // Convert the hashed bytes to a base64 string
            var generatedHash = Convert.ToBase64String(hashedBytes);
    
            // Compare the generated hash with the stored hash to verify the password
            Console.WriteLine($"Generated Hash: {generatedHash}");
            Console.WriteLine($"Stores Hash: {storedHash}");
            return generatedHash == storedHash;
        }
    }

}