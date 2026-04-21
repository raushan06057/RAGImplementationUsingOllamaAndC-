namespace RAGImplementationAPI.Models;

public class OllamaGenerateRequest
{
    public string Model { get; set; } = "llama2";
    public string Prompt { get; set; } = string.Empty;
    public bool Stream { get; set; } = false;
}

public class OllamaEmbedRequest
{
    public string Model { get; set; } = "nomic-embed-text";
    public string Prompt { get; set; } = string.Empty;
}

public class OllamaGenerateResponse
{
    public string Response { get; set; } = string.Empty;
}

public class OllamaEmbedResponse
{
    public float[]? Embedding { get; set; }
}
