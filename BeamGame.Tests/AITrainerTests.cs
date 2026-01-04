using Microsoft.VisualStudio.TestTools.UnitTesting;
using BeamGame.AI;
using BeamGame.Models;

namespace BeamGame.Tests
{
    [TestClass]
    public class AITrainerTests
    {
        [TestMethod]
        public void TrainAI_ShouldCompleteAllGames()
        {
            var ai = new QLearningAI();
            var trainer = new AITrainer(ai);
            
            var stats = trainer.TrainAI(10);
            
            Assert.AreEqual(10, stats.TotalGames);
        }

        [TestMethod]
        public void TrainingStats_ShouldSumToTotalGames()
        {
            var ai = new QLearningAI();
            var trainer = new AITrainer(ai);
            
            var stats = trainer.TrainAI(20);
            
            Assert.AreEqual(20, stats.Wins + stats.Losses + stats.Draws);
        }

        [TestMethod]
        public void TrainAI_ShouldReduceExploration()
        {
            var ai = new QLearningAI(explorationRate: 1.0);
            var trainer = new AITrainer(ai);
            var initialRate = ai.ExplorationRate;
            
            trainer.TrainAI(50);
            
            Assert.IsTrue(ai.ExplorationRate < initialRate);
        }

        [TestMethod]
        public void TrainAI_ShouldPopulateQTable()
        {
            var ai = new QLearningAI();
            var trainer = new AITrainer(ai);
            
            trainer.TrainAI(20);
            
            Assert.IsTrue(ai.QTableSize > 0);
        }

        [TestMethod]
        public void WinRate_ShouldCalculateCorrectly()
        {
            var stats = new TrainingStats
            {
                TotalGames = 100,
                Wins = 40,
                Losses = 55,
                Draws = 5
            };
            
            Assert.AreEqual(40.0, stats.WinRate);
        }

        [TestMethod]
        public void WinRate_WithZeroGames_ShouldBeZero()
        {
            var stats = new TrainingStats();
            
            Assert.AreEqual(0.0, stats.WinRate);
        }
    }
}
