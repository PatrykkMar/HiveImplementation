using HiveGame.Core.Models;

public class GameSceneUIManager : UIManager
{
    public override string Name
    {
        get
        {
            return "Game";
        }
    }

    public override void SetUIAfterStateChange(ClientState state)
    {
        base.SetUIAfterStateChange(state);
        Board.Instance.UpdateBoardUI();
        Board.Instance.UpdatePlayerInsectsUI();
    }

}