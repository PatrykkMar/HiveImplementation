using HiveGame.BusinessLogic.Models.Board;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models;
using HiveGame.Core.Models;
using HiveGame.DataAccess.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HiveGame.BusinessLogic.Factories;

namespace HiveGame.BusinessLogic.Utils
{
    public interface IGameConverter
    {
        GameDbModel ToGameDbModel(Game game);
        Game FromGameDbModel(GameDbModel gameDbModel);
    }

    public class GameConverter : IGameConverter
    {
        private readonly IInsectFactory _insectFactory;

        public GameConverter(IInsectFactory insectFactory)
        {
            _insectFactory = insectFactory;
        }

        public GameDbModel ToGameDbModel(Game game)
        {
            return new GameDbModel
            {
                Id = game.Id,
                NumberOfMove = game.NumberOfMove,
                CurrentColorMove = game.CurrentColorMove.ToString(),
                Board = game.Board.Vertices.Select(vertex => new VertexDbModel
                {
                    Id = vertex.Id,
                    X = vertex.X,
                    Y = vertex.Y,
                    InsectStack = vertex.InsectStack.Reverse().Select(insect => new InsectDbModel
                    {
                        Type = insect.Type.ToString(),
                        PlayerColor = insect.PlayerColor.ToString()
                    }).ToList()
                }).ToList(),
                Players = game.Players.Select(player => new PlayerDbModel
                {
                    PlayerId = player.PlayerId,
                    PlayerColor = player.PlayerColor.ToString(),
                    PlayerInsects = player.PlayerInsects.ToDictionary(
                        kvp => kvp.Key.ToString(),
                        kvp => kvp.Value
                    ),
                    PlayerState = player.PlayerState.ToString(),
                    PlayerNick = player.PlayerNick
                }).ToList()
            };
        }

        public Game FromGameDbModel(GameDbModel gameDbModel)
        {
            var players = gameDbModel.Players.Select(player => new Player
            {
                PlayerId = player.PlayerId,
                PlayerColor = Enum.Parse<PlayerColor>(player.PlayerColor),
                PlayerInsects = player.PlayerInsects.ToDictionary(
                    kvp => Enum.Parse<InsectType>(kvp.Key),
                    kvp => kvp.Value
                ),
                PlayerState = Enum.Parse<ClientState>(player.PlayerState),
                PlayerNick = player.PlayerNick
            }).ToArray();

            var game = new Game(players, Enum.Parse<PlayerColor>(gameDbModel.CurrentColorMove))
            {
                Id = gameDbModel.Id.ToString(),
                NumberOfMove = gameDbModel.NumberOfMove,
                Board = new HiveBoard()
            };

            foreach (var vertexDb in gameDbModel.Board)
            {
                var vertex = new Vertex(vertexDb.X, vertexDb.Y)
                {
                    Id = vertexDb.Id
                };

                foreach (var insectDb in vertexDb.InsectStack)
                {
                    var insect = _insectFactory.CreateInsect(Enum.Parse<InsectType>(insectDb.Type), Enum.Parse<PlayerColor>(insectDb.PlayerColor));
                    vertex.AddInsectToStack(insect);
                }

                game.Board.AddVertex(vertex);
            }

            return game;
        }
    }
}
