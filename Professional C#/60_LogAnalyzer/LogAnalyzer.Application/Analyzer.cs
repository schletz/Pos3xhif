using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Application
{
    public class Analyzer
    {
        private readonly string _filename;

        public Analyzer(string filename)
        {
            _filename = filename;
        }

        /// <summary>
        /// Schreibt eine Logdatei mit der angegebenen Anzahl an Datensätzen.
        /// </summary>
        public void WriteDemoFile(int lines)
        {
            Randomizer.Seed = new Random(1014);
            var faker = new Faker("de");
            var types = new string[] { "GET", "POST", "PUT", "DELETE" };
            var ips = new string[] { "0.0.0.1", "0.0.0.2", "0.0.0.3", "0.0.0.4" };

            using var streamWriter = new StreamWriter(_filename, append: false, encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            for (int i = 0; i < lines; i++)
            {
                var logEntry = new LogEntry(
                    Timestamp: new DateTime(2021, 1, 1).AddSeconds(faker.Random.Int(0, 9 * 30 * 86400)),
                    Ip: faker.Random.ListItem(ips),
                    RequestType: faker.Random.ListItem(types),
                    RequestUrl: faker.Internet.UrlRootedPath());
                streamWriter.Write(logEntry.ToString());
                streamWriter.Write("\r\n");                 // CR + LF bei jederm OS schreiben.
            }
        }

        /// <summary>
        /// Sucht in der Logdatei nach einer IP Adresse und gibt die Datensätze als LogEntries zurück.
        /// Diese Methode liest zuerst alle Daten in den Hauptspeicher als String Array und verwendet
        /// dann LINQ, um die Datensätze zu filtern.
        /// </summary>
        public List<LogEntry> FindIpLinq(string ip)
        {
            var content = File.ReadAllLines(_filename, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            return content
                .Select(line =>
                {
                    var cells = line.Split(";");
                    return new LogEntry(DateTime.Parse(cells[0]), cells[1], cells[2], cells[3]);
                })
                .Where(entry => entry.Ip == ip)
                .ToList();
        }

        /// <summary>
        /// Sucht in der Logdatei nach einer IP Adresse und gibt die Datensätze als LogEntries zurück.
        /// Diese Methode liest nicht alle Daten in den Speicher, sondern liest aus einem Stream Zeile
        /// für Zeile aus. Nur bei Bedarf wird ein LogEntry erstellt.
        /// </summary>
        public List<LogEntry> FindIpStream(string ip)
        {
            var results = new List<LogEntry>();

            using var streamReader = new StreamReader(_filename, encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            while (true)
            {
                var line = streamReader.ReadLine();
                if (line is null) { break; }
                var cells = line.Split(";");
                if (cells[1] == ip)
                {
                    results.Add(new LogEntry(DateTime.Parse(cells[0]), cells[1], cells[2], cells[3]));
                }
            }
            return results;
        }

        /// <summary>
        /// Sucht in der Logdatei nach einer IP Adresse und gibt die Datensätze als LogEntries zurück.
        /// Diese Methode liest Zeile für Zeile aus dem Stream und verwendet Spans, um die einzelne Zeile
        /// zu parsen. Die Zeile wird aber als String aus dem Stream gelesen.
        /// </summary>
        public List<LogEntry> FindIpSpan(string searchIp)
        {
            var results = new List<LogEntry>();
            var searchIpSpan = searchIp.AsSpan();
            using var streamReader = new StreamReader(_filename, encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            while (true)
            {
                var line = streamReader.ReadLine();
                if (line is null) { break; }
                var lineSpan = line.AsSpan();

                // Bis zum ersten ; lesen und den Zeitstempel als Span speichern.
                int found = lineSpan.IndexOf(';');
                var timestamp = lineSpan.Slice(0, found);
                lineSpan = lineSpan.Slice(found + 1);

                // Bis zum zweiten ; lesen und die IP als Span speichern.
                found = lineSpan.IndexOf(';');
                var ip = lineSpan.Slice(0, found);
                if (!ip.Equals(searchIpSpan, StringComparison.Ordinal)) { continue; }
                lineSpan = lineSpan.Slice(found + 1);

                // Bis zum dritten ; lesen und den Request Type als Span speichern.
                found = lineSpan.IndexOf(';');
                var requestType = lineSpan.Slice(0, found);
                lineSpan = lineSpan.Slice(found + 1);
                
                // Der Rest der Zeile ist die URL
                var requestUrl = lineSpan;

                // Das neue LogEntry erstellen. Hier muss in einen String umgewandelt werden.
                results.Add(new LogEntry(DateTime.Parse(timestamp), ip.ToString(), requestType.ToString(), requestUrl.ToString()));
            }
            return results;
        }

        /// <summary>
        /// Sucht in der Logdatei nach einer IP Adresse und gibt die Datensätze als LogEntries zurück.
        /// Diese Methode liest nicht zeilenweise, sondern 1 KB an Daten in einen Span. Dadurch wird kein
        /// String für die Zeile erstellt. Der Buffer wird dann nach Zeilenumbrüchen durchsucht.
        /// Verlangt Windows Zeilenumbrüche (CR+LF)
        /// </summary>
        public List<LogEntry> FindIpSpanBlock(string searchIp)
        {
            var results = new List<LogEntry>();
            var searchIpSpan = searchIp.AsSpan();
            var buffer = new char[1024].AsSpan();
            using var streamReader = new StreamReader(_filename, encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            int remaining = 0;
            while (true)
            {
                int count = streamReader.Read(buffer.Slice(remaining));
                if (count == 0) { break; }
                ReadOnlySpan<char> lineBuffer = buffer.Slice(0, count + remaining);
                while (true)
                {
                    int lineEnd = lineBuffer.IndexOf('\n');
                    if (lineEnd == -1) { break; }
                    var line = lineBuffer.Slice(0, lineEnd - 1);    // Windows CR+LF: CR nicht zur Zeile geben.
                    lineBuffer = lineBuffer.Slice(lineEnd + 1);

                    int found = line.IndexOf(';');
                    var timestamp = line.Slice(0, found);
                    line = line.Slice(found + 1);

                    found = line.IndexOf(';');
                    var ip = line.Slice(0, found);
                    if (!ip.Equals(searchIpSpan, StringComparison.Ordinal)) { continue; }
                    line = line.Slice(found + 1);

                    found = line.IndexOf(';');
                    var requestType = line.Slice(0, found);
                    line = line.Slice(found + 1);

                    var requestUrl = line;
                    results.Add(new LogEntry(DateTime.Parse(timestamp), ip.ToString(), requestType.ToString(), requestUrl.ToString()));
                }
                remaining = lineBuffer.Length;
                lineBuffer.CopyTo(buffer);                         // Restliche Daten an den Anfang des Buffers geben.

            }
            return results;
        }

    }
}
