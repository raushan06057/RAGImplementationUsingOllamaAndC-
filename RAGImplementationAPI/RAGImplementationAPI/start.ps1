# RAG API with Ollama - Quick Start Script
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "RAG API with Ollama - Quick Start" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Check if Ollama is installed
if (-not (Get-Command ollama -ErrorAction SilentlyContinue)) {
    Write-Host "[ERROR] Ollama is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install from: https://ollama.ai" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "[1/3] Checking Ollama service..." -ForegroundColor Green
Write-Host ""

# Start Ollama service in background
Start-Process -FilePath "ollama" -ArgumentList "serve" -WindowStyle Hidden -ErrorAction SilentlyContinue

# Wait for Ollama to start
Start-Sleep -Seconds 2

Write-Host "[2/3] Pulling llama2 model (if not already downloaded)..." -ForegroundColor Green
Write-Host "This may take a while on first run..." -ForegroundColor Yellow
Write-Host ""

ollama pull llama2

Write-Host ""
Write-Host "[3/3] Starting RAG API..." -ForegroundColor Green
Write-Host ""
Write-Host "Swagger UI will be available at:" -ForegroundColor Cyan
Write-Host "  HTTPS: https://localhost:7267" -ForegroundColor White
Write-Host "  HTTP:  http://localhost:5152" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Run the .NET application
dotnet run
