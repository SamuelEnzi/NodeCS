using System;

namespace NodeCS
{
    public static class Log
    {
        public static void WriteLine(object message = null, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            
            Console.WriteLine(message?.ToString());
            Console.ResetColor();
        }
    }
}
