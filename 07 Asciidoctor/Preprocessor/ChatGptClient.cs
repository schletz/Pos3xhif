using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Preprocessor
{
    /// <summary>
    /// Sendet Prompts an die ChatGPT API und gibt die Antwort zurück.
    /// </summary>
    public class ChatGptClient
    {
        public record ChatGptMessage(string Role, string Content);
        private readonly string _apiKey;
        private readonly string _model;
        private readonly static Regex _mdCodeblock = new Regex(@"^```\w*", RegexOptions.Compiled | RegexOptions.Multiline);
        private Dictionary<long, List<ChatGptMessage>> _cache = new();
        private string _cacheFile = string.Empty;
        private List<ChatGptMessage> _chatGptMessages = new();
        /// <summary>
        /// Liest eine Datei mit dem API Key ein und erstellt einen neuen ChatGptClient.
        /// </summary>
        public static async Task<ChatGptClient> Create(string filename, string model = "gpt-4o")
        {
            if (!File.Exists(filename))
                throw new ServiceException($"API Keyfile {filename} nicht gefunden.");

            var apiKey = (await File.ReadAllTextAsync(filename)).Trim();
            Logger.LogInfo($"Using ChatGPT API key from {apiKey}");
            return new ChatGptClient(apiKey, model);
        }

        /// <summary>
        /// Liest das Cachefile.
        /// </summary>
        public async Task EnableCache(string cacheFile)
        {
            _cacheFile = cacheFile;
            if (!File.Exists(cacheFile)) return;
            string content = await File.ReadAllTextAsync(cacheFile, new UTF8Encoding(false));
            var cache = JsonSerializer.Deserialize<Dictionary<long, List<ChatGptMessage>>>(content);
            if (cache is null) return;
            _cache = cache;
            Logger.LogInfo($"Using ChatGPT cache in {cacheFile}");
        }
        private ChatGptClient(string apiKey, string model)
        {
            _apiKey = apiKey;
            _model = model;
            _chatGptMessages.Add(
                new ChatGptMessage("system", @"Du spricht mit einem Computerprogramm, das nur AsciiDoc versteht.
Es dürfen keine anderen Inhalte übermittelt werden.
Source code soll in einen [source] Block mit der entsprechenden Programmiersprache stehen und mit der Option linenums versehen werden.
Achte darauf, dass Callouts immer in spitzen Klammern stehen.
PlantUML Diagramme sind in einem Codeblock mit [plantuml,,svg] ohne Callouts zu übermitteln.
Es darf kein Titel (mit einem = beginnend) übermittelt werden."));
        }
        public bool HasCache => !string.IsNullOrEmpty(_cacheFile);
        public async Task<string> ChatCptMacroProcessor(string target, Attributes attributes, Dictionary<string, string> globalVariables)
        {
            if (attributes.AttributesArray.Length == 0 || string.IsNullOrEmpty(attributes[0]))
                throw new ServiceException("[ERROR] No prompt provided");
            int maxTokens = attributes.NamedAttributes.TryGetValue("max_tokens", out var maxTokensStr) ? int.Parse(maxTokensStr) : 50;
            decimal temperature = attributes.NamedAttributes.TryGetValue("temperature", out var temperatureStr)
                ? decimal.Parse(temperatureStr, System.Globalization.CultureInfo.InvariantCulture) : 0.7M;
            bool saveMessage = attributes.NamedAttributes.TryGetValue("save_message", out var saveMessageStr)
                ? bool.Parse(saveMessageStr) : false;
            return await PerformRequest(attributes[0], maxTokens, temperature, saveMessage);
        }
        public async Task<string> PerformRequest(string prompt, int maxTokens, decimal temperature, bool saveMessage, string role = "user")
        {
            if (TryGetCache(prompt, out var chatGptMessages))
            {
                Logger.LogInfo($"Answering from cache: {prompt.Substring(0, Math.Min(prompt.Length, 60))}");
                if (saveMessage) _chatGptMessages = chatGptMessages;
                return chatGptMessages.Last().Content;
            }
            chatGptMessages.AddRange(_chatGptMessages);
            chatGptMessages.Add(new ChatGptMessage(role, prompt));
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            var payload = new
            {
                model = _model,
                messages = chatGptMessages.Select(m => new { role = m.Role, content = m.Content }),
                max_tokens = maxTokens,
                temperature
            };
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            Logger.LogInfo($"Prompting: {prompt.Substring(0, Math.Min(prompt.Length, 60))}");
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new ServiceException($"[ERROR] Request failed. HTTP Status {response.StatusCode}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent).RootElement;
            if (!responseJson.TryGetProperty("choices", out var choices))
                throw new ServiceException("[ERROR] No choices in response");
            if (!choices[0].TryGetProperty("message", out var message))
                throw new ServiceException("[ERROR] No message in choices");
            if (!message.TryGetProperty("content", out var content))
                throw new ServiceException("[ERROR] No content in message");
            string contentStr = content.GetString() ?? "[ERROR] No message string in choices";
            contentStr = _mdCodeblock.Replace(contentStr, "");
            chatGptMessages.Add(new ChatGptMessage("assistant", contentStr));
            await AddToCache(prompt, chatGptMessages);
            if (saveMessage)
                _chatGptMessages = chatGptMessages;
            return contentStr;
        }

        private long CalculateStringHash(string content)
        {
            string cleanedContent = string.Concat(content.Where(c => !char.IsWhiteSpace(c)));
            // Hash with SHA256 and convert last 8 bytes to long
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(cleanedContent));
            return BitConverter.ToInt64(hash, hash.Length - 8);
        }

        private bool TryGetCache(string prompt, out List<ChatGptMessage> request)
        {
            if (!HasCache) { request = new(); return false; }
            long hash = CalculateStringHash(prompt);
            if (!_cache.TryGetValue(hash, out var query)) { request = new(); return false; }
            request = query;
            return true;
        }
        private async Task AddToCache(string prompt, List<ChatGptMessage> chatGptMessages)
        {
            if (!HasCache) return;
            long hash = CalculateStringHash(prompt);
            _cache[hash] = chatGptMessages;
            await File.WriteAllTextAsync(_cacheFile, JsonSerializer.Serialize(_cache), new UTF8Encoding(false));
        }
    }
}
