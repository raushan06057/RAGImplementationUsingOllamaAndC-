using RAGImplementationAPI.Models;

namespace RAGImplementationAPI.Services;

public interface IRAGService
{
    Task<Document> AddDocumentAsync(string content, Dictionary<string, string>? metadata = null);
    Task<QueryResponse> QueryAsync(string query, int topK = 3, string model = "llama2");
}

public class RAGService : IRAGService
{
    private readonly IOllamaService _ollamaService;
    private readonly IVectorStore _vectorStore;
    private readonly ILogger<RAGService> _logger;

    public RAGService(
        IOllamaService ollamaService,
        IVectorStore vectorStore,
        ILogger<RAGService> logger)
    {
        _ollamaService = ollamaService;
        _vectorStore = vectorStore;
        _logger = logger;
    }

    public async Task<Document> AddDocumentAsync(string content, Dictionary<string, string>? metadata = null)
    {
        _logger.LogInformation("Adding document to vector store");

        var embedding = await _ollamaService.GetEmbeddingAsync(content);

        var document = new Document
        {
            Content = content,
            Metadata = metadata ?? new Dictionary<string, string>(),
            Embedding = embedding
        };

        await _vectorStore.AddDocumentAsync(document);

        _logger.LogInformation("Document added with ID: {DocumentId}", document.Id);

        return document;
    }

    public async Task<QueryResponse> QueryAsync(string query, int topK = 3, string model = "llama2")
    {
        _logger.LogInformation("Processing query: {Query}", query);

        var queryEmbedding = await _ollamaService.GetEmbeddingAsync(query);

        var relevantDocs = await _vectorStore.SearchAsync(queryEmbedding, topK);

        _logger.LogInformation("Retrieved {Count} relevant documents", relevantDocs.Count);

        var context = string.Join("\n\n", relevantDocs.Select((d, i) => $"Document {i + 1}:\n{d.Content}"));

        var prompt = $@"You are a helpful assistant. Use the following context to answer the question. If you cannot answer based on the context, say so.

Context:
{context}

Question: {query}

Answer:";

        var answer = await _ollamaService.GenerateAsync(prompt, model);

        var response = new QueryResponse
        {
            Answer = answer.Trim(),
            RetrievedDocuments = relevantDocs.Select(d => new RetrievedDocument
            {
                Content = d.Content,
                Similarity = CalculateSimilarity(queryEmbedding, d.Embedding!),
                Metadata = d.Metadata
            }).ToList()
        };

        return response;
    }

    private static float CalculateSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length)
            return 0;

        var dotProduct = 0f;
        var magnitudeA = 0f;
        var magnitudeB = 0f;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            magnitudeA += a[i] * a[i];
            magnitudeB += b[i] * b[i];
        }

        magnitudeA = MathF.Sqrt(magnitudeA);
        magnitudeB = MathF.Sqrt(magnitudeB);

        if (magnitudeA == 0 || magnitudeB == 0)
            return 0;

        return dotProduct / (magnitudeA * magnitudeB);
    }
}
