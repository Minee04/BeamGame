namespace TicTacToe.Models
{
    /// <summary>
    /// Represents a position on the game board
    /// </summary>
    public class CellPosition
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public CellPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            if (obj is CellPosition other)
            {
                return Row == other.Row && Column == other.Column;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (Row * 397) ^ Column;
        }
    }
}
