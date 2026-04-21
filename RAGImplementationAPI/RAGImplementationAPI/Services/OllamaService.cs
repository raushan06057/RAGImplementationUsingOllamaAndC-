using RAGImplementationAPI.Models;
using System.Text;
using System.Text.Json;

namespace RAGImplementationAPI.Services;

public interface IOllamaService
{
    Task<float[]> GetEmbeddingAsync(string text, string model = "nomic-embed-text");
    Task<string> GenerateAsync(string prompt, string model = "llama3.2");
}

public class OllamaService : IOllamaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaService> _logger;
    private readonly string _ollamaUrl;

    public OllamaService(HttpClient httpClient, IConfiguration configuration, ILogger<OllamaService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _ollamaUrl = configuration["Ollama:Url"] ?? "http://localhost:11434";
    }

    public async Task<float[]> GetEmbeddingAsync(string text, string model = "nomic-embed-text")
    {
        try
        {
            var request = new OllamaEmbedRequest
            {
                Model = model,
                Prompt = text
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending embedding request for text of length: {Length} characters", text.Length);
            var response = await _httpClient.PostAsync($"{_ollamaUrl}/api/embeddings", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Ollama API error: Status {StatusCode}, Response: {Response}", response.StatusCode, responseContent);
                throw new HttpRequestException($"Ollama API returned {response.StatusCode}: {responseContent}");
            }

            _logger.LogInformation("Successfully received embedding response");

            var embedResponse = JsonSerializer.Deserialize<OllamaEmbedResponse>(responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return embedResponse?.Embedding ?? Array.Empty<float>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting embedding from Ollama");
            throw;
        }
    }

    public async Task<string> GenerateAsync(string prompt, string model = "llama3.2")
    {
        try
        {
            var request = new OllamaGenerateRequest
            {
                Model = model,
                Prompt = prompt,
                Stream = false
            };

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_ollamaUrl}/api/generate", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var generateResponse = JsonSerializer.Deserialize<OllamaGenerateResponse>(responseContent, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return generateResponse?.Response ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating response from Ollama");
            throw;
        }
    }
}
