using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeamGame.AI;
using BeamGame.GameLogic;
using BeamGame.Models;

namespace BeamGame.Tests
{
    [TestClass]
    public class QLearningAITests
    {
        [TestMethod]
        public void NewAI_ShouldStartWithFullExploration()
        {
            var ai = new QLearningAI();
            
            Assert.AreEqual(1.0, ai.ExplorationRate);
        }

        [TestMethod]
        public void NewAI_ShouldHaveEmptyQTable()
        {
            var ai = new QLearningAI();
            
            Assert.AreEqual(0, ai.QTableSize);
        }

        [TestMethod]
        public void DetermineNextAction_ShouldReturnValidAction()
        {
            var ai = new QLearningAI(explorationRate: 0.0);
            var engine = new GameEngine();
            
            var action = ai.DetermineNextAction(engine, Player.Player2);
            
            Assert.IsTrue(action == PlayerAction.MoveLeft || 
                         action == PlayerAction.MoveRight || 
                         action == PlayerAction.Jump || 
                         action == PlayerAction.None);
        }

        [TestMethod]
        public void AI_NearEdge_ShouldMoveTowardCenter()
        {
            var ai = new QLearningAI(explorationRate: 0.0);
            var engine = new GameEngine();
            
            // Push ball to edge
            for (int i = 0; i < 80; i++)
                engine.Step(PlayerAction.None, PlayerAction.MoveRight);
            
            var action = ai.DetermineNextAction(engine, Player.Player2);
            
            Assert.AreEqual(PlayerAction.MoveLeft, action);
        }

        [TestMethod]
        public void LearnFromGame_ShouldReduceExploration()
        {
            var ai = new QLearningAI(explorationRate: 1.0, explorationDecay: 0.99);
            var initialRate = ai.ExplorationRate;
            
            ai.LearnFromGame(0.0);
            
            Assert.IsTrue(ai.ExplorationRate < initialRate);
        }

        [TestMethod]
        public void LearnFromGame_ShouldPopulateQTable()
        {
            var ai = new QLearningAI();
            var engine = new GameEngine();
            
            for (int i = 0; i < 5; i++)
            {
                var state = ai.GetStateString(engine, Player.Player2);
                var action = ai.DetermineNextAction(engine, Player.Player2);
                engine.Step(PlayerAction.None, action);
                var nextState = ai.GetStateString(engine, Player.Player2);
                ai.RecordTransition(state, action, 0.5, nextState);
            }
            
            ai.LearnFromGame(10.0);
            
            Assert.IsTrue(ai.QTableSize > 0);
        }

        [TestMethod]
        public void ResetKnowledge_ShouldClearQTable()
        {
            var ai = new QLearningAI(explorationRate: 0.5);
            ai.RecordTransition("state", PlayerAction.None, 1.0, "next");
            ai.LearnFromGame(10.0);
            
            ai.ResetKnowledge();
            
            Assert.AreEqual(0, ai.QTableSize);
            Assert.AreEqual(1.0, ai.ExplorationRate);
        }
    }
}
