# ⚖️ Balance Beam Game - Q-Learning AI Demo

A physics-based balance game where the goal is to keep a ball balanced on a tilting beam. Watch as reinforcement learning AI learns to master this continuous control challenge!

## 🎯 Game Objective

**Keep the ball balanced on the beam as long as possible!**

- The ball has realistic physics (momentum, gravity, friction)
- Tilt the beam left or right to prevent the ball from falling off
- Survive as long as you can to maximize your score

## 🎮 This is NOT a 2-Player Game!

Unlike TicTacToe, the Balance Beam game is a **single-player challenge**:

- **You vs Physics** - Control the beam and fight gravity
- **AI learns the same task** - The AI isn't your opponent; it's learning to do what you do
- **Compare performance** - See who can balance the ball longer: You or the AI!

## 🤖 Three Game Modes

### 1. 🎮 Manual Play Mode
- **You control the beam** using arrow keys (← →)
- Try to keep the ball balanced as long as possible
- Your objective: Maximize survival time and score

### 2. 🤖 AI Play Mode  
- **Watch the trained AI** control the beam
- See the AI's learned strategy in action
- Compare AI performance vs your manual attempts

### 3. 📚 Training Mode
- **Watch the AI learn** through 1000+ training episodes
- Observe improvement over time (starts bad, gets better!)
- Uses Q-Learning reinforcement learning
- Very satisfying to watch the learning process

## 🎮 Controls

**Manual Mode:**
- **← Left Arrow** - Tilt beam left
- **→ Right Arrow** - Tilt beam right
- The beam will gradually return to center when you release

**Button Controls:**
- **🎮 Play Manually** - Start manual control mode
- **🤖 Watch AI Play** - See the trained AI in action
- **📚 Train AI (1000x)** - Run 1000 training episodes
- **⏸️ Stop Training** - Pause training and save progress
- **🔄 Reset** - Reset game and return to menu

## 🧠 How the Q-Learning AI Works

The AI learns through **trial and error**:

1. **Observes** the game state:
   - Ball position (where is it on the beam?)
   - Ball velocity (how fast is it moving?)
   - Beam angle (current tilt)

2. **Takes actions**:
   - Tilt Left
   - Hold Center  
   - Tilt Right

3. **Receives rewards**:
   - ✅ **+0.1 points** for each step survived
   - ✅ **Bonus** for keeping ball near center
   - ❌ **Penalty** for high velocity (unstable)
   - ❌ **-100 points** for dropping the ball

4. **Learns** from experience:
   - Builds a Q-table mapping states to best actions
   - Gradually improves through thousands of attempts
   - Eventually achieves stable, long-term balance

## 📊 What You'll See

**Initial Training (0-100 episodes):**
- Ball falls off quickly (1-5 seconds)
- Random, jerky movements
- AI is exploring

**Mid Training (100-500 episodes):**
- Survival time increases (5-20 seconds)
- More deliberate corrections
- AI is learning patterns

**Advanced Training (500+ episodes):**
- Can balance for 30+ seconds or indefinitely
- Smooth, predictive corrections
- AI has mastered the task!

## 🎨 Visual Features

- **Side-view perspective** - Clear physics visualization
- **Rotating beam** - Tilts realistically with pivot point
- **Ball with velocity arrows** - See momentum in real-time
- **Red danger zones** - Visual edge boundaries
- **Real-time stats** - Position, velocity, angle, Q-states learned
- **Smooth animations** - 20 FPS physics simulation

## 🏆 Challenge Yourself!

1. **Play manually** - What's your best survival time?
2. **Train the AI** - How many episodes until it beats your score?
3. **Watch the AI** - Can it achieve perfect balance?

## ⭐ Complexity Rating

**Difficulty:** Low-Medium
- Simple to understand objective
- Continuous control (not turn-based)
- Requires timing and prediction
- Perfect for learning reinforcement learning concepts

**Why This Game is Great:**
- ✅ **Visual** - Very satisfying to watch
- ✅ **Educational** - Clear demonstration of Q-Learning
- ✅ **Challenging** - Easy to play, hard to master
- ✅ **Rewarding** - Visible improvement as AI trains

## 🚀 Getting Started

1. **Run the application**
2. **Click "Train AI"** to teach the AI (recommended first step)
3. **Watch the training progress** - See it improve!
4. **Click "Watch AI Play"** to see the trained AI
5. **Click "Play Manually"** to try it yourself

## 💾 Save System

- Q-table automatically saves to `qtable_beam.dat`
- Training progress persists between sessions
- No need to retrain every time!

---

**Enjoy watching the AI master the balance!** 🎯⚖️🤖
