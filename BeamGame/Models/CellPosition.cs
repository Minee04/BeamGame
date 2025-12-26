namespace BeamGame.Models
{
    /// <summary>
    /// Represents the physics state of a player's ball on the beam
    /// </summary>
    public class BallState
    {
        public double Position { get; set; }      // Position on beam: -1.0 (left edge) to 1.0 (right edge)
        public double Velocity { get; set; }      // Horizontal velocity of ball
        public double Acceleration { get; set; }  // Current acceleration
        public double VerticalPosition { get; set; } // Height above beam (for jumping)
        public double VerticalVelocity { get; set; } // Vertical velocity
        public bool IsOnBeam { get; set; }        // True if standing on beam
        public bool HasFallen { get; set; }       // True if fell off
        public double LandingImpact { get; set; }  // Force of landing (for beam physics)
        
        public const double JumpForce = 0.25;     // Reduced jump (was 0.3)
        public const double Gravity = 0.012;      // Less gravity (was 0.015)
        
        public BallState()
        {
            Position = 0.0;      // Start at center
            Velocity = 0.0;
            Acceleration = 0.0;
            VerticalPosition = 0.0;
            VerticalVelocity = 0.0;
            IsOnBeam = true;
            HasFallen = false;
            LandingImpact = 0.0;
        }

        public BallState(double position, double velocity)
        {
            Position = position;
            Velocity = velocity;
            Acceleration = 0.0;
            VerticalPosition = 0.0;
            VerticalVelocity = 0.0;
            IsOnBeam = true;
            HasFallen = false;
            LandingImpact = 0.0;
        }

        public BallState Clone()
        {
            return new BallState(Position, Velocity) 
            { 
                Acceleration = Acceleration,
                VerticalPosition = VerticalPosition,
                VerticalVelocity = VerticalVelocity,
                IsOnBeam = IsOnBeam,
                HasFallen = HasFallen,
                LandingImpact = LandingImpact
            };
        }
        
        /// <summary>
        /// Makes the player jump if on the beam
        /// </summary>
        public void Jump()
        {
            if (IsOnBeam && VerticalPosition <= 0.01)
            {
                VerticalVelocity = JumpForce;
                IsOnBeam = false;
            }
        }
    }
}
