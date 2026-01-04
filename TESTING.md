# VS Code Testing Guide

## Quick Start

**First Time Setup:**
1. Run `setup_and_run_tests.bat` to download packages and run tests
2. Or install the **C# Dev Kit** extension in VS Code (includes test runner)

## Running Tests in VS Code

### Method 1: Using C# Dev Kit Test Explorer (Easiest)
1. Install **C# Dev Kit** extension from Microsoft  
2. Run `setup_and_run_tests.bat` once to install test packages
3. Open Testing panel (beaker icon in sidebar)
4. Click refresh if tests don't appear
5. Click play button to run tests
6. View results in Test Explorer

### Method 2: Using Tasks (Keyboard shortcut)
1. Run `setup_and_run_tests.bat` once 
2. Press `Ctrl+Shift+P` → **Tasks: Run Task** → **test**
3. Or press `Ctrl+Shift+B` → select **test**

### Method 3: Using Terminal
```powershell
# First time - install packages
.\setup_and_run_tests.bat

# After setup - run tests quickly
.\run_tests.bat
```

### Method 4: Using .NET CLI (if you have .NET SDK)
```powershell
# Build and run tests
dotnet test BeamGame.Tests\BeamGame.Tests.csproj

# Run specific test
dotnet test --filter FullyQualifiedName~GameEngineTests
```

## Troubleshooting

**"Package not found" errors:**
- Run `setup_and_run_tests.bat` to download test packages

**Tests don't appear in Test Explorer:**
- Click the refresh icon in Test Explorer
- Rebuild the solution (Ctrl+Shift+B)
- Restart VS Code

**"vstest.console.exe not found":**
- The batch file includes the full path to Visual Studio's test runner
- Make sure Visual Studio 2022 Community is installed

## Test Coverage

**GameEngineTests** (5 tests)
- Game state management, Win/loss detection, Reset functionality

**GameBoardTests** (5 tests)  
- Ball physics, Movement actions, Boundaries

**QLearningAITests** (7 tests)
- AI decision making, Learning process, Edge defense

**AITrainerTests** (5 tests)
- Training process, Statistics tracking, Win rate calculations

**Total: 22 Tests**
