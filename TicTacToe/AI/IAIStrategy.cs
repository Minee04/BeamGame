using TicTacToe.GameLogic;
using TicTacToe.Models;

namespace TicTacToe.AI
{
    /// <summary>
    /// Interface for AI strategies to determine the next move
    /// </summary>
    public interface IAIStrategy
    {
        CellPosition DetermineNextMove(GameEngine engine, PlayerType aiPlayer);
    }
}
