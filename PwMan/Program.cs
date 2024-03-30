namespace PwMan;
using System;
using System.IO;
using System.Text;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to PwMan");
        User newUser = new User("fawfawf", "aaaa");
        Console.WriteLine(newUser.Nickname);
        Console.WriteLine(newUser.Password);
        
        // Check if the directory doesn't exist
        //string currentPath = Directory.GetCurrentDirectory();
        //string folderPath = $"{currentPath}/user_files/";
        //Console.WriteLine(currentPath);
        //Console.WriteLine(folderPath);
        
        //bool test = newUser.HasFile();
        //Console.WriteLine(test);
        //Console.WriteLine(newUser.CreateFile());
        /*if (!Directory.Exists(folderPath))
        {
            // Create the directory
         Directory.CreateDirectory(folderPath);
         Console.WriteLine("Folder created successfully.");
        }
        else
        {
          Console.WriteLine("Folder already exists.");
        }*/
        string testingPassword01 = GeneratePassword(10);
        string testingPassword64 = GeneratePassword();
        Console.WriteLine(testingPassword01);
        Console.WriteLine(testingPassword64);
        Console.WriteLine("please make a choice");
        int userInput = Convert.ToInt32(Console.ReadLine());
        switch (userInput)
        {
            case 1:
                Console.WriteLine("neco");
                break;
        }
    }

    private static string GeneratePassword(int passwordLength = 64)
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