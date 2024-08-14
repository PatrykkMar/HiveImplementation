
public enum ClientState
{
    Disconnected,
    Connected,
    WaitingForPlayers,
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
    PlayerMadeMove = 6,
    OpponentMadeMove = 61
}