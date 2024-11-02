namespace HiveGame.Models
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
