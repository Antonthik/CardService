﻿using CardStorService.Utils;

namespace PasswordUtilsClient
{
    //Пример получения соли и токена из класса PasswordUtils
    internal class Program
    {
        static void Main(string[] args)
        {
            var result = PasswordUtils.CreatePasswordHash("12345");
            Console.WriteLine(result.passwordSalt);
            Console.WriteLine(result.passwordHash);
            Console.ReadKey(true);
        }
    }
}