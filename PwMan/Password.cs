namespace PwMan;

using Newtonsoft.Json;

/// <summary>
/// Represents a password with a tag.
/// </summary>
public class Password
{
    public string Tag { get; set; }
    public string PasswordValue { get; set; }

    /// <summary>
    /// Writes the current password object to a JSON file.
    /// </summary>
    /// <param name="filePath">The file path to write to.</param>
    /// <returns>True if the password is saved successfully; otherwise, false.</returns>
    private bool WriteToJson(string filePath)
    {
        List<Password> existingData = ReadJson(filePath);
        // this line overrides an already existing password with a matching tag
        existingData.RemoveAll(p => p.Tag == Tag);
        existingData.Add(this);

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
        string jsonData = JsonConvert.SerializeObject(existingData, settings);
        File.WriteAllText(filePath, jsonData);
        return true;
    }

    /// <summary>
    /// Reads a list of passwords from a JSON file.
    /// </summary>
    /// <param name="filePath">The file path to read from.</param>
    /// <returns>A list of Password objects.</returns>
    public static List<Password> ReadJson(string filePath)
    {
        if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
        {
            return new List<Password>();
        }

        using (StreamReader sr = new StreamReader(filePath))
        {
            string json = sr.ReadToEnd();
            return JsonConvert.DeserializeObject<List<Password>>(json);
        }
    }

    /// <summary>
    /// Generates a password for the current user.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    public static void GeneratePassword(User currentUser)
    {
        while (true)
        {
            Console.WriteLine("Input a desired password length: (default=64) or enter 'q' to quit.");
            string input = Console.ReadLine();
            input = Program.CleanseInput(input);

            if (input.ToLower() == "q")
            {
                break;
            }

            if (int.TryParse(input, out int passwordLength) && passwordLength >= 8 &&
                passwordLength <= 256 || string.IsNullOrEmpty(input))
            {
                Console.WriteLine("Generating a password...");
                if (string.IsNullOrEmpty(input))
                {
                    passwordLength = 64;
                }

                string password = PasswordMethods.GeneratePassword(passwordLength);
                Console.WriteLine(password);

                Console.WriteLine("Do you wish to save the password?");
                Console.WriteLine("[press any key to proceed, or 'n' to abort]");
                var keyInfo = Console.ReadKey();
                char userInput = char.ToLower(keyInfo.KeyChar);
                if (userInput == 'n')
                {
                    Console.WriteLine("Okay, not saving the password.");
                    continue;
                }

                string tag;
                do
                {
                    Console.Clear();
                    Console.WriteLine("'q' to exit");
                    Console.WriteLine("Tag may contain alphanumeric characters (and a '/')");
                    Console.WriteLine("Please enter a tag:");
                    tag = Console.ReadLine().Trim();

                    tag = Program.CleanseInput(tag);
                } while (!Program.TagIsValid(tag));

                if (tag == "q")
                {
                    break;
                }

                Password passwordToSave = new Password(tag, password);
                Program.StatusMessage = passwordToSave.WriteToJson(currentUser.GetPwFilePath())
                    ? "Password saved successfully."
                    : "Failed to save the password.";
            }
            else
            {
                Console.WriteLine("Invalid password provided, please enter a valid length (8-256).");
            }
        }
    }

    /// <summary>
    /// Displays search results based on the provided search tag
    /// </summary>
    /// <param name="searchResults">List of Password objects.</param>
    /// <param name="searchTag">Tag to use during the search.</param>
    private static void DisplaySearchResults(List<Password> searchResults, string searchTag)
    {
        if (searchResults.Count > 0)
        {
            Console.Clear();
            Console.WriteLine($"Found {searchResults.Count} password(s) with tag '{searchTag}':");
            foreach (var password in searchResults)
            {
                Console.WriteLine($"Tag: {password.Tag}, Password: {password.PasswordValue}");
                Program.StatusMessage = $"Found password for tag {password.Tag}.";
            }

            return;
        }

        Console.Clear();
    }

    /// <summary>
    /// Searches for passwords by tag and displays them.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    public static void SearchForPasswords(User currentUser)
    {
        Console.Clear();
        string searchTag = "";
        while (searchTag.ToLower() != "q")
        {
            Console.WriteLine("Enter a tag to search for (type 'q' to quit):");
            searchTag = Console.ReadLine();
            searchTag = Program.CleanseInput(searchTag);

            List<Password>
                passwordsOfUser = currentUser.FetchUserPasswords();
            List<Password> searchResults = SearchPasswords(passwordsOfUser, searchTag);

            DisplaySearchResults(searchResults, searchTag);
        }
    }

    /// <summary>
    /// Searches for passwords by tag.
    /// </summary>
    /// <param name="passwords">List of passwords.</param>
    /// <param name="tag">Tag to use during the search.</param>
    /// <returns>A list of Password objects.</returns>
    private static List<Password> SearchPasswords(List<Password> passwords, string tag)
    {
        List<Password> searchResults = new List<Password>();

        foreach (var password in passwords)
        {
            if (password.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase))
            {
                searchResults.Add(password);
            }
        }

        return searchResults;
    }

    /// <summary>
    /// Deletes a password based on the input tag.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    public static void DeletePassword(User currentUser)
    {
        Console.WriteLine("Starting password deletion process...");
        string input = "";
        while (input != "q")
        {
            List<Password> passwordList = currentUser.FetchUserPasswords();
            currentUser.DisplayPasswordsForDeletion();
            Console.WriteLine("Input the tag of a password you wish to delete ('q' to exit):");
            input = Console.ReadLine();
            input = Program.CleanseInput(input);

            if (input.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                break; // exit the loop if the user inputs 'q'
            }

            if (!passwordList.Any(p => p.Tag.Equals(input, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("No password found with the given tag.");
                continue; // skip to the next while-loop iteration without deleting anything
            }

            List<Password> passwordListAfterDelete =
                DeletePasswordsByTag(passwordList, input, currentUser.GetPwFilePath());
            Console.WriteLine("Password(s) deleted successfully. Remaining passwords:");
            foreach (var per in passwordListAfterDelete)
            {
                Console.WriteLine($"Tag: {per.Tag}, Password: {per.PasswordValue}");
            }
        }
    }

    /// <summary>
    /// Deletes passwords based on the provided tag, and displays the updated Password list.
    /// </summary>
    /// <param name="passwords">List of Passwords to delete from.</param>
    /// <param name="tag">Tag to use for the deletion process.</param>
    /// <param name="filePath">File path to the password file.</param>
    /// <returns>A list of updated passwords after the deletion.</returns>
    private static List<Password> DeletePasswordsByTag(List<Password> passwords, string tag, string filePath)
    {
        passwords.RemoveAll(p => p.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase));

        // write the updated passwords list to the file
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
        string jsonData = JsonConvert.SerializeObject(passwords, settings);
        File.WriteAllText(filePath, jsonData);

        return passwords;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Password"/> class with the specified tag and passwordValue.
    /// </summary>
    /// <param name="tag">Tag part of the password.</param>
    /// <param name="passwordValue">The actual password value.</param>
    public Password(string tag, string passwordValue)
    {
        Tag = tag;
        PasswordValue = passwordValue;
    }
}