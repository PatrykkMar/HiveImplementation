using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Board;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.Core.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveGameTests.Models.Board
{
    public class VertexTests
    {

        private InsectFactory _factory;

        public VertexTests()
        {
            _factory = new InsectFactory();
        }

        [Theory]
        [InlineData(DirectionConsts.Direction.Right)]
        [InlineData(DirectionConsts.Direction.TopRight)]
        [InlineData(DirectionConsts.Direction.TopLeft)]
        [InlineData(DirectionConsts.Direction.Left)]
        [InlineData(DirectionConsts.Direction.BottomLeft)]
        [InlineData(DirectionConsts.Direction.BottomRight)]
        public void Constructor_WithDirection_InitializesCorrectly_ForAllDirections(DirectionConsts.Direction direction)
        {
            var baseVertex = new Vertex(3, 3);
            var expectedOffset = DirectionConsts.NeighborOffsetsDict[direction].To2D();

            var newVertex = new Vertex(baseVertex, direction);

            Assert.Equal(baseVertex.X + expectedOffset.X, newVertex.X);
            Assert.Equal(baseVertex.Y + expectedOffset.Y, newVertex.Y);
        }

        [Fact]
        public void AddInsectToStack_ShouldAddInsectToStack()
        {
            var vertex = new Vertex(3, 3);
            var insect1 = _factory.CreateInsect(InsectType.Queen, PlayerColor.White);
            var insect2 = _factory.CreateInsect(InsectType.Beetle, PlayerColor.Black);

            vertex.AddInsectToStack(insect1);
            vertex.AddInsectToStack(insect2);

            Assert.Equal(2, vertex.InsectStack.Count);
        }

        [Fact]
        public void AddInsectToStack_CheckCurrentInsect()
        {
            var vertex = new Vertex(3, 3);
            var insect1 = _factory.CreateInsect(InsectType.Queen, PlayerColor.White);
            var insect2 = _factory.CreateInsect(InsectType.Beetle, PlayerColor.Black);

            vertex.AddInsectToStack(insect1);
            vertex.AddInsectToStack(insect2);

            Assert.Equal(insect2, vertex.CurrentInsect);
        }

    }
}
