using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtensionDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime myDate = new DateTime(2019, 11, 2);
            Console.WriteLine($"{myDate} ist am Wochenende? {DateTimeHelpers.IsWeekend(myDate)}");
            Console.WriteLine($"{myDate} ist am Wochenende? {myDate.IsWeekend()}");

            ConsoleLogger myLogger = new ConsoleLogger();
            myLogger.LogUppercase("Hello!");
        }
    }
}
