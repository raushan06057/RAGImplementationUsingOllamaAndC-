# Swagger UI Guide

## Accessing Swagger

Once the application is running, Swagger UI is available at the **root URL**:

- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`

## Features

### Interactive API Documentation

Swagger provides:
- **Complete API Reference**: All endpoints with detailed descriptions
- **Request/Response Models**: See the structure of all data models
- **Try It Out**: Test endpoints directly from the browser
- **Code Examples**: Auto-generated code snippets in multiple languages

### Available Endpoints

#### 1. POST /api/rag/documents
**Add a new document to the vector store**

- Converts document text to embeddings using Ollama
- Stores document with metadata for retrieval
- Returns the created document with ID and embedding

**Sample Request:**
```json
{
  "content": "Python is a high-level programming language known for simplicity.",
  "metadata": {
    "category": "programming",
    "language": "python"
  }
}
```

#### 2. POST /api/rag/query
**Query the RAG system**

- Converts query to embeddings
- Retrieves most similar documents
- Generates AI answer based on context

**Sample Request:**
```json
{
  "query": "What is Python?",
  "topK": 3
}
```

## Using Swagger UI

### Step 1: Expand an Endpoint
Click on any endpoint (e.g., **POST /api/rag/documents**) to see details.

### Step 2: Try It Out
Click the **"Try it out"** button to enable editing.

### Step 3: Enter Data
- Modify the sample JSON in the request body
- Adjust any parameters

### Step 4: Execute
Click **"Execute"** to send the request.

### Step 5: View Response
- See the response code (200, 400, 500, etc.)
- View the response body
- Check response headers

## XML Documentation

All endpoints include:
- **Summary**: Brief description of what the endpoint does
- **Parameters**: Detailed parameter documentation with examples
- **Response Codes**: All possible response types
- **Example Values**: Sample requests and responses

## Tips

1. **Add Documents First**: Before querying, add some documents to the vector store
2. **Test with Examples**: Use the example JSON provided in each endpoint
3. **Check Response**: Always verify the response code and body
4. **Use Metadata**: Add metadata to organize your documents
5. **Adjust topK**: Experiment with different topK values (1-10) for better results

## Troubleshooting

### Swagger UI doesn't load
- Ensure the application is running
- Check if you're accessing the correct URL
- Clear browser cache

### Endpoints return 500 errors
- Verify Ollama is running (`ollama serve`)
- Check that the model is pulled (`ollama pull llama2`)
- Review application logs for details

### No documents retrieved
- Ensure you've added documents first using POST /api/rag/documents
- Check that embeddings were generated successfully
- Verify the query is relevant to your stored documents
