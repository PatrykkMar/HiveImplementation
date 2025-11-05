using System.Collections.Generic;

public class PendingMatchWaitingForConfirmationStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
            new ButtonHelper("Confirm", async () => await ServiceLocator.Services.HubService.ConfirmGameAsync())
        };
    }

    public string InformationText => "Found a player. Confirm your participation in the game in 10 seconds";

    public string Scene => Scenes.MenuScene;
}
