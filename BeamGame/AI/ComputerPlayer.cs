using System;
using BeamGame.GameLogic;
using BeamGame.Models;

namespace BeamGame.AI
{
    /// <summary>
    /// Wrapper for AI strategy that controls a player in the beam game
    /// </summary>
    public class ComputerPlayer
    {
        private readonly QLearningAI _strategy;
        private readonly Player _player;

        public ComputerPlayer(Player player)
        {
            _player = player;
            _strategy = new QLearningAI();
        }

        public ComputerPlayer(Player player, QLearningAI strategy)
        {
            _player = player;
            _strategy = strategy;
        }

        public Player ControlledPlayer => _player;

        public PlayerAction DetermineMove(GameEngine engine)
        {
            return _strategy.DetermineNextAction(engine, _player);
        }

        public void LoadQTable(string filePath)
        {
            _strategy.LoadQTable(filePath);
        }

        public void SaveQTable(string filePath)
        {
            _strategy.SaveQTable(filePath);
        }

        public QLearningAI GetQLearningAI()
        {
            return _strategy;
        }

        public Tuple<int, double> GetAIStatistics()
        {
            return _strategy.GetStatistics();
        }
    }
}
