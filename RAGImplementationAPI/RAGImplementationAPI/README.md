# RAG Implementation with Ollama and .NET 10

This project implements a Retrieval-Augmented Generation (RAG) system using Ollama for embeddings and text generation, built with .NET 10.

## Features

- **Document Storage**: Add and store documents with vector embeddings
- **Semantic Search**: Find relevant documents using cosine similarity
- **RAG Query**: Answer questions using retrieved context
- **In-Memory Vector Store**: Fast document retrieval with vector similarity search
- **Ollama Integration**: Uses Ollama for both embeddings and LLM generation
- **Swagger UI**: Interactive API documentation and testing

## Prerequisites

1. **Ollama**: Install and run Ollama locally
   - Download from: https://ollama.ai
   - Pull a model: `ollama pull llama2`
   - Ollama should be running on http://localhost:11434

2. **.NET 10 SDK**: Required for building and running the API

## Quick Start

1. **Install and start Ollama:**
```bash
ollama pull llama2
ollama serve
```

2. **Run the API:**
```bash
dotnet run
```

3. **Open Swagger UI:**
   - Navigate to **`https://localhost:7267`** (HTTPS) or **`http://localhost:5152`** (HTTP)
   - Swagger UI will load automatically
   - You'll see the interactive API documentation

## Configuration

Edit `appsettings.json` to configure the Ollama URL:

```json
{
  "Ollama": {
    "Url": "http://localhost:11434"
  }
}
```

## API Endpoints

### 1. Upload Document Files (NEW! 📄)
**POST** `/api/rag/upload`

Upload PDF, TXT, or DOCX files directly. The system automatically extracts text and generates embeddings.

**Supported Formats:**
- PDF (.pdf)
- Text (.txt)
- Word (.docx)

**Form Data:**
- `files`: One or more document files
- `category` (optional): Category for organization
- `source` (optional): Source identifier

**Try in Swagger:** Navigate to `https://localhost:7267` and use the interactive file upload!

See [FILE_UPLOAD_GUIDE.md](FILE_UPLOAD_GUIDE.md) for detailed documentation.

### 2. Add Document (JSON)
**POST** `/api/rag/documents`

Add a document to the vector store with automatic embedding generation.

**Request Body:**
```json
{
  "content": "The capital of France is Paris. It is known for the Eiffel Tower.",
  "metadata": {
    "source": "geography",
    "category": "cities"
  }
}
```

**Response:**
```json
{
  "id": "guid",
  "content": "The capital of France is Paris...",
  "metadata": {
    "source": "geography",
    "category": "cities"
  },
  "embedding": [0.1, 0.2, ...]
}
```

### 3. Query with RAG
**POST** `/api/rag/query`

Ask a question and get an answer based on retrieved documents (including uploaded files).

**Request Body:**
```json
{
  "query": "What is the capital of France?",
  "topK": 3
}
```

**Response:**
```json
{
  "answer": "Based on the provided context, the capital of France is Paris...",
  "retrievedDocuments": [
    {
      "content": "The capital of France is Paris...",
      "similarity": 0.95,
      "metadata": {
        "source": "geography"
      }
    }
  ]
}
```

## Usage Example

### Using Swagger UI (Recommended)

1. **Start Ollama:**
```bash
ollama serve
```

2. **Pull a model:**
```bash
ollama pull llama2
```

3. **Run the API:**
```bash
dotnet run
```

4. **Open Swagger UI:**
   - Navigate to `https://localhost:5001` or `http://localhost:5000`
   - The Swagger UI will load automatically at the root URL
   - You can test all endpoints interactively

5. **Try it out in Swagger:**
   - Click on **POST /api/rag/documents** to expand
   - Click **Try it out**
   - Enter sample JSON:
   ```json
   {
     "content": "The Earth orbits around the Sun. It takes 365 days to complete one orbit.",
     "metadata": {"topic": "astronomy"}
   }
   ```
   - Click **Execute**

   - Then try **POST /api/rag/query**:
   ```json
   {
     "query": "How long does it take Earth to orbit the Sun?",
     "topK": 3
   }
   ```

### Using cURL or Postman
### Using cURL or Postman

**Add documents:**
```bash
curl -X POST http://localhost:5000/api/rag/documents \
  -H "Content-Type: application/json" \
  -d '{
    "content": "The Earth orbits around the Sun. It takes 365 days to complete one orbit.",
    "metadata": {"topic": "astronomy"}
  }'
```

**Query:**
```bash
curl -X POST http://localhost:5000/api/rag/query \
  -H "Content-Type: application/json" \
  -d '{
    "query": "How long does it take Earth to orbit the Sun?",
    "topK": 3
  }'
```

## Architecture

- **OllamaService**: Handles communication with Ollama API for embeddings and generation
- **VectorStore**: In-memory storage with cosine similarity search
- **RAGService**: Orchestrates retrieval and generation process
- **RAGController**: REST API endpoints

## How RAG Works

1. **Indexing Phase**:
   - Documents are added via `/api/rag/documents`
   - Each document is converted to embeddings using Ollama
   - Documents and embeddings are stored in the vector store

2. **Query Phase**:
   - User query is converted to embeddings
   - Vector store finds most similar documents using cosine similarity
   - Retrieved documents are used as context for the LLM
   - Ollama generates an answer based on the context

## Notes

- The vector store is in-memory, so data is lost on restart
- For production, consider using a persistent vector database (Qdrant, Pinecone, etc.)
- Default model is `llama2`, but you can specify different models
- Adjust `topK` parameter to retrieve more or fewer documents

## Future Enhancements

- Persistent vector storage (SQL, Redis, or dedicated vector DB)
- Document chunking for large texts
- Multiple embedding models support
- Streaming responses
- Document deletion and updates
- Batch document ingestion
