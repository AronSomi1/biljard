
public class Lobby
{
    public int Id { get; set; }
    public List<User> Users { get; set; } = new List<User>();
    public int NbrOfBalls { get; set; } = 3;

    public User GetUser(string username)
    {
        return Users.Find(user => user.Username == username);
    }
    public void AddUser(string username)
    {
        Users.Add(new User(username, false));
    }


    public bool CheckIfAllUsersReady()
    {
        foreach (var user in Users)
        {
            if (!user.Ready)
            {
                return false;
            }
        }
        return true;
    }

}