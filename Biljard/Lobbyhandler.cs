
class LobbyHandler
{
    private readonly Dictionary<int, Lobby> lobbies = new();
    public Lobby GetLobby(int id)
    {
        return lobbies[id];
    }
    public Boolean ContainsLobby(int value)
    {
        return lobbies.ContainsKey(value);
    }

    public int CreateLobby()
    {
        Random rand = new Random();
        int id = rand.Next(1000, 9999);
        lobbies.Add(id, new Lobby() { Id = id });
        return id;
    }


}