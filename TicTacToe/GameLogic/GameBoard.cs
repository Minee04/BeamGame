using System.Collections.Generic;
using TicTacToe.Models;

namespace TicTacToe.GameLogic
{
    /// <summary>
    /// Manages the game board state and provides methods to interact with it
    /// </summary>
    public class GameBoard
    {
        private PlayerType[,] _board;
        public const int Size = 3;

        public GameBoard()
        {
            _board = new PlayerType[Size, Size];
            Reset();
        }

        /// <summary>
        /// Resets the board to its initial state
        /// </summary>
        public void Reset()
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    _board[row, col] = PlayerType.None;
                }
            }
        }

        /// <summary>
        /// Gets the player type at a specific position
        /// </summary>
        public PlayerType GetCell(int row, int column)
        {
            if (IsValidPosition(row, column))
            {
                return _board[row, column];
            }
            return PlayerType.None;
        }

        /// <summary>
        /// Sets a cell to a specific player type
        /// </summary>
        public bool SetCell(int row, int column, PlayerType player)
        {
            if (IsValidPosition(row, column) && _board[row, column] == PlayerType.None)
            {
                _board[row, column] = player;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears a cell (used for undoing moves in AI simulation)
        /// </summary>
        public void ClearCell(int row, int column)
        {
            if (IsValidPosition(row, column))
            {
                _board[row, column] = PlayerType.None;
            }
        }

        /// <summary>
        /// Checks if a position is valid and empty
        /// </summary>
        public bool IsCellEmpty(int row, int column)
        {
            return IsValidPosition(row, column) && _board[row, column] == PlayerType.None;
        }

        /// <summary>
        /// Checks if the board is full
        /// </summary>
        public bool IsFull()
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if (_board[row, col] == PlayerType.None)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Gets all empty cells on the board
        /// </summary>
        public List<CellPosition> GetEmptyCells()
        {
            var emptyCells = new List<CellPosition>();
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if (_board[row, col] == PlayerType.None)
                    {
                        emptyCells.Add(new CellPosition(row, col));
                    }
                }
            }
            return emptyCells;
        }

        /// <summary>
        /// Checks if a position is within the board bounds
        /// </summary>
        private bool IsValidPosition(int row, int column)
        {
            return row >= 0 && row < Size && column >= 0 && column < Size;
        }

        /// <summary>
        /// Gets a copy of the current board state
        /// </summary>
        public PlayerType[,] GetBoardState()
        {
            var copy = new PlayerType[Size, Size];
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    copy[row, col] = _board[row, col];
                }
            }
            return copy;
        }
    }
}
