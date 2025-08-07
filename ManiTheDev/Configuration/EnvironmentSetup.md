# Environment Variables Setup

This application requires the following environment variables to be configured:

## Required Environment Variables

### OPENAI_API_KEY
- **Description**: Your OpenAI API key required for Semantic Kernel operations
- **Minimum Length**: 20 characters
- **How to get**: Visit https://platform.openai.com/api-keys
- **Example**: `sk-1234567890abcdef1234567890abcdef1234567890abcdef`

### AGENT_WORKSPACE_PATH
- **Description**: The workspace path where agents will store and manage files
- **Minimum Length**: 1 character
- **Recommended Path**: `C:\Users\%USERNAME%\Documents\ManiTheDev\workspace`
- **Example**: `C:\Users\YourUsername\Documents\ManiTheDev\workspace`

## Setup Instructions

### Option 1: Environment Variables (Recommended)
Set these environment variables in your system:

**Windows (PowerShell):**
```powershell
$env:OPENAI_API_KEY="your-openai-api-key-here"
$env:AGENT_WORKSPACE_PATH="C:\Users\$env:USERNAME\Documents\ManiTheDev\workspace"
```

**Windows (Command Prompt):**
```cmd
set OPENAI_API_KEY=your-openai-api-key-here
set AGENT_WORKSPACE_PATH=C:\Users\%USERNAME%\Documents\ManiTheDev\workspace
```

### Option 2: .env File
Create a `.env` file in the application directory with the following content:
```
OPENAI_API_KEY=your-openai-api-key-here
AGENT_WORKSPACE_PATH=C:\Users\%USERNAME%\Documents\ManiTheDev\workspace
```

### Option 3: User Secrets (Development)
For development, you can use .NET User Secrets:
```bash
dotnet user-secrets set "OPENAI_API_KEY" "your-openai-api-key-here"
dotnet user-secrets set "AGENT_WORKSPACE_PATH" "C:\Users\%USERNAME%\Documents\ManiTheDev\workspace"
```

## Workspace Directory

The application will automatically create the workspace directory if it doesn't exist. The recommended location is:
`C:\Users\%USERNAME%\Documents\ManiTheDev\workspace`

This location is:
- Easy to find and access
- Separate from the application code
- Backed up with user documents
- Accessible to all agents 