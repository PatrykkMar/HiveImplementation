using System.Collections.Generic;

public class InGameOpponentMoveStateStrategy : IStateStrategy
{
    public List<ButtonHelper> GetAvailableButtonsList()
    {
        return new List<ButtonHelper>();
    }

    public string InformationText => "Opponent's move\nWait for your turn";

    public string Scene => Scenes.GameScene;
}
