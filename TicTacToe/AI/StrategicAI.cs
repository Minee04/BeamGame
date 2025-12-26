using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToe.GameLogic;
using TicTacToe.Models;

namespace TicTacToe.AI
{
    /// <summary>
    /// Strategic AI that prioritizes winning, blocking, and strategic positions
    /// </summary>
    public class StrategicAI : IAIStrategy
    {
        private readonly Random _random = new Random();

        public CellPosition DetermineNextMove(GameEngine engine, PlayerType aiPlayer)
        {
            PlayerType opponent = aiPlayer == PlayerType.X ? PlayerType.O : PlayerType.X;

            // Priority 1: Try to win
            var winningMove = engine.FindWinningMove(aiPlayer);
            if (winningMove != null)
                return winningMove;

            // Priority 2: Block opponent from winning
            var blockingMove = engine.FindWinningMove(opponent);
            if (blockingMove != null)
                return blockingMove;

            // Priority 3: Take center if available
            var centerMove = TryGetCenter(engine.Board);
            if (centerMove != null)
                return centerMove;

            // Priority 4: Take a corner
            var cornerMove = TryGetCorner(engine.Board, aiPlayer);
            if (cornerMove != null)
                return cornerMove;

            // Priority 5: Take an edge
            var edgeMove = TryGetEdge(engine.Board);
            if (edgeMove != null)
                return edgeMove;

            // Priority 6: Take any open space
            return GetRandomOpenSpace(engine.Board);
        }

        private CellPosition TryGetCenter(GameBoard board)
        {
            if (board.IsCellEmpty(1, 1))
            {
                return new CellPosition(1, 1);
            }
            return null;
        }

        private CellPosition TryGetCorner(GameBoard board, PlayerType aiPlayer)
        {
            var corners = new List<CellPosition>
            {
                new CellPosition(0, 0),
                new CellPosition(0, 2),
                new CellPosition(2, 0),
                new CellPosition(2, 2)
            };

            // First, try to get a corner adjacent to our existing marks
            var preferredCorners = corners.Where(c => board.IsCellEmpty(c.Row, c.Column) && HasAdjacentMark(board, c, aiPlayer)).ToList();
            if (preferredCorners.Any())
            {
                return preferredCorners[_random.Next(preferredCorners.Count)];
            }

            // Otherwise, take any available corner
            var availableCorners = corners.Where(c => board.IsCellEmpty(c.Row, c.Column)).ToList();
            if (availableCorners.Any())
            {
                return availableCorners[_random.Next(availableCorners.Count)];
            }

            return null;
        }

        private CellPosition TryGetEdge(GameBoard board)
        {
            var edges = new List<CellPosition>
            {
                new CellPosition(0, 1),
                new CellPosition(1, 0),
                new CellPosition(1, 2),
                new CellPosition(2, 1)
            };

            var availableEdges = edges.Where(e => board.IsCellEmpty(e.Row, e.Column)).ToList();
            if (availableEdges.Any())
            {
                return availableEdges[_random.Next(availableEdges.Count)];
            }

            return null;
        }

        private CellPosition GetRandomOpenSpace(GameBoard board)
        {
            var emptyCells = board.GetEmptyCells();
            if (emptyCells.Count > 0)
            {
                return emptyCells[_random.Next(emptyCells.Count)];
            }
            return null;
        }

        private bool HasAdjacentMark(GameBoard board, CellPosition position, PlayerType player)
        {
            var adjacentPositions = new[]
            {
                new CellPosition(position.Row - 1, position.Column),
                new CellPosition(position.Row + 1, position.Column),
                new CellPosition(position.Row, position.Column - 1),
                new CellPosition(position.Row, position.Column + 1),
                new CellPosition(position.Row - 1, position.Column - 1),
                new CellPosition(position.Row - 1, position.Column + 1),
                new CellPosition(position.Row + 1, position.Column - 1),
                new CellPosition(position.Row + 1, position.Column + 1)
            };

            return adjacentPositions.Any(p => board.GetCell(p.Row, p.Column) == player);
        }
    }
}
