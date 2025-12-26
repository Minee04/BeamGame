namespace BeamGame.Models
{
    /// <summary>
    /// Represents the tilt action for the beam
    /// </summary>
    public enum BeamAction
    {
        TiltLeft,
        TiltRight,
        Center
    }
    
    /// <summary>
    /// Represents which player
    /// </summary>
    public enum Player
    {
        Player1,
        Player2
    }
    
    /// <summary>
    /// Player input actions
    /// </summary>
    public enum PlayerAction
    {
        MoveLeft,
        MoveRight,
        Jump,
        None
    }
}
