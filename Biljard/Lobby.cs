class Lobby
{
    private int nbr = 4;
    public List<List<InMemoryUser>> lobbys { get; } = new List<List<InMemoryUser>>();
    public Lobby()
    {
        for (int i = 0; i < nbr; i++)
        {
            lobbys.Add(new List<InMemoryUser>());
        }

    }

    public void addUser(InMemoryUser user, int lobby)
    {
        lobbys[lobby].Add(user);
    }

    public void removeUser(InMemoryUser user, int lobby)
    {
        lobbys[lobby].RemoveAll(x => x.Username == user.Username);
    }
}