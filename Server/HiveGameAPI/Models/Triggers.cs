namespace HiveGame.Models
{
    public enum Trigger
    {
        Started = 1,
        ReceivedToken = 2,
        JoinedQueue = 3,
        LeftQueue = 4,
        FoundGamePlayerStarts = 5,
        FoundGameOpponentStarts = 51,
        PlayerMove = 6,
        PlayerFirstMove = 60,
        PlayerMadeMove = 61
    }
}
