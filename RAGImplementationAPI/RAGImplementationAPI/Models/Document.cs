namespace RAGImplementationAPI.Models;

/// <summary>
/// Represents a document stored in the vector database
/// </summary>
public class Document
{
    /// <summary>
    /// Unique identifier for the document
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// The text content of the document
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Custom metadata associated with the document
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Vector embedding representation of the document content
    /// </summary>
    public float[]? Embedding { get; set; }
}
