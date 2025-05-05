using System.Collections.Generic;

public class DisconnectedStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {
        };
    }

    public string InformationText => "You are disconnected. Click a button to get a token and connect";

    public string Scene => Scenes.MenuScene;
}
