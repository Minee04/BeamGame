namespace BeamGame.Models
{
    /// <summary>
    /// Represents the state of the beam
    /// </summary>
    public class BeamState
    {
        public double Angle { get; set; }         // Beam angle in degrees: -40 to +40
        public BeamAction CurrentAction { get; set; }
        
        // Physics constants
        public const double MaxAngle = 40.0; // Increased from 30.0
        public const double AngleChangeRate = 1.2; // Slower tilting (was 2.0)
        public const double ReturnToCenter = 1.5;  // Faster return to center (was 1.0)

        public BeamState()
        {
            Angle = 0.0;
            CurrentAction = BeamAction.Center;
        }

        public BeamState Clone()
        {
            return new BeamState { Angle = Angle, CurrentAction = CurrentAction };
        }
    }
}
