namespace Preprocessor;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var preprocessor = AsciidocPreprocessor.FromFile(args[0]);
            preprocessor.AddMacroProcessor("example_macro", ExampleMacroProcessor);

            // Lädt den GPT Client. Er ist nur aktiv, wenn ein Keyfile mit dem API Key existiert.
            if (File.Exists("chatgpt_key.txt"))
            {
                var client = await ChatGptClient.Create(keyfile: "chatgpt_key.txt", cachefile: "chatgpt_cache.json");
                preprocessor.AddAsyncMacroProcessor("chatgpt_prompt", client.ChatCptMacroProcessor);
            }
            var newContent = await preprocessor.Process();
            Console.WriteLine(newContent);
            return 0;
        }
        catch (Exception e)
        {
            Logger.LogError(e.InnerException?.Message ?? e.Message);
            return 1;
        }
    }

    /// <summary>
    /// Beispiel für einen Makroprozessor, der ein Bildmakro in HTML umwandelt.
    /// Aufruf: example_macro::test.jpg[300,200,css_width=value]
    /// </summary>
    static string ExampleMacroProcessor(string target, Attributes attributes, Dictionary<string, string> globalVariables)
    {
        return @$"
[source,html]
----
<img src=""{target}"" width=""{attributes[0]}"" height=""{attributes[1]}"" style=""width: {attributes["scale"]}"">
----
";
    }
}
