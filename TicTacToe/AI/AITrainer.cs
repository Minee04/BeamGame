using System;
using TicTacToe.GameLogic;
using TicTacToe.Models;

namespace TicTacToe.AI
{
    /// <summary>
    /// Trains Q-Learning AI by playing games against itself or other opponents
    /// </summary>
    public class AITrainer
    {
        private QLearningAI _aiPlayer1;
        private QLearningAI _aiPlayer2;
        private GameEngine _engine;

        public AITrainer(QLearningAI ai1, QLearningAI ai2 = null)
        {
            _aiPlayer1 = ai1;
            _aiPlayer2 = ai2 ?? new QLearningAI(); // Create second AI if not provided
            _engine = new GameEngine();
        }

        /// <summary>
        /// Trains the AI by playing multiple games
        /// </summary>
        public TrainingStats TrainAI(int numberOfGames, Action<int, TrainingStats> progressCallback = null)
        {
            var stats = new TrainingStats();

            for (int game = 0; game < numberOfGames; game++)
            {
                _engine.Reset();
                _aiPlayer1.StartNewGame();
                _aiPlayer2.StartNewGame();

                // Play one game
                GameResult result = PlayGame();

                // Update statistics
                UpdateStats(stats, result);

                // Learn from the game
                _aiPlayer1.LearnFromGame(result);
                _aiPlayer2.LearnFromGame(result);

                // Progress callback every 100 games
                if (progressCallback != null && (game + 1) % 100 == 0)
                {
                    progressCallback(game + 1, stats);
                }
            }

            return stats;
        }

        /// <summary>
        /// Plays a single game between two AIs
        /// </summary>
        private GameResult PlayGame()
        {
            while (true)
            {
                // Determine which AI's turn it is
                QLearningAI currentAI = _engine.CurrentPlayer == PlayerType.X ? _aiPlayer1 : _aiPlayer2;
                
                // Get move from AI
                CellPosition move = currentAI.DetermineNextMove(_engine, _engine.CurrentPlayer);
                
                if (move == null)
                    break;

                // Make the move
                _engine.MakeMove(move.Row, move.Column);

                // Check if game is over
                GameResult result = _engine.CheckGameState();
                if (result.State != Models.GameState.InProgress)
                {
                    return result;
                }

                // Switch player
                _engine.SwitchPlayer();
            }

            return new GameResult(Models.GameState.Draw);
        }

        /// <summary>
        /// Updates training statistics
        /// </summary>
        private void UpdateStats(TrainingStats stats, GameResult result)
        {
            stats.TotalGames++;

            switch (result.State)
            {
                case Models.GameState.XWins:
                    stats.XWins++;
                    break;
                case Models.GameState.OWins:
                    stats.OWins++;
                    break;
                case Models.GameState.Draw:
                    stats.Draws++;
                    break;
            }
        }
    }

    /// <summary>
    /// Statistics from AI training
    /// </summary>
    public class TrainingStats
    {
        public int TotalGames { get; set; }
        public int XWins { get; set; }
        public int OWins { get; set; }
        public int Draws { get; set; }

        public double XWinRate => TotalGames > 0 ? (double)XWins / TotalGames * 100 : 0;
        public double OWinRate => TotalGames > 0 ? (double)OWins / TotalGames * 100 : 0;
        public double DrawRate => TotalGames > 0 ? (double)Draws / TotalGames * 100 : 0;

        public override string ToString()
        {
            return $"Games: {TotalGames} | X Wins: {XWins} ({XWinRate:F1}%) | O Wins: {OWins} ({OWinRate:F1}%) | Draws: {Draws} ({DrawRate:F1}%)";
        }
    }
}
