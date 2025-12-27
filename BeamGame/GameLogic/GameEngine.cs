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
        private double _elapsedTime;
        private const double MaxGameTime = 30.0; // 30 seconds

        public GameBoard Board => _board;
        public double GameTime => _elapsedTime;

        public GameEngine()
        {
            _board = new GameBoard();
            _elapsedTime = 0;
        }

        /// <summary>
        /// Resets the game to initial state
        /// </summary>
        public void Reset()
        {
            _board.Reset();
            _elapsedTime = 0;
        }

        /// <summary>
        /// Executes one game step with player actions and delta time
        /// </summary>
        public void Step(PlayerAction player1Action, PlayerAction player2Action, double deltaTime = 0.0167)
        {
            _board.UpdatePhysics(player1Action, player2Action, deltaTime);
            _elapsedTime += deltaTime;
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
            if (_elapsedTime >= MaxGameTime)
            {
                return new GameResult(Models.GameState.TimeExpired, GameTime, null);
            }
            
            // Game still in progress
            return new GameResult(Models.GameState.InProgress, GameTime, null);
        }
    }
}
