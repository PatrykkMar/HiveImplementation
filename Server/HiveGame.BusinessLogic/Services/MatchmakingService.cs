using AutoMapper;
using HiveGame.BusinessLogic.Managers;
using HiveGame.BusinessLogic.Models.Game;
using HiveGame.BusinessLogic.Models;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;
using System.Net.WebSockets;
using HiveGame.BusinessLogic.Models.WebSocketModels;
using HiveGame.BusinessLogic.Factories;
using HiveGame.BusinessLogic.Models.Game.Graph;

namespace HiveGame.BusinessLogic.Services
{
    public interface IMatchmakingService
    {
        Task<string[]?> JoinQueue(string clientId);
    }

    public class MatchmakingService : IMatchmakingService
    {
        private readonly Queue<string> _queue = new Queue<string>();
        public MatchmakingService()
        {

        }

        public async Task<string[]?> JoinQueue(string clientId)
        {
            _queue.Enqueue(clientId);
            while(_queue.Count >= 2)
            {
                return new string[] {_queue.Dequeue(), _queue.Dequeue};
            }
            return null;
        }
    }
}