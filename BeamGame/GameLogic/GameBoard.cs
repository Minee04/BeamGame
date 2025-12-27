using System;
using BeamGame.Models;

namespace BeamGame.GameLogic
{
    /// <summary>
    /// Manages the beam and 2-player ball physics simulation
    /// </summary>
    public class GameBoard
    {
        private BallState _player1Ball;
        private BallState _player2Ball;
        private BeamState _beam;
        
        // Physics constants - optimized for 60 FPS but scaled by delta time for consistency
        private const double BeamGravityFactor = 0.6;  // Per second (60 FPS: 0.01 per frame)
        private const double Friction = 0.98;      // Per frame (frame-rate independent dampening)
        private const double MaxVelocity = 1.2;   // Per second (60 FPS: 0.02 per frame)
        private const double EdgePosition = 1.0;   // Ball falls off at position > 1.0 or < -1.0
        private const double PlayerMoveSpeed = 0.18; // Per second (60 FPS: 0.003 per frame)

        public BallState Player1Ball => _player1Ball;
        public BallState Player2Ball => _player2Ball;
        public BeamState Beam => _beam;

        public GameBoard()
        {
            _player1Ball = new BallState() { Position = -0.3 }; // Start left of center
            _player2Ball = new BallState() { Position = 0.3 };  // Start right of center
            _beam = new BeamState();
        }

        /// <summary>
        /// Resets the game to initial state
        /// </summary>
        public void Reset()
        {
            _player1Ball = new BallState() { Position = -0.3 };
            _player2Ball = new BallState() { Position = 0.3 };
            _beam = new BeamState();
        }

        /// <summary>
        /// Updates physics for one time step with player inputs and delta time
        /// </summary>
        public void UpdatePhysics(PlayerAction player1Action, PlayerAction player2Action, double deltaTime = 0.0167)
        {
            // Handle player 1 input
            HandlePlayerInput(_player1Ball, player1Action, deltaTime);
            
            // Handle player 2 input
            HandlePlayerInput(_player2Ball, player2Action, deltaTime);
            
            // Update beam angle - tries to tilt toward heavier side
            UpdateBeamDynamically(deltaTime);
            
            // Update both players' physics
            UpdateBallPhysics(_player1Ball, deltaTime);
            UpdateBallPhysics(_player2Ball, deltaTime);
            
            // Check for minimal ball collision (very subtle to preserve AI strategy)
            CheckBallCollision();
            
            // Check if balls have fallen off
            CheckFallConditions();
        }

        private void HandlePlayerInput(BallState ball, PlayerAction action, double deltaTime)
        {
            if (ball.HasFallen) return;
            
            switch (action)
            {
                case PlayerAction.MoveLeft:
                    if (ball.IsOnBeam)
                    {
                        ball.Velocity -= PlayerMoveSpeed * deltaTime;
                    }
                    break;
                    
                case PlayerAction.MoveRight:
                    if (ball.IsOnBeam)
                    {
                        ball.Velocity += PlayerMoveSpeed * deltaTime;
                    }
                    break;
                    
                case PlayerAction.Jump:
                    ball.Jump();
                    break;
            }
        }

        private void UpdateBeamDynamically(double deltaTime)
        {
            // Beam tilts based on where the balls are (physics simulation)
            double torque = 0;
            
            if (!_player1Ball.HasFallen && _player1Ball.IsOnBeam)
            {
                // Base weight torque
                torque += _player1Ball.Position * 0.5;
                
                // Landing impact creates extra torque (jumps shake the beam!)
                torque += _player1Ball.Position * _player1Ball.LandingImpact * 0.3;
            }
            
            if (!_player2Ball.HasFallen && _player2Ball.IsOnBeam)
            {
                // Base weight torque
                torque += _player2Ball.Position * 0.5;
                
                // Landing impact creates extra torque
                torque += _player2Ball.Position * _player2Ball.LandingImpact * 0.3;
            }
            
            // Beam rotates toward the torque
            _beam.Angle += torque;
            
            // Damping - beam naturally returns to center
            _beam.Angle *= 0.95;
            
            // Clamp beam angle
            _beam.Angle = Math.Max(-BeamState.MaxAngle, Math.Min(BeamState.MaxAngle, _beam.Angle));
        }

        private void UpdateBallPhysics(BallState ball, double deltaTime)
        {
            if (ball.HasFallen) return;
            
            // Decay landing impact over time
            ball.LandingImpact *= 0.8;
            
            // Update vertical physics (jumping)
            if (!ball.IsOnBeam || ball.VerticalPosition > 0)
            {
                ball.VerticalVelocity -= BallState.Gravity; // Gravity pulls down
                ball.VerticalPosition += ball.VerticalVelocity;
                
                // Land back on beam
                if (ball.VerticalPosition <= 0)
                {
                    ball.VerticalPosition = 0;
                    
                    // Calculate landing impact based on fall velocity
                    ball.LandingImpact = Math.Abs(ball.VerticalVelocity) * 15.0; // Impact force
                    
                    ball.VerticalVelocity = 0;
                    ball.IsOnBeam = true;
                }
            }
            
            // Horizontal physics - only if on beam
            if (ball.IsOnBeam)
            {
                // Calculate ball acceleration based on beam angle
                double angleInRadians = _beam.Angle * Math.PI / 180.0;
                ball.Acceleration = Math.Sin(angleInRadians) * BeamGravityFactor * deltaTime;
                
                // Update ball velocity
                ball.Velocity += ball.Acceleration;
                ball.Velocity *= Friction; // Friction is per-frame, frame-rate independent
                
                // Clamp velocity (per-second rate scaled by deltaTime)
                double maxVel = MaxVelocity * deltaTime;
                ball.Velocity = Math.Max(-maxVel, Math.Min(maxVel, ball.Velocity));
            }
            
            // Update ball position (velocity is already scaled)
            ball.Position += ball.Velocity;
        }

        private void CheckBallCollision()
        {
            // Only check collision if both balls are on the beam
            if (!_player1Ball.IsOnBeam || !_player2Ball.IsOnBeam) return;
            if (_player1Ball.HasFallen || _player2Ball.HasFallen) return;
            
            // Calculate distance between balls (normalized positions on beam)
            double distance = Math.Abs(_player1Ball.Position - _player2Ball.Position);
            
            // Slightly tighter collision to be more forgiving
            const double BallRadiusNormalized = 0.045; // Slightly smaller (was 0.05)
            const double CollisionDistance = BallRadiusNormalized * 2;
            
            if (distance < CollisionDistance)
            {
                // Balanced collision - noticeable but not chaotic
                double overlap = CollisionDistance - distance;
                double pushForce = overlap * 0.25; // Gentler push (was 0.4)
                
                if (_player1Ball.Position < _player2Ball.Position)
                {
                    _player1Ball.Position -= pushForce;
                    _player2Ball.Position += pushForce;
                }
                else
                {
                    _player1Ball.Position += pushForce;
                    _player2Ball.Position -= pushForce;
                }
                
                // Softer bounce - mostly dampening, less chaotic bouncing
                double temp = _player1Ball.Velocity;
                _player1Ball.Velocity = _player2Ball.Velocity * 0.5; // Reduced (was 0.7)
                _player2Ball.Velocity = temp * 0.5;
                
                // Minimal beam shake - keep physics more predictable
                double collisionImpact = Math.Abs(temp - _player2Ball.Velocity) * 1.0; // Reduced (was 2.0)
                _player1Ball.LandingImpact += collisionImpact;
                _player2Ball.LandingImpact += collisionImpact;
            }
        }

        private void CheckFallConditions()
        {
            // Check if player 1 fell off
            if (!_player1Ball.HasFallen && Math.Abs(_player1Ball.Position) > EdgePosition)
            {
                _player1Ball.HasFallen = true;
                _player1Ball.IsOnBeam = false;
            }
            
            // Check if player 2 fell off
            if (!_player2Ball.HasFallen && Math.Abs(_player2Ball.Position) > EdgePosition)
            {
                _player2Ball.HasFallen = true;
                _player2Ball.IsOnBeam = false;
            }
        }

        /// <summary>
        /// Checks if a player has fallen off the beam
        /// </summary>
        public bool HasPlayer1Fallen() => _player1Ball.HasFallen;
        public bool HasPlayer2Fallen() => _player2Ball.HasFallen;

        /// <summary>
        /// Clones the current board state
        /// </summary>
        public GameBoard Clone()
        {
            var clone = new GameBoard();
            clone._player1Ball = _player1Ball.Clone();
            clone._player2Ball = _player2Ball.Clone();
            clone._beam = _beam.Clone();
            return clone;
        }
    }
}
