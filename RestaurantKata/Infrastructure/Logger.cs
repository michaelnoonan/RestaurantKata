using System;

namespace RestaurantKata.Infrastructure
{
    public class Logger
    {
        public static void Debug(string message, params object[] values)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(message, values);
            Console.ResetColor();
        }

        public static void Info(string message, params object[] values)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message, values);
            Console.ResetColor();
        }

        public static void Warn(string message, params object[] values)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message, values);
            Console.ResetColor();
        }

        public static void Error(string message, params object[] values)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message, values);
            Console.ResetColor();
        }
    }
}