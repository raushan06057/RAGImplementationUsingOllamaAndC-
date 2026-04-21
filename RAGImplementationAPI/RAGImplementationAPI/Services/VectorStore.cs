using RAGImplementationAPI.Models;
using System.Numerics.Tensors;

namespace RAGImplementationAPI.Services;

public interface IVectorStore
{
    Task AddDocumentAsync(Document document);
    Task<List<Document>> SearchAsync(float[] queryEmbedding, int topK = 3);
    Task<List<Document>> GetAllDocumentsAsync();
}

public class InMemoryVectorStore : IVectorStore
{
    private readonly List<Document> _documents = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task AddDocumentAsync(Document document)
    {
        await _semaphore.WaitAsync();
        try
        {
            _documents.Add(document);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<Document>> SearchAsync(float[] queryEmbedding, int topK = 3)
    {
        await _semaphore.WaitAsync();
        try
        {
            var results = _documents
                .Where(d => d.Embedding != null)
                .Select(d => new
                {
                    Document = d,
                    Similarity = CosineSimilarity(queryEmbedding, d.Embedding!)
                })
                .OrderByDescending(x => x.Similarity)
                .Take(topK)
                .Select(x => x.Document)
                .ToList();

            return results;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<List<Document>> GetAllDocumentsAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return _documents.ToList();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length)
            return 0;

        var dotProduct = TensorPrimitives.Dot(a.AsSpan(), b.AsSpan());
        var magnitudeA = MathF.Sqrt(TensorPrimitives.Dot(a.AsSpan(), a.AsSpan()));
        var magnitudeB = MathF.Sqrt(TensorPrimitives.Dot(b.AsSpan(), b.AsSpan()));

        if (magnitudeA == 0 || magnitudeB == 0)
            return 0;

        return dotProduct / (magnitudeA * magnitudeB);
    }
}
