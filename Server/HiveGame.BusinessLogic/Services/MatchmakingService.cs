using AutoMapper;
using HiveGame.BusinessLogic.Managers;
using HiveGame.BusinessLogic.Models.Insects;
using HiveGame.BusinessLogic.Models.Requests;

namespace HiveGame.BusinessLogic.Services
{
    public interface IMatchmakingService
    {
        void AddToQueue(AddToQueue request);
    }

    public class MatchmakingService : IMatchmakingService
    {
        private readonly IWebSocketManager _gameManager;
        private Queue<string> _playerQueue = new Queue<string>();
        public MatchmakingService(IWebSocketManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void AddToQueue(AddToQueue request)
        {
            _playerQueue.Enqueue(request.Nick);

            if(_playerQueue.Count >= 2)
            {
                
            }
        }
    }
}