namespace Preprocessor
{
    public class Logger
    {
        public static void LogInfo(string message) => Log($"[INFO] {message}");
        public static void Log(string message)
        {
            Console.Error.WriteLine(message);
        }
    }
}
