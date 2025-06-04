using HiveGame.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameOverStateStrategy : IStateStrategy
{
    public string InformationText => "The game is over";

    public string Scene => Scenes.GameScene;

    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
            new ButtonHelper("Exit the game", async () => ExitGame()),
            new ButtonHelper("Go back to joining queue screen", async () => await JoinQueueScreenAsync())
        };
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private async Task JoinQueueScreenAsync()
    {
        await ServiceLocator.Services.ClientStateMachine.ChangeStateAsync(ClientState.Connected);
    }
}