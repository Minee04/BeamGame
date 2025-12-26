namespace TicTacToe.Models
{
    /// <summary>
    /// Represents the result of checking the game state
    /// </summary>
    public class GameResult
    {
        public GameState State { get; set; }
        public PlayerType Winner { get; set; }

        public GameResult(GameState state, PlayerType winner = PlayerType.None)
        {
            State = state;
            Winner = winner;
        }
    }
}
