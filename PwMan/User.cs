namespace PwMan;

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

    public User(string nickname, string password)
    {
        this.nickname = nickname;
        this.password = password;
    }
}