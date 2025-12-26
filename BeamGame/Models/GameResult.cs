namespace BeamGame.Models
{
    /// <summary>
    /// Represents the result of checking the beam game state
    /// </summary>
    public class GameResult
    {
        public GameState State { get; set; }
        public double GameTime { get; set; }      // How long the game lasted
        public Player? Winner { get; set; }       // Who won (if any)

        public GameResult(GameState state, double gameTime = 0.0, Player? winner = null)
        {
            State = state;
            GameTime = gameTime;
            Winner = winner;
        }
    }
}
