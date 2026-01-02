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
        private List<(string state, PlayerAction action, double reward, string nextState)> _gameHistory;

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
            _gameHistory = new List<(string, PlayerAction, double, string)>();
        }

        /// <summary>
        /// Determines next action - uses safety rules + Q-learning
        /// </summary>
        public PlayerAction DetermineNextAction(GameEngine engine, Player aiPlayer)
        {
            var ball = aiPlayer == Player.Player1 ? engine.Board.Player1Ball : engine.Board.Player2Ball;
            
            // Safety rules: emergency edge escape
            if (ball.IsOnBeam && Math.Abs(ball.Position) > 0.85)
                return ball.Position > 0 ? PlayerAction.MoveLeft : PlayerAction.MoveRight;
            
            // Counter fast motion toward edge
            if (ball.IsOnBeam && Math.Abs(ball.Position) > 0.65 &&
                ((ball.Position > 0 && ball.Velocity > 0.02) || (ball.Position < 0 && ball.Velocity < -0.02)))
                return ball.Velocity > 0 ? PlayerAction.MoveLeft : PlayerAction.MoveRight;

            string state = GetStateString(engine, aiPlayer);

            // Epsilon-greedy: explore with smart bias or exploit Q-values
            if (_random.NextDouble() < _explorationRate)
                return GetSmartRandomAction(ball);
            else
                return GetBestAction(state);
        }
        
        private PlayerAction GetSmartRandomAction(BallState ball)
        {
            // 70% toward center if far from center
            if (Math.Abs(ball.Position) > 0.5 && _random.NextDouble() < 0.7)
                return ball.Position > 0 ? PlayerAction.MoveLeft : PlayerAction.MoveRight;
            
            // 10% jump if stable
            if (ball.IsOnBeam && Math.Abs(ball.Velocity) < 0.02 && _random.NextDouble() < 0.1)
                return PlayerAction.Jump;
            
            // Weighted random: 40% left, 40% right, 15% none, 5% jump
            double r = _random.NextDouble();
            if (r < 0.40) return PlayerAction.MoveLeft;
            if (r < 0.80) return PlayerAction.MoveRight;
            if (r < 0.95) return PlayerAction.None;
            return PlayerAction.Jump;
        }

        public void RecordTransition(string state, PlayerAction action, double reward, string nextState)
        {
            _gameHistory.Add((state, action, reward, nextState));
        }

        /// <summary>
        /// Learn from game using TD(0) Q-learning with correct temporal ordering
        /// </summary>
        public void LearnFromGame(double finalReward)
        {
            // Process in FORWARD order (correct temporal sequence)
            for (int i = 0; i < _gameHistory.Count; i++)
            {
                var (state, action, reward, nextState) = _gameHistory[i];
                
                string actionStr = action.ToString();
                double currentQ = GetQValue(state, actionStr);
                
                // For last transition, use final reward; otherwise bootstrap from next state
                double maxNextQ = (i == _gameHistory.Count - 1) 
                    ? finalReward 
                    : GetMaxQValue(nextState);
                
                // Q(s,a) = Q(s,a) + α[r + γ*maxQ(s',a') - Q(s,a)]
                double newQ = currentQ + _learningRate * (reward + _discountFactor * maxNextQ - currentQ);
                SetQValue(state, actionStr, newQ);
            }

            _gameHistory.Clear();
            _explorationRate = Math.Max(_minExplorationRate, _explorationRate * _explorationDecay);
        }

        public void StartNewGame()
        {
            _gameHistory.Clear();
        }

        /// <summary>
        /// ULTRA-SIMPLE 3D state: Zone (5) × Velocity (3) × Danger (3) = 45 states
        /// </summary>
        public string GetStateString(GameEngine engine, Player aiPlayer)
        {
            var ball = aiPlayer == Player.Player1 ? engine.Board.Player1Ball : engine.Board.Player2Ball;
            double p = ball.Position, v = ball.Velocity;
            double absP = Math.Abs(p), absV = Math.Abs(v);
            
            // Calculate zone (5 bins)
            int zone;
            if (p < -0.5) zone = 0;
            else if (p < -0.15) zone = 1;
            else if (p < 0.15) zone = 2;
            else if (p < 0.5) zone = 3;
            else zone = 4;
            
            // Calculate velocity direction (3 bins)
            int velDir;
            if (v < -0.01) velDir = 0;
            else if (v < 0.01) velDir = 1;
            else velDir = 2;
            
            // Calculate danger level (3 bins)
            int danger;
            if (absP > 0.7 || absV > 0.04) danger = 2;
            else if (absP > 0.4 || absV > 0.02) danger = 1;
            else danger = 0;
            
            return $"{zone}_{velDir}_{danger}";
        }

        private PlayerAction GetBestAction(string state)
        {
            var actions = new[] { PlayerAction.MoveLeft, PlayerAction.MoveRight, PlayerAction.Jump, PlayerAction.None };
            
            PlayerAction bestAction = actions[0];
            double maxQ = GetQValue(state, actions[0].ToString());
            
            for (int i = 1; i < actions.Length; i++)
            {
                double q = GetQValue(state, actions[i].ToString());
                if (q > maxQ)
                {
                    maxQ = q;
                    bestAction = actions[i];
                }
            }
            
            return bestAction;
        }

        private double GetQValue(string state, string action)
        {
            if (_qTable.TryGetValue(state, out var actions) && 
                actions.TryGetValue(action, out double value))
            {
                return value;
            }
            return 0.0;
        }

        private void SetQValue(string state, string action, double value)
        {
            if (!_qTable.TryGetValue(state, out var actions))
                _qTable[state] = actions = new Dictionary<string, double>();
            actions[action] = value;
        }

        private double GetMaxQValue(string state)
        {
            if (!_qTable.TryGetValue(state, out var actions) || actions.Count == 0)
                return 0.0;
            
            double maxQ = double.MinValue;
            foreach (var q in actions.Values)
            {
                if (q > maxQ)
                    maxQ = q;
            }
            return maxQ;
        }

        public Tuple<int, double> GetStatistics()
        {
            return new Tuple<int, double>(_qTable.Count, _explorationRate);
        }

        public void SaveQTable(string filepath)
        {
            using (var writer = new System.IO.StreamWriter(filepath))
                foreach (var state in _qTable)
                    foreach (var action in state.Value)
                        writer.WriteLine($"{state.Key}|{action.Key}|{action.Value}");
        }

        public void LoadQTable(string filepath)
        {
            if (!System.IO.File.Exists(filepath)) return;
            _qTable.Clear();
            foreach (var line in System.IO.File.ReadLines(filepath))
            {
                var parts = line.Split('|');
                if (parts.Length == 3 && double.TryParse(parts[2], out double value))
                    SetQValue(parts[0], parts[1], value);
            }
        }

        public void ResetKnowledge()
        {
            _qTable.Clear();
            _explorationRate = 1.0;
        }
    }
}