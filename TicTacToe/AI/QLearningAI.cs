using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToe.GameLogic;
using TicTacToe.Models;

namespace TicTacToe.AI
{
    /// <summary>
    /// Q-Learning AI that learns optimal moves through reinforcement learning
    /// </summary>
    public class QLearningAI : IAIStrategy
    {
        // Q-table: maps (state, action) -> Q-value
        private Dictionary<string, Dictionary<string, double>> _qTable;
        
        // Learning parameters
        private double _learningRate;      // Alpha: how much to update Q-values (0-1)
        private double _discountFactor;    // Gamma: importance of future rewards (0-1)
        private double _explorationRate;   // Epsilon: probability of random exploration (0-1)
        private double _minExplorationRate;
        private double _explorationDecay;
        
        private Random _random;
        private PlayerType _aiPlayer;
        
        // Track game history for learning
        private List<(string state, string action)> _gameHistory;

        public double ExplorationRate => _explorationRate;
        public int QTableSize => _qTable.Count;

        /// <summary>
        /// Creates a new Q-Learning AI
        /// </summary>
        /// <param name="learningRate">Alpha: how quickly to learn (0.1 - 0.5 typical)</param>
        /// <param name="discountFactor">Gamma: importance of future rewards (0.9 - 0.99 typical)</param>
        /// <param name="explorationRate">Epsilon: initial exploration rate (0.5 - 1.0)</param>
        /// <param name="explorationDecay">How much to reduce exploration each game (0.995 - 0.999)</param>
        public QLearningAI(
            double learningRate = 0.3,
            double discountFactor = 0.95,
            double explorationRate = 1.0,
            double explorationDecay = 0.998,
            double minExplorationRate = 0.01)
        {
            _qTable = new Dictionary<string, Dictionary<string, double>>();
            _learningRate = learningRate;
            _discountFactor = discountFactor;
            _explorationRate = explorationRate;
            _explorationDecay = explorationDecay;
            _minExplorationRate = minExplorationRate;
            _random = new Random();
            _gameHistory = new List<(string, string)>();
        }

        public CellPosition DetermineNextMove(GameEngine engine, PlayerType aiPlayer)
        {
            _aiPlayer = aiPlayer;
            string state = GetStateString(engine.Board);
            var availableMoves = engine.Board.GetEmptyCells();

            if (availableMoves.Count == 0)
                return null;

            // CRITICAL: Always check for immediate win or block first (safety layer)
            PlayerType opponent = aiPlayer == PlayerType.X ? PlayerType.O : PlayerType.X;
            
            // Priority 1: Win if possible
            CellPosition winningMove = engine.FindWinningMove(aiPlayer);
            if (winningMove != null)
            {
                // Record and return winning move
                string action = PositionToAction(winningMove);
                _gameHistory.Add((state, action));
                return winningMove;
            }

            // Priority 2: Block opponent's winning move
            CellPosition blockingMove = engine.FindWinningMove(opponent);
            if (blockingMove != null)
            {
                // Record and return blocking move
                string action = PositionToAction(blockingMove);
                _gameHistory.Add((state, action));
                return blockingMove;
            }

            // Priority 3: Use Q-learning for strategic positioning
            CellPosition selectedMove;

            // Epsilon-greedy: explore vs exploit
            if (_random.NextDouble() < _explorationRate)
            {
                // Explore: random move
                selectedMove = availableMoves[_random.Next(availableMoves.Count)];
            }
            else
            {
                // Exploit: choose best known move
                selectedMove = GetBestMove(state, availableMoves);
            }

            // Record this state-action pair for learning
            string selectedAction = PositionToAction(selectedMove);
            _gameHistory.Add((state, selectedAction));

            return selectedMove;
        }

        /// <summary>
        /// Called when a game ends to update Q-values based on the result
        /// </summary>
        public void LearnFromGame(GameResult result)
        {
            double reward = GetReward(result);

            // Update Q-values for all moves in reverse order (temporal difference learning)
            for (int i = _gameHistory.Count - 1; i >= 0; i--)
            {
                var (state, action) = _gameHistory[i];
                
                // Get current Q-value
                double currentQ = GetQValue(state, action);
                
                // Calculate max Q-value for next state (if not terminal)
                double maxNextQ = 0;
                if (i < _gameHistory.Count - 1)
                {
                    var (nextState, _) = _gameHistory[i + 1];
                    maxNextQ = GetMaxQValue(nextState);
                }
                
                // Q-learning update formula: Q(s,a) = Q(s,a) + α * [R + γ * max(Q(s',a')) - Q(s,a)]
                double newQ = currentQ + _learningRate * (reward + _discountFactor * maxNextQ - currentQ);
                
                // Update Q-table
                SetQValue(state, action, newQ);
                
                // For non-terminal states, reward is 0 (only final state gets actual reward)
                reward = 0;
            }

            // Clear history for next game
            _gameHistory.Clear();

            // Decay exploration rate
            _explorationRate = Math.Max(_minExplorationRate, _explorationRate * _explorationDecay);
        }

        /// <summary>
        /// Resets the game history (call this at the start of each game)
        /// </summary>
        public void StartNewGame()
        {
            _gameHistory.Clear();
        }

        /// <summary>
        /// Gets reward based on game outcome
        /// </summary>
        private double GetReward(GameResult result)
        {
            if (result.State == Models.GameState.Draw)
                return 0.5;  // Small positive reward for draw
            
            if ((result.Winner == PlayerType.X && _aiPlayer == PlayerType.X) ||
                (result.Winner == PlayerType.O && _aiPlayer == PlayerType.O))
                return 1.0;  // Win
            
            return -1.0;  // Loss
        }

        /// <summary>
        /// Converts board state to string for Q-table lookup
        /// </summary>
        private string GetStateString(GameBoard board)
        {
            var state = board.GetBoardState();
            var chars = new char[9];
            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    PlayerType cellValue = state[i, j];
                    if (cellValue == PlayerType.X)
                        chars[i * 3 + j] = 'X';
                    else if (cellValue == PlayerType.O)
                        chars[i * 3 + j] = 'O';
                    else
                        chars[i * 3 + j] = '_';
                }
            }
            
            return new string(chars);
        }

        /// <summary>
        /// Converts position to action string
        /// </summary>
        private string PositionToAction(CellPosition pos)
        {
            return $"{pos.Row},{pos.Column}";
        }

        /// <summary>
        /// Converts action string to position
        /// </summary>
        private CellPosition ActionToPosition(string action)
        {
            var parts = action.Split(',');
            return new CellPosition(int.Parse(parts[0]), int.Parse(parts[1]));
        }

        /// <summary>
        /// Gets Q-value for state-action pair
        /// </summary>
        private double GetQValue(string state, string action)
        {
            if (!_qTable.ContainsKey(state))
                return 0.0;
            
            if (!_qTable[state].ContainsKey(action))
                return 0.0;
            
            return _qTable[state][action];
        }

        /// <summary>
        /// Sets Q-value for state-action pair
        /// </summary>
        private void SetQValue(string state, string action, double value)
        {
            if (!_qTable.ContainsKey(state))
                _qTable[state] = new Dictionary<string, double>();
            
            _qTable[state][action] = value;
        }

        /// <summary>
        /// Gets maximum Q-value for a state
        /// </summary>
        private double GetMaxQValue(string state)
        {
            if (!_qTable.ContainsKey(state) || _qTable[state].Count == 0)
                return 0.0;
            
            return _qTable[state].Values.Max();
        }

        /// <summary>
        /// Gets the best move based on Q-values
        /// </summary>
        private CellPosition GetBestMove(string state, List<CellPosition> availableMoves)
        {
            CellPosition bestMove = availableMoves[0];
            double bestValue = double.MinValue;

            foreach (var move in availableMoves)
            {
                string action = PositionToAction(move);
                double qValue = GetQValue(state, action);

                if (qValue > bestValue)
                {
                    bestValue = qValue;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        /// <summary>
        /// Saves Q-table to file
        /// </summary>
        public void SaveQTable(string filepath)
        {
            using (var writer = new System.IO.StreamWriter(filepath))
            {
                foreach (var statePair in _qTable)
                {
                    foreach (var actionPair in statePair.Value)
                    {
                        writer.WriteLine($"{statePair.Key}|{actionPair.Key}|{actionPair.Value}");
                    }
                }
            }
        }

        /// <summary>
        /// Loads Q-table from file
        /// </summary>
        public void LoadQTable(string filepath)
        {
            if (!System.IO.File.Exists(filepath))
                return;

            _qTable.Clear();
            
            using (var reader = new System.IO.StreamReader(filepath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        string state = parts[0];
                        string action = parts[1];
                        double value = double.Parse(parts[2]);
                        SetQValue(state, action, value);
                    }
                }
            }
        }
    }
}
