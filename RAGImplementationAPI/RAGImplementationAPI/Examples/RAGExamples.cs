namespace RAGImplementationAPI.Examples;

/// <summary>
/// Example scenarios for using the RAG API
/// </summary>
public class RAGExamples
{
    // Example 1: Adding knowledge base documents
    public static readonly string[] KnowledgeBaseDocuments = new[]
    {
        "Microsoft Azure is a cloud computing platform that offers services for building, deploying, and managing applications through Microsoft-managed data centers.",
        ".NET is a free, cross-platform, open-source developer platform for building many different types of applications. It supports C#, F#, and Visual Basic.",
        "Docker is a platform for developing, shipping, and running applications in containers. Containers are lightweight and contain everything needed to run the application.",
        "Kubernetes is an open-source container orchestration platform that automates the deployment, scaling, and management of containerized applications.",
        "REST APIs follow the principles of Representational State Transfer and use HTTP methods like GET, POST, PUT, and DELETE to perform operations."
    };

    // Example 2: Sample queries
    public static readonly string[] SampleQueries = new[]
    {
        "What is Azure?",
        "Tell me about .NET",
        "What programming languages does .NET support?",
        "What is Docker used for?",
        "How does Kubernetes help with containers?"
    };

    // Example 3: Adding documents with metadata
    public class DocumentExample
    {
        public string Content { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new();
    }

    public static readonly DocumentExample[] DocumentsWithMetadata = new[]
    {
        new DocumentExample
        {
            Content = "Python is a high-level, interpreted programming language known for its simplicity and readability.",
            Metadata = new Dictionary<string, string>
            {
                { "category", "programming" },
                { "language", "python" },
                { "difficulty", "beginner" }
            }
        },
        new DocumentExample
        {
            Content = "Machine learning is a subset of artificial intelligence that enables systems to learn and improve from experience without being explicitly programmed.",
            Metadata = new Dictionary<string, string>
            {
                { "category", "AI" },
                { "topic", "machine-learning" },
                { "difficulty", "advanced" }
            }
        }
    };
}
