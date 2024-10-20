using AutoMapper;
using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HiveGame.BusinessLogic.Models
{
    public class Game
    {
        public Game(Player[] players, IInsectFactory factory, IMapper mapper, PlayerColor startingColor = PlayerColor.White)
        {
            Board = new HiveBoard(factory, mapper);
            Players = players;
            CurrentColorMove = startingColor;
            players[0].PlayerColor = PlayerColor.White;
            players[1].PlayerColor = PlayerColor.Black;
        }

        public HiveBoard Board { get; set; }
        public Player[] Players { get; set; }
        public PlayerColor CurrentColorMove { get; set; }
        public Player GetCurrentPlayer()
        {
            return Players.FirstOrDefault(x => x.PlayerColor == CurrentColorMove);
        }
        public Player GetOtherPlayer()
        {
            return Players.FirstOrDefault(x => x.PlayerColor != CurrentColorMove);
        }

        public PlayerViewDTO GetPlayerView(string playerId)
        {
            var playerViewDTO = new PlayerViewDTO();
            var player = Players.FirstOrDefault(x=>x.PlayerId == playerId);
            playerViewDTO.PlayerInsects = player.PlayerInsects;
            playerViewDTO.Board = Board.VerticesDTO;
            return playerViewDTO;
        }

        public void AfterActionMade()
        {
            CurrentColorMove = (PlayerColor)(((int)(CurrentColorMove + 1)) % 2);
        }

    }
}
