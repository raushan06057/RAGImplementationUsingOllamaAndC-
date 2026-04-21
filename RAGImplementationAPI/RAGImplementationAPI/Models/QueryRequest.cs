namespace RAGImplementationAPI.Models;

/// <summary>
/// Request model for querying the RAG system
/// </summary>
public class QueryRequest
{
    /// <summary>
    /// The question or search query
    /// </summary>
    /// <example>What is the capital of France?</example>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Number of most relevant documents to retrieve (default: 3)
    /// </summary>
    /// <example>3</example>
    public int TopK { get; set; } = 3;
}
