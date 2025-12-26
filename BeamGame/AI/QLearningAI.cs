using System;
using System.Collections.Generic;
using System.Linq;
using BeamGame.GameLogic;
using BeamGame.Models;

namespace BeamGame.AI
{
    /// <summary>
    /// ULTRA-SIMPLIFIED Q-Learning AI for beam balance game
    /// Uses only 3-dimensional state space: Position Zone + Velocity Direction + Danger Level
    /// Total states: 5 × 3 × 3 = 45 states (extremely learnable!)
    /// </summary>
    public class QLearningAI
    {
        private Dictionary<string, Dictionary<string, double>> _qTable;
        private double _learningRate;
        private double _discountFactor;
        private double _explorationRate;
        private double _minExplorationRate;
        private double _explorationDecay;
        private Random _random;
        private List<(string state, PlayerAction action, double reward)> _gameHistory;

        public double ExplorationRate => _explorationRate;
        public int QTableSize => _qTable.Count;

        public QLearningAI(
            double learningRate = 0.4,
            double discountFactor = 0.9,
            double explorationRate = 1.0,
            double explorationDecay = 0.996,
            double minExplorationRate = 0.05)
        {
            _qTable = new Dictionary<string, Dictionary<string, double>>();
            _learningRate = learningRate;
            _discountFactor = discountFactor;
            _explorationRate = explorationRate;
            _explorationDecay = explorationDecay;
            _minExplorationRate = minExplorationRate;
            _random = new Random();
            _gameHistory = new List<(string, PlayerAction, double)>();
        }

        /// <summary>
        /// Determines next action - uses safety rules + Q-learning
        /// </summary>
        public PlayerAction DetermineNextAction(GameEngine engine, Player aiPlayer)
        {
            var ball = aiPlayer == Player.Player1 ? engine.Board.Player1Ball : engine.Board.Player2Ball;
            
            // SAFETY RULE 1: Emergency edge escape (ALWAYS override Q-learning)
            if (ball.IsOnBeam && Math.Abs(ball.Position) > 0.85)
            {
                return ball.Position > 0 ? PlayerAction.MoveLeft : PlayerAction.MoveRight;
            }
            
            // SAFETY RULE 2: Moving fast toward edge
            if (ball.IsOnBeam && Math.Abs(ball.Position) > 0.65)
            {
                if ((ball.Position > 0 && ball.Velocity > 0.02) || (ball.Position < 0 && ball.Velocity < -0.02))
                {
                    // Moving toward edge - counter it
                    return ball.Velocity > 0 ? PlayerAction.MoveLeft : PlayerAction.MoveRight;
                }
            }

            string state = GetStateString(engine, aiPlayer);
            PlayerAction[] actions = { PlayerAction.MoveLeft, PlayerAction.MoveRight, PlayerAction.Jump, PlayerAction.None };

            // Epsilon-greedy
            if (_random.NextDouble() < _explorationRate)
            {
                // Explore: but with intelligent bias
                return GetSmartRandomAction(ball);
            }
            else
            {
                // Exploit: use learned Q-values
                return GetBestAction(state, actions);
            }
        }
        
        /// <summary>
        /// Smart random action - biased toward safe moves
        /// </summary>
        private PlayerAction GetSmartRandomAction(BallState ball)
        {
            // If far from center, 70% toward center
            if (Math.Abs(ball.Position) > 0.5 && _random.NextDouble() < 0.7)
            {
                return ball.Position > 0 ? PlayerAction.MoveLeft : PlayerAction.MoveRight;
            }
            
            // 10% chance to jump if stable (for strategic positioning)
            if (ball.IsOnBeam && Math.Abs(ball.Velocity) < 0.02 && _random.NextDouble() < 0.1)
            {
                return PlayerAction.Jump;
            }
            
            // Otherwise weighted random: 40% left, 40% right, 15% none, 5% jump
            double r = _random.NextDouble();
            if (r < 0.40) return PlayerAction.MoveLeft;
            if (r < 0.80) return PlayerAction.MoveRight;
            if (r < 0.95) return PlayerAction.None;
            return PlayerAction.Jump;
        }

        public void RecordTransition(string state, PlayerAction action, double reward)
        {
            _gameHistory.Add((state, action, reward));
        }

        /// <summary>
        /// Learn from game using TD(0) Q-learning
        /// </summary>
        public void LearnFromGame(double finalReward)
        {
            for (int i = _gameHistory.Count - 1; i >= 0; i--)
            {
                var (state, action, reward) = _gameHistory[i];
                
                double currentQ = GetQValue(state, ActionToString(action));
                
                double maxNextQ = 0;
                if (i < _gameHistory.Count - 1)
                {
                    var (nextState, _, _) = _gameHistory[i + 1];
                    maxNextQ = GetMaxQValue(nextState);
                }
                else
                {
                    reward += finalReward;
                }
                
                // Q(s,a) = Q(s,a) + α[r + γ*maxQ(s',a') - Q(s,a)]
                double newQ = currentQ + _learningRate * (reward + _discountFactor * maxNextQ - currentQ);
                SetQValue(state, ActionToString(action), newQ);
            }

            _gameHistory.Clear();
            _explorationRate = Math.Max(_minExplorationRate, _explorationRate * _explorationDecay);
        }

        public void StartNewGame()
        {
            _gameHistory.Clear();
        }

        /// <summary>
        /// ULTRA-SIMPLE 3D state representation:
        /// Zone (5) × Velocity (3) × Danger (3) = 45 states total!
        /// </summary>
        public string GetStateString(GameEngine engine, Player aiPlayer)
        {
            var ball = aiPlayer == Player.Player1 ? engine.Board.Player1Ball : engine.Board.Player2Ball;
            
            // Dimension 1: Position Zone (5 bins)
            // 0=far-left, 1=left, 2=center, 3=right, 4=far-right
            int zone = GetZone(ball.Position);
            
            // Dimension 2: Velocity Direction (3 bins)
            // 0=moving-left, 1=stopped, 2=moving-right
            int velDir = GetVelocityDirection(ball.Velocity);
            
            // Dimension 3: Danger Level (3 bins)
            // 0=safe, 1=caution, 2=danger
            int danger = GetDangerLevel(ball.Position, ball.Velocity);
            
            return $"{zone}_{velDir}_{danger}";
        }

        private int GetZone(double position)
        {
            if (position < -0.5) return 0; // Far left
            if (position < -0.15) return 1; // Left
            if (position < 0.15) return 2;  // Center
            if (position < 0.5) return 3;   // Right
            return 4; // Far right
        }

        private int GetVelocityDirection(double velocity)
        {
            if (velocity < -0.01) return 0; // Moving left
            if (velocity < 0.01) return 1;  // Stopped
            return 2; // Moving right
        }

        private int GetDangerLevel(double position, double velocity)
        {
            double absPos = Math.Abs(position);
            double absVel = Math.Abs(velocity);
            
            // Danger if near edge OR moving fast toward edge
            if (absPos > 0.7 || absVel > 0.04)
                return 2; // Danger
            else if (absPos > 0.4 || absVel > 0.02)
                return 1; // Caution
            else
                return 0; // Safe
        }

        private string ActionToString(PlayerAction action)
        {
            return action.ToString();
        }

        private PlayerAction GetBestAction(string state, PlayerAction[] actions)
        {
            double maxQ = double.MinValue;
            PlayerAction bestAction = actions[0];
            
            foreach (var action in actions)
            {
                double q = GetQValue(state, ActionToString(action));
                if (q > maxQ)
                {
                    maxQ = q;
                    bestAction = action;
                }
            }
            
            return bestAction;
        }

        private double GetQValue(string state, string action)
        {
            if (!_qTable.ContainsKey(state))
                return 0.0;
            
            if (!_qTable[state].ContainsKey(action))
                return 0.0;
            
            return _qTable[state][action];
        }

        private void SetQValue(string state, string action, double value)
        {
            if (!_qTable.ContainsKey(state))
                _qTable[state] = new Dictionary<string, double>();
            
            _qTable[state][action] = value;
        }

        private double GetMaxQValue(string state)
        {
            if (!_qTable.ContainsKey(state) || _qTable[state].Count == 0)
                return 0.0;
            
            return _qTable[state].Values.Max();
        }

        public Tuple<int, double> GetStatistics()
        {
            return new Tuple<int, double>(_qTable.Count, _explorationRate);
        }

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

        public void ResetKnowledge()
        {
            _qTable.Clear();
            _explorationRate = 1.0;
        }
    }
}
