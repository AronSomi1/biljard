interface PoolUser
{

}

class InMemoryUser : PoolUser
{
    public string Username { get; set; }

    public InMemoryUser(string username)
    {
        this.Username = username;
    }
}