using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ideal_giggle
{
    public static class ConsolePrinter
    {

        public static void Print(string text, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void PrintLine(string text, ConsoleColor color = ConsoleColor.White)
        {
            Print(text, color);
            Console.WriteLine();
        }
    }
}
