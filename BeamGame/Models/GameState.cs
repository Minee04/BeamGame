namespace BeamGame.Models
{
    /// <summary>
    /// Represents the current state of the beam balance game
    /// </summary>
    public enum GameState
    {
        InProgress,
        Player1Wins,
        Player2Wins,
        BothFell,
        TimeExpired
    }
}
