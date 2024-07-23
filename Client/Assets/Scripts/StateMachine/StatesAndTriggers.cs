
public enum ClientState
{
    Nothing,
    Connected,
    WaitingForPlayers,
    InGame
}

public enum Trigger
{
    Started = 1,
    ReceivedToken = 2,
    JoinedQueue = 3,
    LeftQueue = 4,
    FoundGame = 5
}