using Microsoft.AspNetCore.Mvc;
using RAGImplementationAPI.Models;
using RAGImplementationAPI.Services;

namespace RAGImplementationAPI.Controllers;

/// <summary>
/// RAG (Retrieval-Augmented Generation) API endpoints for document management and querying
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RAGController : ControllerBase
{
    private readonly IRAGService _ragService;
    private readonly IDocumentProcessor _documentProcessor;
    private readonly ILogger<RAGController> _logger;

    public RAGController(IRAGService ragService, IDocumentProcessor documentProcessor, ILogger<RAGController> logger)
    {
        _ragService = ragService;
        _documentProcessor = documentProcessor;
        _logger = logger;
    }

    /// <summary>
    /// Add a new document to the vector store
    /// </summary>
    /// <param name="request">Document content and optional metadata</param>
    /// <returns>The created document with its embedding and unique ID</returns>
    /// <response code="200">Document successfully added to the vector store</response>
    /// <response code="500">Internal server error occurred while processing the document</response>
    [HttpPost("documents")]
    [ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Document>> AddDocument([FromBody] AddDocumentRequest request)
    {
        try
        {
            var document = await _ragService.AddDocumentAsync(request.Content, request.Metadata);
            return Ok(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding document");
            return StatusCode(500, new ErrorResponse 
            { 
                Error = "Failed to add document", 
                Details = ex.Message 
            });
        }
    }

    /// <summary>
    /// Query the RAG system with a question
    /// </summary>
    /// <param name="request">Query text and number of documents to retrieve</param>
    /// <returns>AI-generated answer based on retrieved relevant documents</returns>
    /// <response code="200">Successfully generated answer from retrieved context</response>
    /// <response code="400">Invalid query request</response>
    /// <response code="500">Internal server error occurred while processing the query</response>
    [HttpPost("query")]
    [ProducesResponseType(typeof(QueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<QueryResponse>> Query([FromBody] QueryRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest(new ErrorResponse { Error = "Query cannot be empty" });
            }

            var response = await _ragService.QueryAsync(request.Query, request.TopK);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing query");
            return StatusCode(500, new ErrorResponse
            {
                Error = "Failed to process query",
                Details = ex.Message
            });
        }
    }

    /// <summary>
    /// Upload one or more document files to the vector store
    /// </summary>
    /// <param name="files">The document files to upload (PDF, TXT, DOCX)</param>
    /// <param name="category">Optional category for all uploaded files</param>
    /// <param name="source">Optional source identifier for all uploaded files</param>
    /// <returns>Upload results with successfully processed documents and any errors</returns>
    /// <response code="200">Files processed (check response for individual file results)</response>
    /// <response code="400">No files provided or invalid request</response>
    /// <response code="500">Internal server error occurred</response>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(FileUploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [RequestSizeLimit(52428800)] // 50 MB limit
    public async Task<ActionResult<FileUploadResponse>> UploadDocuments(
        [FromForm] IFormFileCollection files,
        [FromForm] string? category = null,
        [FromForm] string? source = null)
    {
        try
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest(new ErrorResponse { Error = "No files provided" });
            }

            var response = new FileUploadResponse
            {
                TotalFiles = files.Count
            };

            foreach (var file in files)
            {
                try
                {
                    if (!_documentProcessor.IsSupportedFileType(file.FileName))
                    {
                        response.Errors.Add(new FileUploadError
                        {
                            FileName = file.FileName,
                            Error = $"Unsupported file type. Supported types: {string.Join(", ", _documentProcessor.GetSupportedExtensions())}"
                        });
                        continue;
                    }

                    using var stream = file.OpenReadStream();
                    var extractedText = await _documentProcessor.ExtractTextAsync(stream, file.FileName);

                    if (string.IsNullOrWhiteSpace(extractedText))
                    {
                        response.Errors.Add(new FileUploadError
                        {
                            FileName = file.FileName,
                            Error = "No text content could be extracted from the file"
                        });
                        continue;
                    }

                    var metadata = new Dictionary<string, string>
                    {
                        { "filename", file.FileName },
                        { "uploadDate", DateTime.UtcNow.ToString("O") },
                        { "fileSize", file.Length.ToString() },
                        { "contentType", file.ContentType }
                    };

                    if (!string.IsNullOrWhiteSpace(category))
                        metadata["category"] = category;

                    if (!string.IsNullOrWhiteSpace(source))
                        metadata["source"] = source;

                    var document = await _ragService.AddDocumentAsync(extractedText, metadata);
                    response.Documents.Add(document);
                    response.SuccessCount++;

                    _logger.LogInformation("Successfully processed file: {FileName}", file.FileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file: {FileName}", file.FileName);
                    response.Errors.Add(new FileUploadError
                    {
                        FileName = file.FileName,
                        Error = ex.Message
                    });
                }
            }

            response.FailureCount = response.Errors.Count;

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading documents");
            return StatusCode(500, new ErrorResponse
            {
                Error = "Failed to upload documents",
                Details = ex.Message
            });
        }
    }

    ///// <summary>
    ///// Upload a single document file to the vector store
    ///// </summary>
    ///// <param name="file">The document file to upload (PDF, TXT, DOCX)</param>
    ///// <param name="category">Optional category for the file</param>
    ///// <param name="source">Optional source identifier</param>
    ///// <returns>The created document with its embedding and unique ID</returns>
    ///// <response code="200">File successfully processed and added to vector store</response>
    ///// <response code="400">No file provided or unsupported file type</response>
    ///// <response code="500">Internal server error occurred</response>
    //[HttpPost("uploadsingle")]
    //[ProducesResponseType(typeof(Document), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    //[RequestSizeLimit(52428800)] // 50 MB limit
    //public async Task<ActionResult<Document>> UploadSingleDocument(
    //    [FromForm] IFormFile file,
    //    [FromForm] string? category = null,
    //    [FromForm] string? source = null)
    //{
    //    try
    //    {
    //        if (file == null)
    //        {
    //            return BadRequest(new ErrorResponse { Error = "No file provided" });
    //        }

    //        if (!_documentProcessor.IsSupportedFileType(file.FileName))
    //        {
    //            return BadRequest(new ErrorResponse
    //            {
    //                Error = $"Unsupported file type. Supported types: {string.Join(", ", _documentProcessor.GetSupportedExtensions())}"
    //            });
    //        }

    //        using var stream = file.OpenReadStream();
    //        var extractedText = await _documentProcessor.ExtractTextAsync(stream, file.FileName);

    //        if (string.IsNullOrWhiteSpace(extractedText))
    //        {
    //            return BadRequest(new ErrorResponse
    //            {
    //                Error = "No text content could be extracted from the file"
    //            });
    //        }

    //        var metadata = new Dictionary<string, string>
    //        {
    //            { "filename", file.FileName },
    //            { "uploadDate", DateTime.UtcNow.ToString("O") },
    //            { "fileSize", file.Length.ToString() },
    //            { "contentType", file.ContentType }
    //        };

    //        if (!string.IsNullOrWhiteSpace(category))
    //            metadata["category"] = category;

    //        if (!string.IsNullOrWhiteSpace(source))
    //            metadata["source"] = source;

    //        var document = await _ragService.AddDocumentAsync(extractedText, metadata);

    //        _logger.LogInformation("Successfully processed file: {FileName}", file.FileName);

    //        return Ok(document);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Error uploading document: {FileName}", file?.FileName ?? "unknown");
    //        return StatusCode(500, new ErrorResponse
    //        {
    //            Error = "Failed to upload document",
    //            Details = ex.Message
    //        });
    //    }
    //}

    /// <summary>
    /// Get list of supported file types for upload
    /// </summary>
    /// <returns>List of supported file extensions</returns>
    /// <response code="200">List of supported file extensions</response>
    [HttpGet("upload/supported-types")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public ActionResult<List<string>> GetSupportedFileTypes()
    {
        return Ok(_documentProcessor.GetSupportedExtensions());
    }
}

/// <summary>
/// Request model for adding a new document
/// </summary>
public class AddDocumentRequest
{
    /// <summary>
    /// The text content of the document
    /// </summary>
    /// <example>The capital of France is Paris. It is known for the Eiffel Tower.</example>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Optional metadata key-value pairs for the document
    /// </summary>
    /// <example>{ "source": "geography", "category": "cities" }</example>
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Error response model
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error message
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Detailed error information
    /// </summary>
    public string? Details { get; set; }
}
