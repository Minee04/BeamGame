using TicTacToe.GameLogic;
using TicTacToe.Models;

namespace TicTacToe.AI
{
    /// <summary>
    /// Manages computer player behavior and decision making
    /// </summary>
    public class ComputerPlayer
    {
        private readonly IAIStrategy _strategy;
        private readonly PlayerType _playerType;

        public PlayerType PlayerType => _playerType;

        public ComputerPlayer(PlayerType playerType, IAIStrategy strategy = null)
        {
            _playerType = playerType;
            _strategy = strategy ?? new StrategicAI();
        }

        /// <summary>
        /// Determines and returns the next move for the computer
        /// </summary>
        public CellPosition GetNextMove(GameEngine engine)
        {
            return _strategy.DetermineNextMove(engine, _playerType);
        }
    }
}
