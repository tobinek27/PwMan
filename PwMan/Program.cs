namespace PwMan;
using System;
using System.IO;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to PwMan");
        User newUser = new User("fawfawf", "aaaa");
        Console.WriteLine(newUser.Nickname);
        Console.WriteLine(newUser.Password);
        
        // Check if the directory doesn't exist
        string currentPath = Directory.GetCurrentDirectory();
        string folderPath = $"{currentPath}/user_files/";
        Console.WriteLine(currentPath);
        Console.WriteLine(folderPath);

        bool test = newUser.HasFile();
        Console.WriteLine(test);
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
    }
}