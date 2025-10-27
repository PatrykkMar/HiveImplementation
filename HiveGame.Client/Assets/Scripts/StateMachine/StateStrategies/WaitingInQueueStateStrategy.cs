using System.Collections.Generic;

public class WaitingInQueueStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
            new ButtonHelper("Leave the queue", async () => await ServiceLocator.Services.HubService.LeaveQueueAsync())
        };
    }

    public string InformationText => "You are in a queue, wait for another player. You can click a button to leave a queue";

    public string Scene => Scenes.MenuScene;
}
