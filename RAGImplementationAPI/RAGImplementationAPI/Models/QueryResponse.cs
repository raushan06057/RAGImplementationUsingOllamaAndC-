namespace RAGImplementationAPI.Models;

/// <summary>
/// Response model containing the AI-generated answer and retrieved documents
/// </summary>
public class QueryResponse
{
    /// <summary>
    /// The AI-generated answer based on retrieved context
    /// </summary>
    public string Answer { get; set; } = string.Empty;

    /// <summary>
    /// List of documents retrieved from the vector store
    /// </summary>
    public List<RetrievedDocument> RetrievedDocuments { get; set; } = new();
}

/// <summary>
/// Represents a document retrieved from the vector store with similarity score
/// </summary>
public class RetrievedDocument
{
    /// <summary>
    /// The content of the retrieved document
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Cosine similarity score between query and document (0-1)
    /// </summary>
    public float Similarity { get; set; }

    /// <summary>
    /// Metadata associated with the document
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}
