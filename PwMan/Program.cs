﻿namespace PwMan;

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to PwMan");
        User newUser = new User("fawfawf", "aaaa");
        Console.WriteLine(newUser.Nickname);
        Console.WriteLine(newUser.Password);
        
        string testingPassword01 = PasswordMethods.GeneratePassword(10);
        string testingPassword64 = PasswordMethods.GeneratePassword();
        Console.WriteLine(testingPassword01);
        Console.WriteLine(testingPassword64);
        Console.WriteLine("please make a choice");
        Console.WriteLine("1 - load user passwords");
        Console.WriteLine("2 - sign up an account");
        int userInput = Convert.ToInt32(Console.ReadLine());
        switch (userInput)
        {
            case 1: // load user passwords
                Console.WriteLine("Please, input username:");
                string usernameInput = Console.ReadLine();
                if (User.HasFile(usernameInput))
                {
                    Console.WriteLine($"Input user password for {usernameInput}: ");
                    string passwordFromInput = Console.ReadLine();
                    Console.WriteLine(passwordFromInput);
                }
                else
                {
                    Console.WriteLine("to implement");
                }
                break;
            case 2: // sign up a new account
                Console.WriteLine("input a username to register");
                string usernameToRegister = Console.ReadLine();
                if (User.HasLoginFile(usernameToRegister))
                {
                    Console.WriteLine($"sorry, but the username '{usernameToRegister}' is already taken\r\nShutting down");
                    break;
                }
                Console.WriteLine($"input a password to be used for {usernameToRegister}");
                string inputPassword = Console.ReadLine();
                byte[] salt = PasswordMethods.GenerateSalt(usernameToRegister);
                string hashedPassword = PasswordMethods.HashPassword(inputPassword, salt);
                Console.WriteLine($"hashed password: {hashedPassword}");
                Console.WriteLine($"salt: {Convert.ToBase64String(salt)}");
                User.CreateLoginFile(usernameToRegister, hashedPassword);
                break;
        }
    }
    
}