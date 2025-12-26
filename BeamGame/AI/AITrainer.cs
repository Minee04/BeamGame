using System;
using BeamGame.GameLogic;
using BeamGame.Models;

namespace BeamGame.AI
{
    /// <summary>
    /// Trains Q-Learning AI for 2-player beam game
    /// AI plays as Player2 against smart random opponent
    /// </summary>
    public class AITrainer
    {
        private QLearningAI _ai;
        private GameEngine _engine;
        private Random _random;

        public AITrainer(QLearningAI ai)
        {
            _ai = ai;
            _engine = new GameEngine();
            _random = new Random();
        }

        public TrainingStats TrainAI(int numberOfGames, Action<int, TrainingStats> progressCallback = null)
        {
            var stats = new TrainingStats();

            for (int game = 0; game < numberOfGames; game++)
            {
                _engine.Reset();
                _ai.StartNewGame();

                GameResult result = PlayGame();

                UpdateStats(stats, result);

                double finalReward = CalculateFinalReward(result);
                _ai.LearnFromGame(finalReward);

                if (progressCallback != null && (game + 1) % 100 == 0)
                {
                    progressCallback(game + 1, stats);
                }
            }

            return stats;
        }

        private GameResult PlayGame()
        {
            while (true)
            {
                string currentState = _ai.GetStateString(_engine, Player.Player2);
                
                // AI controls Player2, Player1 does smart random
                PlayerAction p1Action = GetOpponentAction();
                PlayerAction p2Action = _ai.DetermineNextAction(_engine, Player.Player2);
                
                _engine.Step(p1Action, p2Action);
                
                double reward = CalculateStepReward(_engine, Player.Player2);
                _ai.RecordTransition(currentState, p2Action, reward);

                GameResult result = _engine.CheckGameState();
                if (result.State != Models.GameState.InProgress)
                {
                    return result;
                }
            }
        }

        /// <summary>
        /// Opponent plays with 50% strategy, 50% random
        /// </summary>
        private PlayerAction GetOpponentAction()
        {
            var p1Ball = _engine.Board.Player1Ball;
            
            // 50% strategic
            if (_random.NextDouble() < 0.5)
            {
                // Near edge: move toward center
                if (Math.Abs(p1Ball.Position) > 0.7)
                {
                    return p1Ball.Position > 0 ? PlayerAction.MoveLeft : PlayerAction.MoveRight;
                }
                
                // Moving fast toward edge: counter
                if (Math.Abs(p1Ball.Position) > 0.5)
                {
                    if ((p1Ball.Position > 0 && p1Ball.Velocity > 0.02) || 
                        (p1Ball.Position < 0 && p1Ball.Velocity < -0.02))
                    {
                        return p1Ball.Velocity > 0 ? PlayerAction.MoveLeft : PlayerAction.MoveRight;
                    }
                }
            }
            
            // Random with bias
            double rand = _random.NextDouble();
            if (rand < 0.4) return PlayerAction.MoveLeft;
            if (rand < 0.8) return PlayerAction.MoveRight;
            return PlayerAction.None;
        }

        /// <summary>
        /// CLEAR REWARD SHAPING for fast learning
        /// </summary>
        private double CalculateStepReward(GameEngine engine, Player aiPlayer)
        {
            var ball = aiPlayer == Player.Player1 ? engine.Board.Player1Ball : engine.Board.Player2Ball;
            var oppBall = aiPlayer == Player.Player1 ? engine.Board.Player2Ball : engine.Board.Player1Ball;
            
            if (!ball.IsOnBeam)
                return 0; // Final reward handles this
            
            double reward = 0.05; // Small survival reward
            
            // Position reward: stay near center
            double absPos = Math.Abs(ball.Position);
            if (absPos < 0.2)
                reward += 1.0;    // Excellent: center
            else if (absPos < 0.5)
                reward += 0.3;    // Good: inner zone
            else if (absPos > 0.8)
                reward -= 3.0;    // Very bad: near edge
            else if (absPos > 0.6)
                reward -= 1.0;    // Bad: danger zone
            
            // Velocity reward: prefer low velocity
            double absVel = Math.Abs(ball.Velocity);
            if (absVel < 0.01)
                reward += 0.3;    // Good: stable
            else if (absVel > 0.04)
                reward -= 1.0;    // Bad: fast
            
            // Opponent comparison
            if (!oppBall.HasFallen && Math.Abs(oppBall.Position) > absPos + 0.15)
                reward += 0.5;    // Opponent in worse position
            
            return reward;
        }

        private double CalculateFinalReward(GameResult result)
        {
            // AI is Player2
            if (result.Winner == Player.Player2)
                return 50.0; // Win!
            else if (result.Winner == Player.Player1)
                return -50.0; // Loss
            else
                return 5.0; // Draw (both fell or timeout)
        }

        private void UpdateStats(TrainingStats stats, GameResult result)
        {
            stats.TotalGames++;

            if (result.Winner == Player.Player2)
                stats.Wins++;
            else if (result.Winner == Player.Player1)
                stats.Losses++;
            else
                stats.Draws++;

            stats.AverageGameTime = ((stats.AverageGameTime * (stats.TotalGames - 1)) + result.GameTime) / stats.TotalGames;
        }
    }

    public class TrainingStats
    {
        public int TotalGames { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
        public double AverageGameTime { get; set; }

        public double WinRate => TotalGames > 0 ? (double)Wins / TotalGames * 100 : 0;

        public override string ToString()
        {
            return $"Games: {TotalGames} | Wins: {Wins} ({WinRate:F1}%) | Losses: {Losses} | Draws: {Draws}";
        }
    }
}
