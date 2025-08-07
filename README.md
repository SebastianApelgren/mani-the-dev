# ManiTheDev - AI Agent System

An AI agent system built with .NET 8, Semantic Kernel, and EasyReasy.EnvironmentVariables for robust environment variable management.

## Features

- **Environment Variable Management**: Uses EasyReasy.EnvironmentVariables for startup-time validation and type-safe access
- **AI Agent System**: Multiple specialized agents working together
- **File Management**: Tools for file operations and workspace management
- **Database Operations**: JSON-based database operations
- **Code Generation**: AI-powered code generation capabilities

## Quick Start

### 1. Prerequisites

- .NET 8.0 SDK
- OpenAI API key

### 2. Environment Setup

The application requires two environment variables:

#### Option A: Environment Variables (Recommended)
```powershell
$env:OPENAI_API_KEY="your-openai-api-key-here"
$env:AGENT_WORKSPACE_PATH="C:\Users\$env:USERNAME\Documents\ManiTheDev\workspace"
```

#### Option B: .env File
Create a `.env` file in the project root:
```
OPENAI_API_KEY=your-openai-api-key-here
AGENT_WORKSPACE_PATH=C:\Users\%USERNAME%\Documents\ManiTheDev\workspace
```

Copy `ManiTheDev/env.example` to `.env` and configure your values.

### 3. Build and Run

```bash
dotnet build
dotnet run --project ManiTheDev
```

## Environment Variables

| Variable | Description | Required | Min Length |
|----------|-------------|----------|------------|
| `OPENAI_API_KEY` | Your OpenAI API key | Yes | 20 |
| `AGENT_WORKSPACE_PATH` | Path for agent workspace | Yes | 1 |

## Workspace Directory

The application automatically creates the workspace directory at:
`C:\Users\%USERNAME%\Documents\ManiTheDev\workspace`

This location is:
- Easy to find and access
- Separate from application code
- Backed up with user documents
- Accessible to all agents

## Architecture

### Environment Variable Management
- Uses EasyReasy.EnvironmentVariables for type-safe environment variable access
- Startup-time validation ensures all required variables are present
- Clear error messages if configuration is missing

### Service Configuration
- Dependency injection with Microsoft.Extensions.DependencyInjection
- Automatic workspace directory creation
- Semantic Kernel integration with OpenAI

### Agent System
- Multiple specialized agents (DatabaseManager, Coding, Orchestrator)
- Tool-based architecture for file and database operations
- Async interface methods with synchronous utility implementations

## Configuration Files

- `ManiTheDev/Configuration/EnvironmentVariable.cs` - Environment variable definitions
- `ManiTheDev/Configuration/ServiceConfiguration.cs` - DI container configuration
- `ManiTheDev/Configuration/EnvironmentSetup.md` - Detailed setup instructions

## Development

### Adding New Environment Variables

1. Add the variable definition to `EnvironmentVariable.cs`:
```csharp
[EnvironmentVariableName(minLength: 10)]
public static readonly VariableName NewVariable = new VariableName("NEW_VARIABLE");
```

2. Use it in your code:
```csharp
string value = EnvironmentVariable.NewVariable.GetValue();
```

### Building

```bash
dotnet build
```

The build includes StyleCop analysis for code quality.

## Error Handling

The application provides clear error messages for:
- Missing environment variables
- Invalid environment variable values
- Workspace directory creation issues

See `Configuration/EnvironmentSetup.md` for detailed troubleshooting. 