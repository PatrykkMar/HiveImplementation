using System.Collections.Generic;

public class InGamePlayerFirstMoveStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
            new ButtonHelper("Put first insect", async () => await ServiceLocator.Services.HubService.PutFirstInsectAsync(PlayerInsectView.ChosenInsect.Value)),
        };
    }

    public string InformationText => "Your move";

    public string Scene => Scenes.GameScene;
}
