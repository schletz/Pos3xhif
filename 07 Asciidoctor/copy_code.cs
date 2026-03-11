#:property PublishAot=false
#:property PublishSingleFile=true
#:property SelfContained=false
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

public class Program
{
    private static readonly HashSet<string> excludeDirs = new(StringComparer.OrdinalIgnoreCase)
        { "bin", "obj", "node_modules" };

    // Siehe https://github.com/rouge-ruby/rouge/blob/main/docs/Languages.md
    private static readonly Dictionary<string, string> sourceTypes = new(StringComparer.OrdinalIgnoreCase) {
        { "cs", "[source,csharp]" },
        { "csproj", "[source,xml]" },
        { "java", "[source,java]" },
        { "rb", "[source,ruby]" },
        { "json", "[source,json]" },
        { "js", "[source,javascript]" },
        { "ts", "[source,typescript]" },
        { "jsx", "[source,jsx]" },
        { "tsx", "[source,tsx]" },
        { "py", "[source,python]" },
        { "txt", "[source]" },
        { "xml", "[source,xml]" },
        { "adoc", "[source,asciidoc]" },
        { "md", "[source,markdown]" },
        { "cmd", "[source]" },
        { "sh", "[source,bash]" },
        { "sql", "[source,sql]" },
        { "yaml", "[source,yaml]" },
        { "puml", "[source]" },
        { "docx", "" },
    };

    private static Dictionary<string, Action<Stream, string>> documentProcessors = new(StringComparer.OrdinalIgnoreCase)
    {
        { "docx",  ReadWordXml}
    };

    private static Regex? extensionExp;
    private static ReadOnlySpan<char> DividerBytes => "----------";
    private static ReadOnlySpan<char> MaxEquals => "======";
    private static string DefaultSource => "[source]";
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (args.Length < 1)
        {
            Console.Error.WriteLine("Benutzung: copy_files <ordnername> [<erweiterungen-regex>]");
            return;
        }

        try
        {
            using var outStream = Console.OpenStandardOutput();
            var (targetDir, newExtensionExp) = ParseArgs(args);
            extensionExp = newExtensionExp;

            WriteLineToStream(outStream, $"= Listing of {targetDir}");
            WriteLineToStream(outStream, ":source-highlighter: rouge");
            WriteLineToStream(outStream);
            WriteLineToStream(outStream, $"Created: {DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}");
            WriteLineToStream(outStream);

            WalkDirectory(targetDir, outStream);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    static (string targetDir, Regex? extensionExp) ParseArgs(string[] args)
    {
        var result = args.Length switch
        {
            2 => (args[0], new Regex($"^({args[1]})$", RegexOptions.IgnoreCase | RegexOptions.Compiled)),
            1 => (args[0], null),
            _ => throw new ArgumentException("Invalid Parameters")
        };

        if (!Directory.Exists(result.Item1))
            throw new ApplicationException($"Fehler: Der Ordner '{result.Item1}' existiert nicht.");

        return result;
    }

    static void WalkDirectory(string directory, Stream outStream, int depth = 0)
    {
        // Das Verzeichnis mit einem = pro Tiefe schreiben.
        // Beispiel: === Test/testsub
        int equalsCount = Math.Min(5, depth + 2);
        WriteLineToStream(outStream, MaxEquals[..equalsCount], " ", directory);
        WriteLineToStream(outStream);

        ProcessDirectory(directory, outStream, depth);

        foreach (var dir in Directory.EnumerateDirectories(directory))
        {
            ReadOnlySpan<char> dirName = Path.GetFileName(dir.AsSpan());
            if (dirName.StartsWith('.') ||
                excludeDirs.GetAlternateLookup<ReadOnlySpan<char>>().Contains(dirName))
                continue;

            WalkDirectory(dir, outStream, depth + 1);
        }
    }

    static void ProcessDirectory(string directory, Stream outStream, int depth)
    {
        try
        {
            // Den Dateinamen mit der Anzahl von = (1 je Tiefe) schreiben.
            // Beispiel: ==== Test/test2/test3/testfile.txt
            int equalsCount = Math.Min(6, depth + 3);
            ReadOnlySpan<char> equalsSpan = MaxEquals[..equalsCount];

            var dirInfo = new DirectoryInfo(directory);
            foreach (var f in dirInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
            {
                if (f.Length >= 1_048_576) continue; // Überspringe große Dateien
                ReadOnlySpan<char> ext = f.Extension.AsSpan().TrimStart('.');
                if (extensionExp?.IsMatch(ext) == false) continue;
                if (!sourceTypes.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(ext, out var sourceType))
                {
                    if (extensionExp is null) continue;
                    sourceType = DefaultSource;
                }
                try
                {
                    // Die Anzahl an = und den Dateinamen schreiben.
                    WriteLineToStream(outStream, equalsSpan, " ", f.FullName);
                    WriteLineToStream(outStream);
                    WriteLineToStream(outStream, sourceType);
                    WriteLineToStream(outStream, DividerBytes);
                    if (documentProcessors.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(ext, out var processor))
                    {
                        processor(outStream, f.FullName);
                    }
                    else
                    {
                        using (var inStream = File.OpenRead(f.FullName))
                            inStream.CopyTo(outStream);
                    }
                    WriteLineToStream(outStream, Environment.NewLine, DividerBytes, Environment.NewLine);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"[Fehler beim Lesen von {f.FullName}: {e.Message}]");
                }
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"[Fehler beim Durchsuchen des Verzeichnisses: {e.Message}]");
        }
    }
    static void WriteLineToStream(Stream stream) => WriteLineToStream(stream, default, default, default);
    static void WriteLineToStream(Stream stream, ReadOnlySpan<char> part1) => WriteLineToStream(stream, part1, default, default);
    static void WriteLineToStream(Stream stream, ReadOnlySpan<char> part1, ReadOnlySpan<char> part2, ReadOnlySpan<char> part3)
    {
        int maxByteCount = Encoding.UTF8.GetMaxByteCount(part1.Length + part2.Length + part3.Length + 2);
        Span<byte> buffer = stackalloc byte[maxByteCount];
        int pos = Encoding.UTF8.GetBytes(part1, buffer);
        pos += Encoding.UTF8.GetBytes(part2, buffer[pos..]);
        pos += Encoding.UTF8.GetBytes(part3, buffer[pos..]);
        pos += Encoding.UTF8.GetBytes(Environment.NewLine, buffer[pos..]);
        stream.Write(buffer[..pos]);
    }

    static void ReadWordXml(Stream outStream, string filename)
    {
        var encoder = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);        
        using var archive = ZipFile.OpenRead(filename);
        foreach (var entry in archive.Entries)
            if (entry.FullName.StartsWith("word/document.xml"))
            {
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(outStream, encoder, leaveOpen: true);
                var settings = new XmlReaderSettings
                {
                    IgnoreComments = true,
                    IgnoreProcessingInstructions = true
                };
                using var reader = XmlReader.Create(entryStream, settings);
                bool whitespaceWritten = false;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Prefix == "w" && reader.LocalName == "p" && !whitespaceWritten)
                        {
                            writer.WriteLine();
                            whitespaceWritten = true;
                            continue;
                        }
                        if (reader.Prefix == "w" && reader.LocalName == "tab" && !whitespaceWritten)
                        {
                            writer.Write(' ');
                            whitespaceWritten = true;
                            continue;
                        }
                        continue;
                    }
                    if (reader.NodeType == XmlNodeType.Text)
                    {
                        writer.Write(reader.Value);
                        whitespaceWritten = false;
                        continue;
                    }
                }
                writer.Flush();
                break;
            }
    }
}
