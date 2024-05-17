using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

/// <summary>
/// Represents a hashed password and its corresponding salt.
/// </summary>
public class HashSalt
{
    public string? Hash { get; set; }
    public string? Salt { get; set; }
    public string? Password { get; set; }


    /// <summary>
    /// Generates a salted hash for the given password.
    /// </summary>
    /// <param name="size">The size of the salt to generate.</param>
    /// <param name="password">The password to hash.</param>
    /// <returns>A <see cref="HashSalt"/> object containing the hash and salt.</returns>
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

    /// <summary>
    /// Deserializes a JSON string into a <see cref="HashSalt"/> object.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>A <see cref="HashSalt"/> object.</returns>
    public static HashSalt FromJson(string json)
    {
        return JsonConvert.DeserializeObject<HashSalt>(json);
    }

    /// <summary>
    /// Verifies the entered password against the stored hash and salt.
    /// </summary>
    /// <param name="enteredPassword">The password entered by the user.</param>
    /// <param name="storedHash">The stored hash of the password.</param>
    /// <param name="storedSalt">The stored salt used in hashing the password.</param>
    /// <returns><c>true</c> if the entered password matches the stored hash; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="HashSalt"/> class.
    /// </summary>
    private HashSalt()
    {
    }

    /// <summary>
    /// Creates a new <see cref="HashSalt"/> object with the specified hash and salt.
    /// </summary>
    /// <param name="hash">The hash of the password.</param>
    /// <param name="salt">The salt used in hashing the password.</param>
    /// <returns>A new <see cref="HashSalt"/> object.</returns>
    public static HashSalt CreateHashSalt(string hash, string salt)
    {
        return new HashSalt { Hash = hash, Salt = salt };
    }
}