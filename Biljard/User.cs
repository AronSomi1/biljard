public class User
{
    public string Username { get; set; }
    public bool Ready { get; set; }

    public User(string username, bool ready)
    {
        Username = username;
        Ready = ready;
    }
}

public class UserRequest
{
    public string Username { get; set; }
}