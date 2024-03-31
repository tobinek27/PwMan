namespace PwMan;
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

    public static bool CreateLoginFile(string username, string password)
    {
        string currentPath = Directory.GetCurrentDirectory();
        string filePath = $"{currentPath}/user_logins/{username}";
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
    
    public User(string nickname, string password)
    {
        this.nickname = nickname;
        this.password = password;
    }
}