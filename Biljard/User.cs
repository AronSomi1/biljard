public class User
{
    public string Username { get; set; }
    public bool Ready { get; set; }
    public List<int> Balls { get; set; } = new List<int>();

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

public class UsersBallChoice
{
    public string Username { get; set; }
    public List<int> Balls { get; set; }
}