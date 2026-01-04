using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeamGame.GameLogic;
using BeamGame.Models;

namespace BeamGame.Tests
{
    [TestClass]
    public class GameEngineTests
    {
        [TestMethod]
        public void NewGame_ShouldInitializeInProgressState()
        {
            var engine = new GameEngine();
            var result = engine.CheckGameState();
            
            Assert.AreEqual(GameState.InProgress, result.State);
        }

        [TestMethod]
        public void Reset_ShouldResetToInitialState()
        {
            var engine = new GameEngine();
            engine.Step(PlayerAction.MoveLeft, PlayerAction.MoveRight);
            
            engine.Reset();
            var result = engine.CheckGameState();
            
            Assert.AreEqual(GameState.InProgress, result.State);
            Assert.IsTrue(engine.Board.Player1Ball.IsOnBeam);
            Assert.IsTrue(engine.Board.Player2Ball.IsOnBeam);
        }

        [TestMethod]
        public void Step_ShouldUpdateGameState()
        {
            var engine = new GameEngine();
            var initialPos = engine.Board.Player1Ball.Position;
            
            engine.Step(PlayerAction.MoveLeft, PlayerAction.None);
            
            Assert.AreNotEqual(initialPos, engine.Board.Player1Ball.Position);
        }

        [TestMethod]
        public void Player1FallsOff_ShouldMakePlayer2Winner()
        {
            var engine = new GameEngine();
            
            for (int i = 0; i < 100; i++)
            {
                engine.Step(PlayerAction.MoveRight, PlayerAction.None);
                var result = engine.CheckGameState();
                
                if (result.State != GameState.InProgress)
                {
                    Assert.AreEqual(Player.Player2, result.Winner);
                    return;
                }
            }
        }

        [TestMethod]
        public void Player2FallsOff_ShouldMakePlayer1Winner()
        {
            var engine = new GameEngine();
            
            for (int i = 0; i < 100; i++)
            {
                engine.Step(PlayerAction.None, PlayerAction.MoveLeft);
                var result = engine.CheckGameState();
                
                if (result.State != GameState.InProgress)
                {
                    Assert.AreEqual(Player.Player1, result.Winner);
                    return;
                }
            }
        }
    }
}
