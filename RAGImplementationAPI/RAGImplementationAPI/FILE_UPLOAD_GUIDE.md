# Document Upload API Documentation

## Overview

The RAG API now supports uploading document files directly. The system automatically extracts text from supported file formats, generates embeddings, and adds them to the vector store.

## Supported File Types

- ✅ **PDF** (.pdf) - Portable Document Format
- ✅ **Text** (.txt) - Plain text files
- ✅ **Word** (.docx) - Microsoft Word documents (Open XML format)
- ❌ **Legacy Word** (.doc) - Not supported (convert to .docx first)

## API Endpoints

### 1. Upload Multiple Documents
**POST** `/api/rag/upload`

Upload multiple files in a single request.

**Content-Type:** `multipart/form-data`

**Parameters:**
- `files` (required): One or more files to upload
- `category` (optional): Category to assign to all uploaded files
- `source` (optional): Source identifier for all uploaded files

**Example using cURL:**
```bash
curl -X POST https://localhost:7267/api/rag/upload \
  -F "files=@document1.pdf" \
  -F "files=@document2.txt" \
  -F "files=@document3.docx" \
  -F "category=research" \
  -F "source=user-upload"
```

**Example Response:**
```json
{
  "documents": [
    {
      "id": "abc123",
      "content": "Extracted text from document1.pdf...",
      "metadata": {
        "filename": "document1.pdf",
        "uploadDate": "2024-01-15T10:30:00Z",
        "fileSize": "52000",
        "contentType": "application/pdf",
        "category": "research",
        "source": "user-upload"
      },
      "embedding": [0.1, 0.2, ...]
    }
  ],
  "errors": [],
  "totalFiles": 3,
  "successCount": 3,
  "failureCount": 0
}
```

### 2. Upload Single Document
**POST** `/api/rag/upload/single`

Upload a single file.

**Content-Type:** `multipart/form-data`

**Parameters:**
- `file` (required): The file to upload
- `category` (optional): Category for the file
- `source` (optional): Source identifier

**Example using cURL:**
```bash
curl -X POST https://localhost:7267/api/rag/upload/single \
  -F "file=@mydocument.pdf" \
  -F "category=technical" \
  -F "source=manual"
```

**Example Response:**
```json
{
  "id": "xyz789",
  "content": "Extracted text content...",
  "metadata": {
    "filename": "mydocument.pdf",
    "uploadDate": "2024-01-15T10:30:00Z",
    "fileSize": "52000",
    "contentType": "application/pdf",
    "category": "technical",
    "source": "manual"
  },
  "embedding": [0.1, 0.2, ...]
}
```

### 3. Get Supported File Types
**GET** `/api/rag/upload/supported-types`

Returns a list of supported file extensions.

**Example Response:**
```json
[".txt", ".pdf", ".docx", ".doc"]
```

## Using Swagger UI

1. **Navigate to:** `https://localhost:7267`

2. **Find the upload endpoint:**
   - Scroll to **POST /api/rag/upload** or **POST /api/rag/upload/single**
   - Click to expand

3. **Try it out:**
   - Click **"Try it out"**
   - Click **"Choose Files"** to select documents
   - Optionally enter `category` and `source`
   - Click **"Execute"**

4. **View results:**
   - See which files were successfully processed
   - Check for any errors
   - Each document gets a unique ID for tracking

## File Upload Limits

- **Maximum file size:** 50 MB per file
- **Multiple files:** Upload multiple files in one request
- **Concurrent uploads:** Supported

## Metadata

Each uploaded document automatically includes:
- `filename` - Original file name
- `uploadDate` - ISO 8601 timestamp
- `fileSize` - File size in bytes
- `contentType` - MIME type
- `category` - Optional user-provided category
- `source` - Optional user-provided source

## Error Handling

### Common Errors

**Unsupported file type:**
```json
{
  "errors": [
    {
      "fileName": "document.xyz",
      "error": "Unsupported file type. Supported types: .txt, .pdf, .docx"
    }
  ]
}
```

**Empty file:**
```json
{
  "errors": [
    {
      "fileName": "empty.pdf",
      "error": "No text content could be extracted from the file"
    }
  ]
}
```

**File too large:**
```json
{
  "error": "Request body too large",
  "details": "Maximum file size is 50 MB"
}
```

## Examples

### PowerShell
```powershell
# Upload a single PDF
$file = Get-Item "C:\docs\myfile.pdf"
$uri = "https://localhost:7267/api/rag/upload/single"

$form = @{
    file = $file
    category = "documentation"
    source = "user-docs"
}

Invoke-RestMethod -Uri $uri -Method Post -Form $form
```

### C# HttpClient
```csharp
using var client = new HttpClient();
using var form = new MultipartFormDataContent();

var fileContent = new ByteArrayContent(File.ReadAllBytes("document.pdf"));
fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
form.Add(fileContent, "file", "document.pdf");
form.Add(new StringContent("technical"), "category");

var response = await client.PostAsync(
    "https://localhost:7267/api/rag/upload/single", 
    form
);
```

### Python
```python
import requests

url = "https://localhost:7267/api/rag/upload/single"
files = {"file": open("document.pdf", "rb")}
data = {"category": "research", "source": "python-script"}

response = requests.post(url, files=files, data=data, verify=False)
print(response.json())
```

### JavaScript/Fetch
```javascript
const formData = new FormData();
formData.append('file', fileInput.files[0]);
formData.append('category', 'technical');
formData.append('source', 'web-upload');

const response = await fetch('https://localhost:7267/api/rag/upload/single', {
    method: 'POST',
    body: formData
});

const result = await response.json();
console.log(result);
```

## Query Uploaded Documents

After uploading, you can query the documents:

```bash
curl -X POST https://localhost:7267/api/rag/query \
  -H "Content-Type: application/json" \
  -d '{
    "query": "What information is in the uploaded documents?",
    "topK": 5
  }'
```

The RAG system will:
1. Convert your query to embeddings
2. Find the most relevant uploaded documents
3. Generate an answer based on the content

## Best Practices

1. **File Naming:** Use descriptive file names (they're stored in metadata)
2. **Categories:** Use consistent category names for better organization
3. **Sources:** Track where documents come from (e.g., "email", "web", "manual")
4. **Batch Uploads:** Upload multiple related files together
5. **Error Checking:** Always check the response for errors
6. **Text Quality:** Ensure PDFs have actual text (not just scanned images)

## Troubleshooting

### PDF text extraction fails
- Ensure the PDF contains actual text (not scanned images)
- Try converting scanned PDFs to text first using OCR
- Check if the PDF is password-protected

### DOCX extraction fails
- Ensure file is in .docx format (not legacy .doc)
- Try opening and re-saving in Microsoft Word
- Check file isn't corrupted

### File upload times out
- Check file size (must be under 50 MB)
- Verify network connection
- Try uploading one file at a time

### Ollama errors during processing
- Ensure Ollama is running: `ollama serve`
- Verify the model is available: `ollama list`
- Check Ollama logs for errors

## Performance Notes

- **Processing time:** Depends on file size and Ollama embedding speed
- **Large files:** May take several seconds to process
- **Multiple files:** Processed sequentially
- **Embeddings:** Generated in real-time during upload

## Security Considerations

- Files are processed in-memory only
- Original files are NOT permanently stored
- Only extracted text and embeddings are kept
- Validate file types on client side too
- Consider implementing virus scanning for production
