using System;
using BeamGame.Models;

namespace BeamGame.GameLogic
{
    /// <summary>
    /// Main game engine for 2-player competitive beam balancing
    /// </summary>
    public class GameEngine
    {
        private GameBoard _board;
        private int _stepCount;
        private const int MaxSteps = 3600;  // Maximum steps before time expires (60 seconds at 60 FPS)

        public GameBoard Board => _board;
        public int StepCount => _stepCount;
        public double GameTime => _stepCount * 0.05; // Each step is ~50ms

        public GameEngine()
        {
            _board = new GameBoard();
            _stepCount = 0;
        }

        /// <summary>
        /// Resets the game to initial state
        /// </summary>
        public void Reset()
        {
            _board.Reset();
            _stepCount = 0;
        }

        /// <summary>
        /// Executes one game step with player actions
        /// </summary>
        public void Step(PlayerAction player1Action, PlayerAction player2Action)
        {
            _board.UpdatePhysics(player1Action, player2Action);
            _stepCount++;
        }

        /// <summary>
        /// Checks the current game state and returns the result
        /// </summary>
        public GameResult CheckGameState()
        {
            bool p1Fell = _board.HasPlayer1Fallen();
            bool p2Fell = _board.HasPlayer2Fallen();
            
            // Both players fell - draw
            if (p1Fell && p2Fell)
            {
                return new GameResult(Models.GameState.BothFell, GameTime, null);
            }
            
            // Player 1 fell - Player 2 wins
            if (p1Fell)
            {
                return new GameResult(Models.GameState.Player2Wins, GameTime, Player.Player2);
            }
            
            // Player 2 fell - Player 1 wins
            if (p2Fell)
            {
                return new GameResult(Models.GameState.Player1Wins, GameTime, Player.Player1);
            }
            
            // Time limit reached - both survived
            if (_stepCount >= MaxSteps)
            {
                return new GameResult(Models.GameState.TimeExpired, GameTime, null);
            }
            
            // Game still in progress
            return new GameResult(Models.GameState.InProgress, GameTime, null);
        }
    }
}
