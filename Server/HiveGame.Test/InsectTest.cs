using NUnit.Framework;
using System;

namespace HiveGame.Test
{
    public class InsectTests
    {
        [Test]
        public void QueenCanMove()
        {
            // Arrange
            var queen = InsectFactory.CreateInsect(InsectType.Queen);

            // Act
            var result = queen.CanMove();

            // Assert
            Assert.Is
            Assert.True(result);
        }

        [Test]
        public void AntCanMove()
        {
            // Arrange
            var ant = InsectFactory.CreateInsect(InsectType.Ant);

            // Act
            var result = ant.CanMove();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void SpiderCanMove()
        {
            // Arrange
            var spider = InsectFactory.CreateInsect(InsectType.Spider);

            // Act
            var result = spider.CanMove();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void GrasshopperCanMove()
        {
            // Arrange
            var grasshopper = InsectFactory.CreateInsect(InsectType.Grasshopper);

            // Act
            var result = grasshopper.CanMove();

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void BeetleCanMove()
        {
            // Arrange
            var beetle = InsectFactory.CreateInsect(InsectType.Beetle);

            // Act
            var result = beetle.CanMove();

            // Assert
            Assert.IsTrue(result);
        }
    }
}