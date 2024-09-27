using System.Collections.Generic;

public class ConnectedStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
            new ButtonHelper("Join the queue", async () => await ServiceLocator.Services.HubService.JoinQueueAsync())
        };
    }

    public string InformationText => "You are connected. Click a button to join a queue";

    public string Scene => Scenes.MenuScene;
}