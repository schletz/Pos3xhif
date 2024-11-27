namespace Preprocessor;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var preprocessor = AsciidocPreprocessor.FromFile(args[0]);
            preprocessor.AddMacroProcessor("example_macro", ExampleMacroProcessor);

            // Lädt den GPT Client. Deaktiviere diese Zeilen, wenn du den GPT Client nicht verwenden möchtest.
            var client = await ChatGptClient.FromKeyfile("chatgpt_key.txt");
            await client.EnableCache("chatgpt_cache.json");
            preprocessor.AddAsyncMacroProcessor("chatgpt_prompt", client.ChatCptMacroProcessor);

            var newContent = await preprocessor.Process();
            Console.WriteLine(newContent);
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.InnerException?.Message ?? e.Message);
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
<img src=""{target}"" width=""{attributes[0]}"" height=""{attributes[1]}"" style=""width: {attributes["css_width"]}"">
----
";
    }
}
