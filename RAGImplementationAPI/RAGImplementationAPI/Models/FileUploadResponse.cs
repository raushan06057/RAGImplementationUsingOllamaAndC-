namespace RAGImplementationAPI.Models;

/// <summary>
/// Response model for file upload operations
/// </summary>
public class FileUploadResponse
{
    /// <summary>
    /// List of successfully processed documents
    /// </summary>
    public List<Document> Documents { get; set; } = new();

    /// <summary>
    /// List of files that failed to process
    /// </summary>
    public List<FileUploadError> Errors { get; set; } = new();

    /// <summary>
    /// Total number of files uploaded
    /// </summary>
    public int TotalFiles { get; set; }

    /// <summary>
    /// Number of successfully processed files
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Number of failed files
    /// </summary>
    public int FailureCount { get; set; }
}

/// <summary>
/// Represents an error that occurred during file upload
/// </summary>
public class FileUploadError
{
    /// <summary>
    /// Name of the file that failed
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Error message describing what went wrong
    /// </summary>
    public string Error { get; set; } = string.Empty;
}
