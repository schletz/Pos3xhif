using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionDemo
{
    interface ILogger
    {
        void Log(string value);
    }

    class ConsoleLogger : ILogger
    {
        public void Log(string value)
        {
            Console.WriteLine(value);
        }
    }

    /// <summary>
    /// Erweitert das Interface ILogger. Wenn wir das Interface selbst geschrieben haben, sollten wir
    /// auch weiterhin abstrakte Klassen verwenden. Es dient nur zur Demonstration, wie Interfaces
    /// aus dem Framework erweitert werden können.
    /// </summary>
    static class LoggerExtensions
    {
        public static void LogUppercase(this ILogger logger, string value)
        {
            logger.Log(value.ToUpper());
        }
    }
}
