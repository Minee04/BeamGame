# TicTacToe with Q-Learning AI

A Windows Forms TicTacToe game featuring AI opponents including a reinforcement learning (Q-Learning) AI that learns through self-play.

## Quick Start

### Run in VS Code
1. Install [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
2. Open workspace in VS Code
3. **Press F5** to build and run

### Command Line
```bash
# Build
msbuild TicTacToe.sln /p:Configuration=Debug

# Run
.\TicTacToe\bin\Debug\TicTacToe.exe
```

## Features

- **Player vs Player** - Classic two-player mode
- **Player vs Strategic AI** - Rule-based AI (blocks, wins, takes center/corners)
- **Player vs Q-Learning AI** - Reinforcement learning AI
- **Train AI** - Train Q-Learning through self-play (customizable game count)
- **Save/Load Q-Table** - Trained models persist between sessions

## Project Structure

```
TicTacToe/
├── Models/          # GameState, PlayerType, CellPosition
├── GameLogic/       # GameEngine, GameBoard (core game rules)
├── AI/              # StrategicAI, QLearningAI, AITrainer
└── Form1.cs         # UI
```

**Architecture**: Clean separation between UI, game logic, and AI using SOLID principles and the Strategy Pattern for pluggable AI implementations.

## How Q-Learning Works

The AI learns by playing thousands of games against itself, updating a Q-table that maps game states to optimal moves. Training parameters (learning rate, exploration rate) are tunable in the code.