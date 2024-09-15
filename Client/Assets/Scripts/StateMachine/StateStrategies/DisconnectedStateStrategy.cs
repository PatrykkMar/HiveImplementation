using System.Collections.Generic;

public class DisconnectedStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
            new ButtonHelper("Get token", () => ServiceLocator.Services.HttpService.GetToken())
        };
    }

    public string InformationText => "You are disconnected. Click a button to get a token and connect";
}
