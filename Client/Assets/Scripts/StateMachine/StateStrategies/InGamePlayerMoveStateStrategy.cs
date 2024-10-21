using System.Collections.Generic;

public class InGamePlayerMoveStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
            //new ButtonHelper("Put insect", async () => await ServiceLocator.Services.HubService.PutInsectAsync(PlayerInsectView.ChosenInsect.Value, null)),
            //new ButtonHelper("Move insect", async () => await ServiceLocator.Services.HubService.MoveInsectAsync())
        };
    }

    public string InformationText => "Your move\nTo put an insect, click an insect button and choose one of the half-transparent hex on the board\nTo move an insect, click on a insect and choose one of the half-transparent hex on the board";

    public string Scene => Scenes.GameScene;

    public void OnInsectButtonClick(InsectType insect)
    {
        
    }

    public void OnHexClick(VertexDTO hex)
    {

    }
}
