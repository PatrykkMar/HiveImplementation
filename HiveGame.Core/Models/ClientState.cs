namespace HiveGame.Core.Models
{
    public enum ClientState
    {
        Disconnected,
        Connected,
        WaitingInQueue,
        PendingMatchWaitingForConfirmation,
        PendingMatchPlayerConfirmed,
        InGamePlayerFirstMove,
        InGamePlayerMove,
        InGameOpponentMove,
        GameOver
    }
}
