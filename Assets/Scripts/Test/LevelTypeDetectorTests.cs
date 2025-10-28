using NUnit.Framework;
using UnityEngine;

namespace Test
{
    [TestFixture]
    public class LevelTypeDetectorTests
    {
        [Test]
        public void DetectLevelType_NormalMode_ReturnsNormal()
        {
            // Arrange
            int maxMoves = 0;
            float timeLimit = 0;
            
            // Act
            LevelType result = LevelTypeDetector.DetectLevelType(maxMoves, timeLimit);
            
            // Assert
            Assert.AreEqual(LevelType.Normal, result);
        }
        
        [Test]
        public void DetectLevelType_TimerMode_ReturnsTimer()
        {
            // Arrange
            int maxMoves = 0;
            float timeLimit = 120;
            
            // Act
            LevelType result = LevelTypeDetector.DetectLevelType(maxMoves, timeLimit);
            
            // Assert
            Assert.AreEqual(LevelType.Timer, result);
        }
        
        [Test]
        public void DetectLevelType_MovesMode_ReturnsMoves()
        {
            // Arrange
            int maxMoves = 50;
            float timeLimit = 0;
            
            // Act
            LevelType result = LevelTypeDetector.DetectLevelType(maxMoves, timeLimit);
            
            // Assert
            Assert.AreEqual(LevelType.Moves, result);
        }
        
        [Test]
        public void DetectLevelType_HybridMode_ReturnsTimerWithWarning()
        {
            // Arrange
            int maxMoves = 50;
            float timeLimit = 120;
            
            // Act
            LevelType result = LevelTypeDetector.DetectLevelType(maxMoves, timeLimit);
            
            // Assert
            Assert.AreEqual(LevelType.Timer, result);
            // Note: Warning should be logged to console
        }
        
        [Test]
        public void DetectLevelType_InvalidValues_ReturnsNormal()
        {
            // Arrange
            int maxMoves = -1;
            float timeLimit = -1;
            
            // Act
            LevelType result = LevelTypeDetector.DetectLevelType(maxMoves, timeLimit);
            
            // Assert
            Assert.AreEqual(LevelType.Normal, result);
            // Note: Error should be logged to console
        }
        
        [Test]
        public void GetLevelTypeDisplayText_ReturnsCorrectText()
        {
            // Assert
            Assert.AreEqual("NORMAL", LevelTypeDetector.GetLevelTypeDisplayText(LevelType.Normal));
            Assert.AreEqual("TIMER", LevelTypeDetector.GetLevelTypeDisplayText(LevelType.Timer));
            Assert.AreEqual("MOVES", LevelTypeDetector.GetLevelTypeDisplayText(LevelType.Moves));
        }
        
        [Test]
        public void ValidateLevelConfiguration_ValidConfigurations_ReturnsTrue()
        {
            // Assert
            Assert.IsTrue(LevelTypeDetector.ValidateLevelConfiguration(0, 0)); // Normal
            Assert.IsTrue(LevelTypeDetector.ValidateLevelConfiguration(0, 120)); // Timer
            Assert.IsTrue(LevelTypeDetector.ValidateLevelConfiguration(50, 0)); // Moves
            Assert.IsTrue(LevelTypeDetector.ValidateLevelConfiguration(50, 120)); // Hybrid (allowed with warning)
        }
        
        [Test]
        public void ValidateLevelConfiguration_InvalidValues_ReturnsFalse()
        {
            // Assert
            Assert.IsFalse(LevelTypeDetector.ValidateLevelConfiguration(-1, 0)); // Negative moves
            Assert.IsFalse(LevelTypeDetector.ValidateLevelConfiguration(0, -1)); // Negative time
            Assert.IsFalse(LevelTypeDetector.ValidateLevelConfiguration(-1, -1)); // Both negative
        }
    }
}