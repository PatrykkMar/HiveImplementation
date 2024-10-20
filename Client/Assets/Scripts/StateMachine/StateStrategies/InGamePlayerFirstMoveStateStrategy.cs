using System.Collections.Generic;
using System.Threading.Tasks;

public class InGamePlayerFirstMoveStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
        };
    }

    public string InformationText => "Your first move\nClick insect to put it";

    public string Scene => Scenes.GameScene;

    public async Task PutFirstInsect()
    {
        if(!PlayerInsectView.ChosenInsect.HasValue)
        {
            ServiceLocator.Services.EventAggregator.InvokeInformationTextReceived("Insect to put not chosen");
            return;
        }


        await ServiceLocator.Services.HubService.PutFirstInsectAsync(PlayerInsectView.ChosenInsect.Value);
    }

    public void OnInsectButtonClick(InsectType insect)
    {
        if (!PlayerInsectView.ChosenInsect.HasValue)
        {
            ServiceLocator.Services.EventAggregator.InvokeInformationTextReceived("Insect to put not chosen");
            return;
        }

        ServiceLocator.Services.HubService.PutFirstInsectAsync(insect);
    }
}
