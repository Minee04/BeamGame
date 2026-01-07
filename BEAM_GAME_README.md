# ⚖️ Balance Beam Game - Q-Learning AI Demo

A physics-based balance game where two players compete on a tilting beam. Play against a friend or watch the AI learn to master the challenge through Q-Learning!

## 🚀 How to Run

**Prerequisites:** .NET Framework 4.7.2 or higher

**To Run the Game:**
1. Double-click `BeamGame.exe`

**For Development:**
1. Open `BeamGame.sln` in Visual Studio
2. Press F5 to build and run

## 🎮 Game Modes

**👥 Two Player** - Compete against a friend  
**🤖 vs AI** - Play against the trained AI  
**📚 Train AI** - Watch the AI learn through thousands of training games

## 🎮 Controls

🔴 **Player 1:** A/D = Move, W = Jump  
🔵 **Player 2:** ← → = Move, ↑ = Jump

## 🧠 Q-Learning AI

The AI learns by observing player positions, velocities, and beam angle, then choosing optimal actions. Through reinforcement learning, it progressively improves its balance and strategy, with progress auto-saved to `ai_qtable.dat`.