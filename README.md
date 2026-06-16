# Deep-Skilling

This repository contains learning exercises organized by topic.

Top-level folders (exact names):

- 01-Design-Patterns
- 02-DSA
- 03-Advanced-SQL
- 04-NUnit-Moq
- 05-Entity-Framework
- 06-ASP.NET-WebAPI
- 07-Microservices
- 08-Angular
- 09-GIT

How to run a single exercise:

```powershell
# Build
dotnet build "01-Design-Patterns\Exercise2-FactoryMethod"
# Run
dotnet run --project "01-Design-Patterns\Exercise2-FactoryMethod"
```

Run all exercises (PowerShell script):

```powershell
.\scripts\run_all.ps1
```

Build all exercises:

```powershell
.\scripts\build_all.ps1
```

Conventions:
- `01-Design-Patterns` contains subfolders `ExerciseN-Name` with self-contained C# projects.
- Avoid including README files inside individual exercise folders unless necessary.
- Ignore build artifacts via `.gitignore`.
