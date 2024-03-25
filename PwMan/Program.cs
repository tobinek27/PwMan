namespace PwMan;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to PwMan");
        User newUser = new User("fawfawf", "aaaa");
        Console.WriteLine(newUser.Nickname);
        Console.WriteLine(newUser.Password);
    }
}