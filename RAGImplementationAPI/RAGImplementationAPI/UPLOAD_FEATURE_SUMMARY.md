# 📄 File Upload Feature - Quick Reference

## ✅ What Was Added

### **New API Endpoints:**
1. **POST /api/rag/upload** - Upload multiple files
2. **POST /api/rag/upload/single** - Upload single file
3. **GET /api/rag/upload/supported-types** - Get supported file types

### **Supported File Formats:**
- ✅ PDF (.pdf)
- ✅ Text (.txt)
- ✅ Word (.docx)

### **New Features:**
- Automatic text extraction from documents
- Automatic embedding generation
- File metadata tracking (name, size, upload date)
- Batch file upload support
- Error handling per file

## 🚀 Quick Start

### **Option 1: Use the Upload UI (Easiest)**
1. Navigate to: **`https://localhost:7267/upload.html`**
2. Drag & drop files or click "Choose Files"
3. Add optional category/source
4. Click "Upload to RAG System"
5. Done! ✅

### **Option 2: Use Swagger UI**
1. Navigate to: **`https://localhost:7267`**
2. Find **POST /api/rag/upload**
3. Click "Try it out"
4. Click "Choose Files" and select documents
5. Click "Execute"

### **Option 3: Use cURL**
```bash
curl -X POST https://localhost:7267/api/rag/upload \
  -F "files=@document.pdf" \
  -F "category=technical" \
  -F "source=manual"
```

## 📋 Example Workflows

### Upload and Query
```bash
# 1. Upload a document
curl -X POST https://localhost:7267/api/rag/upload/single \
  -F "file=@mydocument.pdf"

# 2. Query the content
curl -X POST https://localhost:7267/api/rag/query \
  -H "Content-Type: application/json" \
  -d '{"query": "Summarize the document", "topK": 3}'
```

### Batch Upload with Categories
```bash
curl -X POST https://localhost:7267/api/rag/upload \
  -F "files=@doc1.pdf" \
  -F "files=@doc2.txt" \
  -F "files=@doc3.docx" \
  -F "category=research" \
  -F "source=batch-upload-2024"
```

## 🔧 New NuGet Packages Added

- **itext7** (v9.0.0) - PDF text extraction
- **DocumentFormat.OpenXml** (v3.2.0) - DOCX text extraction

## 📁 New Files Created

### Services:
- `Services/DocumentProcessor.cs` - Handles file text extraction

### Models:
- `Models/FileUploadResponse.cs` - Upload response model

### Documentation:
- `FILE_UPLOAD_GUIDE.md` - Comprehensive upload documentation
- `wwwroot/upload.html` - Web-based upload UI

### Controllers:
- Updated `Controllers/RAGController.cs` with upload endpoints

## 🎯 Features in Detail

### Automatic Metadata
Each uploaded file gets:
```json
{
  "filename": "original-name.pdf",
  "uploadDate": "2024-01-15T10:30:00Z",
  "fileSize": "52000",
  "contentType": "application/pdf",
  "category": "your-category",
  "source": "your-source"
}
```

### Error Handling
Handles multiple error scenarios:
- Unsupported file types
- Empty files
- Extraction failures
- Individual file failures in batch uploads

### Response Format
```json
{
  "documents": [...],
  "errors": [...],
  "totalFiles": 3,
  "successCount": 2,
  "failureCount": 1
}
```

## 🌐 Access URLs

- **Swagger UI:** https://localhost:7267
- **Upload UI:** https://localhost:7267/upload.html
- **API Base:** https://localhost:7267/api/rag

## 💡 Tips

1. **Use the Upload UI** for interactive testing
2. **Check file size** - 50 MB limit per file
3. **PDF quality** - Works best with text-based PDFs (not scanned images)
4. **Batch uploads** - Upload related documents together
5. **Add metadata** - Use categories and sources for organization

## 📖 Full Documentation

See [FILE_UPLOAD_GUIDE.md](FILE_UPLOAD_GUIDE.md) for:
- Detailed API documentation
- Code examples in multiple languages
- Troubleshooting guide
- Best practices

## 🧪 Test It Now!

1. **Start Ollama:**
   ```bash
   ollama serve
   ```

2. **Run the API:**
   ```bash
   dotnet run
   ```

3. **Open Upload UI:**
   ```
   https://localhost:7267/upload.html
   ```

4. **Upload a test file and query it!**

## 🎉 Success!

Your RAG API now supports document uploads! Users can:
- Upload PDFs, TXT, and DOCX files
- Process multiple files at once
- Query uploaded document content
- Track all uploaded documents with metadata
