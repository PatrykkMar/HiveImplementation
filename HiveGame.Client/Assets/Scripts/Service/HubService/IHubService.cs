using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using HiveGame.Core.Models;
using HiveGame.Core.Models.Requests;
using UnityEngine;

public interface IHubService
{
    event Action<ClientState> OnStateFromServerReceived;

    Task InitializeMatchmakingServiceAsync(string token);
    Task JoinQueueAsync(string nick);
    Task LeaveQueueAsync();
    Task PutInsectAsync(InsectType insect, (int, int, int) position);
    Task PutFirstInsectAsync(InsectType insect);
    Task MoveInsectAsync((int, int, int) moveFrom, (int, int, int) moveTo);
}