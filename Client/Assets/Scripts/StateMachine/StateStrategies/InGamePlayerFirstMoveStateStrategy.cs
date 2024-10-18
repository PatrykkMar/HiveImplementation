using System.Collections.Generic;
using System.Threading.Tasks;

public class InGamePlayerFirstMoveStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
            new ButtonHelper("Put first insect", async () => await PutFirstInsect()),
        };
    }

    public string InformationText => "Your move";

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
}
