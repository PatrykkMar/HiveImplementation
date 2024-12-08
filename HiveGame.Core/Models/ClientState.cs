namespace HiveGame.Core.Models
{
    public enum ClientState
    {
        Disconnected,
        Connected,
        WaitingForPlayers,
        InGamePlayerFirstMove,
        InGamePlayerMove,
        InGameOpponentMove,
        GameOver
    }
}
