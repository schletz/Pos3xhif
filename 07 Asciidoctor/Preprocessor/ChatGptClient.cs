using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Preprocessor
{
    public class ChatGptClient
    {
        private readonly string apiKey;

        public static async Task<ChatGptClient> FromKeyfile(string filename)
        {
            if (!File.Exists(filename))
                throw new Exception($"API Keyfile {filename} nicht gefunden.");
                
            var apiKey = await File.ReadAllTextAsync(filename);
            return new ChatGptClient(apiKey.Trim());
        }

        public ChatGptClient(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public async Task<string> ChatCptMacroProcessor(string target, Attributes attributes, Dictionary<string, string> globalVariables)
        {
            if (attributes.AttributesArray.Length == 0 || string.IsNullOrEmpty(attributes[0]))
                return "[ERROR] No prompt provided";
            int maxTokens = attributes.NamedAttributes.TryGetValue("max_tokens", out var maxTokensStr) ? int.Parse(maxTokensStr) : 50;
            decimal temperature = attributes.NamedAttributes.TryGetValue("temperature", out var temperatureStr)
                ? decimal.Parse(temperatureStr, System.Globalization.CultureInfo.InvariantCulture) : 0.7M;
            string model = attributes.NamedAttributes.TryGetValue("model", out var modelStr) ? modelStr : "gpt-4o";
            return await PerformRequest(attributes[0], maxTokens, temperature, model);
        }
        public async Task<string> PerformRequest(string prompt, int maxTokens, decimal temperature, string model)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
            var payload = new
            {
                model,
                messages = new[] {
                    new {
                        role = "user",
                        content = prompt
                    }
                },
                max_tokens = maxTokens,
                temperature
            };
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return $"[ERROR] Request failed. HTTP Status {response.StatusCode}";
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonDocument.Parse(responseContent).RootElement;
            if (!responseJson.TryGetProperty("choices", out var choices))
                return "[ERROR] No choices in response";
            if (!choices[0].TryGetProperty("message", out var message))
                return "[ERROR] No message in choices";
            if (!message.TryGetProperty("content", out var content))
                return "[ERROR] No content in message";
            return content.GetString() ?? "[ERROR] No message string in choices";
        }
    }
}
