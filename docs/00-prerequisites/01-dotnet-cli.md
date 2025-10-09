# Comprehensive .NET CLI Commands for Building Enterprise Applications

Below is a complete guide to using .NET CLI commands for building, managing, and deploying enterprise applications. This assumes you're working in a Windows environment with VS Code, as per your setup.

## 📋 Prerequisites

- Install .NET SDK (latest LTS version recommended: 8.0)
- Ensure dotnet is in your PATH
- Open terminal in VS Code (Ctrl+`)

## 🏗️ Step-by-Step Enterprise Application Build Process

### 1. Create Solution Structure

```shell
# Create root directory and navigate
mkdir EnterpriseApp && cd EnterpriseApp

# Create solution file
dotnet new sln -n EnterpriseApp

# Create API project (Web API)
dotnet new webapi -n EnterpriseApp.Api --framework net8.0

# Create Class Library for Business Logic
dotnet new classlib -n EnterpriseApp.Core --framework net8.0

# Create Data Access Layer
dotnet new classlib -n EnterpriseApp.Data --framework net8.0

# Create Test Projects
dotnet new xunit -n EnterpriseApp.Api.Tests --framework net8.0
dotnet new xunit -n EnterpriseApp.Core.Tests --framework net8.0

# Add projects to solution
dotnet sln add EnterpriseApp.Api/EnterpriseApp.Api.csproj
dotnet sln add EnterpriseApp.Core/EnterpriseApp.Core.csproj
dotnet sln add EnterpriseApp.Data/EnterpriseApp.Data.csproj
dotnet sln add EnterpriseApp.Api.Tests/EnterpriseApp.Api.Tests.csproj
dotnet sln add EnterpriseApp.Core.Tests/EnterpriseApp.Core.Tests.csproj
```

### 2. Add Project References

```shell
# API references Core and Data
dotnet add EnterpriseApp.Api/EnterpriseApp.Api.csproj reference EnterpriseApp.Core/EnterpriseApp.Core.csproj
dotnet add EnterpriseApp.Api/EnterpriseApp.Api.csproj reference EnterpriseApp.Data/EnterpriseApp.Data.csproj

# Test projects reference their respective projects
dotnet add EnterpriseApp.Api.Tests/EnterpriseApp.Api.Tests.csproj reference EnterpriseApp.Api/EnterpriseApp.Api.csproj
dotnet add EnterpriseApp.Core.Tests/EnterpriseApp.Core.Tests.csproj reference EnterpriseApp.Core/EnterpriseApp.Core.csproj
```

### 3. Add NuGet Packages

```shell
# Entity Framework Core for Data layer
dotnet add EnterpriseApp.Data/EnterpriseApp.Data.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add EnterpriseApp.Data/EnterpriseApp.Data.csproj package Microsoft.EntityFrameworkCore.Design --version 8.0.0

# API packages
dotnet add EnterpriseApp.Api/EnterpriseApp.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add EnterpriseApp.Api/EnterpriseApp.Api.csproj package Swashbuckle.AspNetCore --version 6.5.0
dotnet add EnterpriseApp.Api/EnterpriseApp.Api.csproj package Serilog.AspNetCore --version 8.0.0

# Test packages
dotnet add EnterpriseApp.Api.Tests/EnterpriseApp.Api.Tests.csproj package Microsoft.AspNetCore.Mvc.Testing --version 8.0.0
dotnet add EnterpriseApp.Api.Tests/EnterpriseApp.Api.Tests.csproj package FluentAssertions --version 6.12.0
```

### 4. Restore Dependencies

```shell
# Restore all packages in solution
dotnet restore
```

### 5. Build the Application

```shell
# Build entire solution in Debug mode
dotnet build

# Build in Release mode
dotnet build --configuration Release

# Build specific project
dotnet build EnterpriseApp.Api/EnterpriseApp.Api.csproj --configuration Release

# Build with verbose output
dotnet build --verbosity detailed
```

### 6. Run and Test

```shell
# Run the API (from API project directory)
cd EnterpriseApp.Api
dotnet run

# Run with specific configuration
dotnet run --configuration Release --urls "https://localhost:5001;http://localhost:5000"

# Run tests
dotnet test

# Run specific test project
dotnet test EnterpriseApp.Api.Tests/EnterpriseApp.Api.Tests.csproj

# Run tests with coverage (requires coverlet)
dotnet test --collect:"XPlat Code Coverage"
```

### 7. Publish for Deployment

```shell
# Publish API for production
dotnet publish EnterpriseApp.Api/EnterpriseApp.Api.csproj --configuration Release --output ./publish --runtime win-x64 --self-contained false

# Publish as self-contained (includes runtime)
dotnet publish EnterpriseApp.Api/EnterpriseApp.Api.csproj --configuration Release --output ./publish-self-contained --runtime win-x64 --self-contained true

# Publish for Linux deployment
dotnet publish EnterpriseApp.Api/EnterpriseApp.Api.csproj --configuration Release --output ./publish-linux --runtime linux-x64 --self-contained true
```

### 8. Database Operations (EF Core)

```shell
# Add migration (from Data project)
cd EnterpriseApp.Data
dotnet ef migrations add InitialCreate --project EnterpriseApp.Data.csproj --startup-project ../EnterpriseApp.Api/EnterpriseApp.Api.csproj

# Update database
dotnet ef database update --project EnterpriseApp.Data.csproj --startup-project ../EnterpriseApp.Api/EnterpriseApp.Api.csproj

# Generate SQL script
dotnet ef migrations script --project EnterpriseApp.Data.csproj --startup-project ../EnterpriseApp.Api/EnterpriseApp.Api.csproj --output migration.sql
```

### 9. Docker Support (Optional)

```shell
# Add Docker support to API project
dotnet add EnterpriseApp.Api/EnterpriseApp.Api.csproj package Microsoft.VisualStudio.Azure.Containers.Tools.Targets

# Build Docker image
docker build -t enterpriseapp-api .

# Run in Docker
docker run -p 8080:80 enterpriseapp-api
```

## 🔧 Common Enterprise Scenarios

### Multi-Environment Configuration

```shell
# Build for different environments
dotnet build --configuration Release /p:EnvironmentName=Staging
dotnet publish --configuration Release /p:EnvironmentName=Production
```

### Clean and Rebuild

```shell
# Clean solution
dotnet clean

# Clean and build
dotnet clean && dotnet build --configuration Release
```

### Version Management

```shell
# Set version
dotnet build /p:Version=1.2.3.4

# Use version prefix/suffix
dotnet build /p:VersionPrefix=1.0.0 /p:VersionSuffix=beta
```

### Performance Analysis

```shell
# Build with symbols for debugging
dotnet build --configuration Debug --source-link

# Publish with ready-to-run
dotnet publish --configuration Release --runtime win-x64 /p:PublishReadyToRun=true
```

## 📊 Build Output Locations

- Binaries: `./EnterpriseApp.Api/bin/Debug/net8.0/` (Debug) or `./EnterpriseApp.Api/bin/Release/net8.0/` (Release)
- Published App: `./publish/` (from publish command)
- Test Results: `./TestResults/` (when running tests with coverage)

## 🚀 CI/CD Integration

For enterprise deployments, integrate these commands into your CI/CD pipeline:

```yaml
# Example GitHub Actions step
- name: Build and Test
  run: |
    dotnet restore
    dotnet build --configuration Release
    dotnet test --configuration Release --no-build
    dotnet publish --configuration Release --output ./publish
```

This comprehensive set of commands covers the entire lifecycle of building an enterprise .NET application with proper project structure, testing, and deployment preparation.
