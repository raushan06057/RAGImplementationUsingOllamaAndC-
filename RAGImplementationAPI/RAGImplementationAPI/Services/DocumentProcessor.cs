using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text;

namespace RAGImplementationAPI.Services;

public interface IDocumentProcessor
{
    Task<string> ExtractTextAsync(Stream fileStream, string fileName);
    bool IsSupportedFileType(string fileName);
    List<string> GetSupportedExtensions();
}

public class DocumentProcessor : IDocumentProcessor
{
    private readonly ILogger<DocumentProcessor> _logger;
    private readonly HashSet<string> _supportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt", ".pdf", ".docx", ".doc"
    };

    public DocumentProcessor(ILogger<DocumentProcessor> logger)
    {
        _logger = logger;
    }

    public List<string> GetSupportedExtensions()
    {
        return _supportedExtensions.ToList();
    }

    public bool IsSupportedFileType(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        bool isSupport = _supportedExtensions.Contains(extension);
        return  isSupport;
    }

    public async Task<string> ExtractTextAsync(Stream fileStream, string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            ".txt" => await ExtractTextFromTextFileAsync(fileStream),
            ".pdf" => await ExtractTextFromPdfAsync(fileStream),
            ".docx" => await ExtractTextFromDocxAsync(fileStream),
            ".doc" => throw new NotSupportedException("Legacy .doc format is not supported. Please convert to .docx"),
            _ => throw new NotSupportedException($"File type {extension} is not supported")
        };
    }

    private async Task<string> ExtractTextFromTextFileAsync(Stream fileStream)
    {
        try
        {
            using var reader = new StreamReader(fileStream, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from text file");
            throw new InvalidOperationException("Failed to extract text from text file", ex);
        }
    }

    private async Task<string> ExtractTextFromPdfAsync(Stream fileStream)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var pdfReader = new PdfReader(memoryStream);
            using var pdfDocument = new PdfDocument(pdfReader);

            var text = new StringBuilder();
            for (int page = 1; page <= pdfDocument.GetNumberOfPages(); page++)
            {
                var strategy = new SimpleTextExtractionStrategy();
                var pageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page), strategy);
                text.AppendLine(pageText);
            }

            return text.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from PDF");
            throw new InvalidOperationException("Failed to extract text from PDF file", ex);
        }
    }

    private async Task<string> ExtractTextFromDocxAsync(Stream fileStream)
    {
        try
        {
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var wordDocument = WordprocessingDocument.Open(memoryStream, false);
            var body = wordDocument.MainDocumentPart?.Document.Body;

            if (body == null)
                return string.Empty;

            return body.InnerText;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from DOCX");
            throw new InvalidOperationException("Failed to extract text from DOCX file", ex);
        }
    }
}
