namespace Preprocessor
{
    public class Logger
    {
        public static void LogInfo(string message) => Log($"[INFO] {message}");
        public static void LogError(string message)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Log($"[ERROR] {message}");
            Console.ForegroundColor = color;
        }
        public static void Log(string message)
        {
            // Auf stderr ausgeben, damit die Ausgabe nicht in die Ausgabe des Preprocessors gemischt wird.
            Console.Error.WriteLine(message);
        }
    }
}
