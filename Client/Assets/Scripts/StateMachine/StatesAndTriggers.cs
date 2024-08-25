
public enum ClientState
{
    Disconnected,
    Connected,
    WaitingForPlayers,
    InGamePlayerFirstMove,
    InGamePlayerMove,
    InGameOpponentMove
}

public enum Trigger
{
    Started = 1,
    ReceivedToken = 2,
    JoinedQueue = 3,
    LeftQueue = 4,
    FoundGamePlayerStarts = 5,
    FoundGameOpponentStarts = 51,
    OpponentMadeMove = 6,
    PlayerFirstMove = 60,
    PlayerMadeMove = 61
}