using System.Collections.Generic;

public class ConnectedStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
            new ButtonHelper("Join the queue", async () => {
                var nick = Board.Instance.PlayerNick;
                if(string.IsNullOrEmpty(nick))
                    ServiceLocator.Services.EventAggregator.InvokeMinorInformationTextReceived("There is no nick entered");
                else
                    await ServiceLocator.Services.HubService.JoinQueueAsync(nick);
                }
            )
        };
    }

    public string InformationText => "You are connected. Enter yout nick and click a button to join a queue";

    public string Scene => Scenes.MenuScene;
}