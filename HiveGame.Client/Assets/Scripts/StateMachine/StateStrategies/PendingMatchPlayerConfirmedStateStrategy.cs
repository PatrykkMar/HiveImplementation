using System.Collections.Generic;

public class PendingMatchPlayerConfirmedStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>
        {

        };
    }

    public string InformationText => "Found a player. You confirmed your participation in the game";

    public string Scene => Scenes.MenuScene;
}
