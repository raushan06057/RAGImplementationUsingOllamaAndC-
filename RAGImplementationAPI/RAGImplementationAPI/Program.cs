using RAGImplementationAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "RAG Implementation API",
        Version = "v1",
        Description = "A Retrieval-Augmented Generation (RAG) API using Ollama and .NET 10"
    });

    // Enable XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Register HTTP client for Ollama
builder.Services.AddHttpClient<IOllamaService, OllamaService>();

// Register RAG services
builder.Services.AddSingleton<IVectorStore, InMemoryVectorStore>();
builder.Services.AddScoped<IRAGService, RAGService>();
builder.Services.AddScoped<IDocumentProcessor, DocumentProcessor>();

var app = builder.Build();

// Enable static files for upload UI
app.UseStaticFiles();

// Enable Swagger for all environments (can be restricted to Development if needed)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RAG API v1");
    options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    options.DocumentTitle = "RAG API Documentation";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
