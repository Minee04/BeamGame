using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeamGame.GameLogic;
using BeamGame.Models;

namespace BeamGame.Tests
{
    [TestClass]
    public class GameBoardTests
    {
        [TestMethod]
        public void NewBoard_ShouldHaveBothBallsOnBeam()
        {
            var board = new GameBoard();
            
            Assert.IsTrue(board.Player1Ball.IsOnBeam);
            Assert.IsTrue(board.Player2Ball.IsOnBeam);
            Assert.IsFalse(board.Player1Ball.HasFallen);
            Assert.IsFalse(board.Player2Ball.HasFallen);
        }

        [TestMethod]
        public void MoveLeft_ShouldDecreasePosition()
        {
            var board = new GameBoard();
            var initialPos = board.Player1Ball.Position;
            
            board.UpdatePhysics(PlayerAction.MoveLeft, PlayerAction.None, 0.016);
            
            Assert.IsTrue(board.Player1Ball.Position < initialPos);
        }

        [TestMethod]
        public void MoveRight_ShouldIncreasePosition()
        {
            var board = new GameBoard();
            var initialPos = board.Player2Ball.Position;
            
            board.UpdatePhysics(PlayerAction.None, PlayerAction.MoveRight, 0.016);
            
            Assert.IsTrue(board.Player2Ball.Position > initialPos);
        }

        [TestMethod]
        public void Jump_ShouldRemoveFromBeam()
        {
            var board = new GameBoard();
            
            board.UpdatePhysics(PlayerAction.Jump, PlayerAction.None, 0.016);
            
            Assert.IsFalse(board.Player1Ball.IsOnBeam);
        }

        [TestMethod]
        public void Reset_ShouldRestoreBalls()
        {
            var board = new GameBoard();
            board.UpdatePhysics(PlayerAction.MoveRight, PlayerAction.None, 0.016);
            
            board.Reset();
            
            Assert.IsTrue(board.Player1Ball.IsOnBeam);
            Assert.IsTrue(board.Player2Ball.IsOnBeam);
        }
    }
}
