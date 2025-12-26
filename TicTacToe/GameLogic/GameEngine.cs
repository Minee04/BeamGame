using System.Collections.Generic;
using TicTacToe.Models;

namespace TicTacToe.GameLogic
{
    /// <summary>
    /// Main game engine that handles game rules, win conditions, and turn management
    /// </summary>
    public class GameEngine
    {
        private GameBoard _board;
        private PlayerType _currentPlayer;
        private int _moveCount;

        public GameBoard Board => _board;
        public PlayerType CurrentPlayer => _currentPlayer;
        public int MoveCount => _moveCount;

        public GameEngine()
        {
            _board = new GameBoard();
            _currentPlayer = PlayerType.X;
            _moveCount = 0;
        }

        /// <summary>
        /// Resets the game to initial state
        /// </summary>
        public void Reset()
        {
            _board.Reset();
            _currentPlayer = PlayerType.X;
            _moveCount = 0;
        }

        /// <summary>
        /// Makes a move at the specified position
        /// </summary>
        /// <returns>True if the move was successful</returns>
        public bool MakeMove(int row, int column)
        {
            if (_board.SetCell(row, column, _currentPlayer))
            {
                _moveCount++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Switches to the next player
        /// </summary>
        public void SwitchPlayer()
        {
            _currentPlayer = _currentPlayer == PlayerType.X ? PlayerType.O : PlayerType.X;
        }

        /// <summary>
        /// Checks the current game state and returns the result
        /// </summary>
        public GameResult CheckGameState()
        {
            // Check for winner
            PlayerType winner = CheckForWinner();
            if (winner != PlayerType.None)
            {
                return new GameResult(
                    winner == PlayerType.X ? Models.GameState.XWins : Models.GameState.OWins,
                    winner
                );
            }

            // Check for draw
            if (_board.IsFull())
            {
                return new GameResult(Models.GameState.Draw);
            }

            // Game is still in progress
            return new GameResult(Models.GameState.InProgress);
        }

        /// <summary>
        /// Checks all win conditions and returns the winner if any
        /// </summary>
        private PlayerType CheckForWinner()
        {
            // Check horizontal lines
            for (int row = 0; row < GameBoard.Size; row++)
            {
                if (CheckLine(_board.GetCell(row, 0), _board.GetCell(row, 1), _board.GetCell(row, 2)))
                {
                    return _board.GetCell(row, 0);
                }
            }

            // Check vertical lines
            for (int col = 0; col < GameBoard.Size; col++)
            {
                if (CheckLine(_board.GetCell(0, col), _board.GetCell(1, col), _board.GetCell(2, col)))
                {
                    return _board.GetCell(0, col);
                }
            }

            // Check diagonals
            if (CheckLine(_board.GetCell(0, 0), _board.GetCell(1, 1), _board.GetCell(2, 2)))
            {
                return _board.GetCell(0, 0);
            }

            if (CheckLine(_board.GetCell(0, 2), _board.GetCell(1, 1), _board.GetCell(2, 0)))
            {
                return _board.GetCell(0, 2);
            }

            return PlayerType.None;
        }

        /// <summary>
        /// Checks if three cells form a winning line
        /// </summary>
        private bool CheckLine(PlayerType a, PlayerType b, PlayerType c)
        {
            return a != PlayerType.None && a == b && b == c;
        }

        /// <summary>
        /// Finds a winning or blocking move for the specified player
        /// </summary>
        public CellPosition FindWinningMove(PlayerType player)
        {
            // Check all empty cells
            var emptyCells = _board.GetEmptyCells();
            foreach (var cell in emptyCells)
            {
                // Temporarily place the player's mark
                _board.SetCell(cell.Row, cell.Column, player);

                // Check if this creates a win
                bool isWinningMove = CheckForWinner() == player;

                // Undo the move using ClearCell
                _board.ClearCell(cell.Row, cell.Column);

                if (isWinningMove)
                {
                    return cell;
                }
            }

            return null;
        }
    }
}
