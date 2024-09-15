using System.Collections.Generic;

public class InGamePlayerMoveStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
            new ButtonHelper("Put insect", async () => await ServiceLocator.Services.HubService.PutInsectAsync(PlayerInsectView.ChosenInsect.Value, null)),
            new ButtonHelper("Move insect", async () => await ServiceLocator.Services.HubService.MoveInsectAsync())
        };
    }

    public string InformationText => "Your move";
}
