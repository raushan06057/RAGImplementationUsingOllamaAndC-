@echo off
echo ================================================
echo RAG API with Ollama - Quick Start
echo ================================================
echo.

REM Check if Ollama is installed
where ollama >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Ollama is not installed or not in PATH
    echo Please install from: https://ollama.ai
    pause
    exit /b 1
)

echo [1/3] Checking Ollama service...
echo.

REM Start Ollama service in background
start /B ollama serve >nul 2>&1

REM Wait a moment for Ollama to start
timeout /t 2 /nobreak >nul

echo [2/3] Pulling llama2 model (if not already downloaded)...
echo This may take a while on first run...
echo.
ollama pull llama2

echo.
echo [3/3] Starting RAG API...
echo.
echo Swagger UI will open at: https://localhost:7267
echo.
echo Press Ctrl+C to stop the server
echo ================================================
echo.

REM Run the .NET application
dotnet run
