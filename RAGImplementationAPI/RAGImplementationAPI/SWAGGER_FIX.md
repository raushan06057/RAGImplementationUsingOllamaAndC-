# How to Access Swagger UI - FIXED

## ✅ **The Correct URLs**

Your application runs on **custom ports**, not the default 5001. Use these URLs instead:

### **HTTPS (Recommended):**
```
https://localhost:7267
```

### **HTTP:**
```
http://localhost:5152
```

## 🚀 **Quick Start**

### Option 1: Run from Visual Studio
1. Press **F5** or click the **Play** button in Visual Studio
2. The browser will automatically open to Swagger UI
3. You're ready to test the API!

### Option 2: Run from Command Line
1. Open PowerShell in the project directory
2. Run:
   ```powershell
   dotnet run
   ```
3. Open your browser to: **https://localhost:7267**

## ✅ **What I Fixed**

1. **Removed environment restriction** - Swagger now works in all environments
2. **Enabled browser launch** - Browser will auto-open when you run the app
3. **Set correct URL** - Empty launchUrl means it goes to root (where Swagger is)

## 🔧 **If It Still Doesn't Work**

### 1. Check if the app is running
Look for this message in the terminal/output:
```
Now listening on: https://localhost:7267
Now listening on: http://localhost:5152
```

### 2. Check for port conflicts
If you see an error about ports already in use:
- Close any other instances of the application
- Or change the ports in `Properties/launchSettings.json`

### 3. Trust the development certificate
If you get SSL warnings:
```powershell
dotnet dev-certs https --trust
```

### 4. Clear browser cache
- Press **Ctrl+Shift+Delete**
- Clear cached data
- Try again

### 5. Check firewall
Make sure Windows Firewall isn't blocking the ports.

## 📝 **Testing the API**

Once Swagger loads at `https://localhost:7267`:

1. **Expand POST /api/rag/documents**
2. Click **"Try it out"**
3. Enter test data:
   ```json
   {
     "content": "Test document about .NET 10",
     "metadata": { "source": "test" }
   }
   ```
4. Click **"Execute"**
5. You should see a 200 response!

## 🌐 **Accessing from Other Devices**

If you want to access Swagger from another computer on your network:
1. Find your IP address: `ipconfig` in PowerShell
2. Update `launchSettings.json` to bind to `0.0.0.0` instead of `localhost`
3. Access via `https://YOUR_IP:7267`

## 🔍 **Verify Swagger Endpoint**

You can also directly access the OpenAPI spec:
```
https://localhost:7267/swagger/v1/swagger.json
```

This should return the JSON specification for your API.

## ⚡ **Hot Reload**

Since you have hot reload enabled:
- Code changes will automatically apply without restarting
- Swagger UI will reflect the changes
- Just refresh the browser page

## 📞 **Still Having Issues?**

Check the application logs in:
- Visual Studio: **View > Output** (select "RAGImplementationAPI" from dropdown)
- Terminal: Look for error messages in the console

Common errors:
- **401/403**: Authorization issues (shouldn't happen for Swagger)
- **404**: Wrong URL or Swagger not configured
- **500**: Check if Ollama is running for API endpoints
