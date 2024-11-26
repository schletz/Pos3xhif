namespace Preprocessor;

public class Program
{

    public static async Task Main(string[] args)
    {
        try 
        {
            var preprocessor = AsciidocPreprocessor.FromFile(args[0]);
            var client = await ChatGptClient.FromKeyfile("chatgpt_key.txt");
            preprocessor.AddMacroProcessor("example_macro", ExampleMacroProcessor);
            preprocessor.AddMacroProcessor("chatgpt_prompt", client.ChatCptMacroProcessor);
            var newContent = await preprocessor.Process();
            Console.WriteLine(newContent);
        }
        catch(Exception e)
        {
            Console.Error.WriteLine(e.InnerException?.Message ?? e.Message);
        }
    }

    static Task<string> ExampleMacroProcessor(string target, Attributes attributes, Dictionary<string, string> globalVariables)
    {
        return Task.FromResult(@$"
.variables
----
{System.Text.Json.JsonSerializer.Serialize(globalVariables)}
----

.target
----
{target}
----

.attributesArray
----
{System.Text.Json.JsonSerializer.Serialize(attributes.AttributesArray)}
----

.namedAttributes
----
{System.Text.Json.JsonSerializer.Serialize(attributes.NamedAttributes)}
----
");
    }
}
