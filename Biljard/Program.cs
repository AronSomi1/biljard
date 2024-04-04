
using System.Numerics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<LobbyHandler>(new LobbyHandler());


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles(); // Enable serving static files
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.MapPost("/createLobby", (LobbyHandler lb) =>
{
    var id = lb.CreateLobby();
    return TypedResults.Created($"/lobby/{id}", new { id });
});

app.MapPost("/joinLobby/{id}", (LobbyHandler lb, int id) =>
{
    return TypedResults.Ok(new { id });

}).AddEndpointFilter(async (context, next) =>
{
    var id = context.GetArgument<int>(1);
    var lb = context.GetArgument<LobbyHandler>(0);
    var errors = new Dictionary<string, string[]>();

    if (!lb.Found((int)id))
    {
        errors.Add("id", ["Lobby not found"]);
        return Results.ValidationProblem(errors);
    }
    return await next(context);
});

app.MapGet("/lobby/{id}", async (HttpContext context, string id) =>
{
    var filePath = Path.Combine(app.Environment.WebRootPath, "lobby.html");
    await context.Response.SendFileAsync(filePath);

});

app.MapPost("/lobby/{id}/addUser", (LobbyHandler lb, int id, [FromBody] string username) =>
{
    lb.AddUserToLobby(id, username);
    return TypedResults.Ok(new { id, username });

}).AddEndpointFilter(async (context, next) =>
{
    var id = context.GetArgument<int>(1);
    var lb = context.GetArgument<LobbyHandler>(0);
    var errors = new Dictionary<string, string[]>();

    if (!lb.Found((int)id))
    {
        errors.Add("id", ["Lobby not found"]);
        return Results.ValidationProblem(errors);
    }
    return await next(context);
});

app.MapPost("/lobby/{id}/userReady", (LobbyHandler lb, int id, [FromBody] string Username) =>
{
    var lobby = lb.GetLobby(id);
    lobby.UserReady(Username);
    return TypedResults.Ok(new { id, Username });
}
);


app.MapGet("/lobby/{id}/queue", (LobbyHandler lb, int id) =>
{
    var users = lb.GetUsersInLobby(id);
    return TypedResults.Ok(users);
});


app.Run();


class LobbyHandler
{
    private readonly Dictionary<int, Lobby> lobbies = new();
    public Lobby GetLobby(int id)
    {
        return lobbies[id];
    }
    public Boolean Found(int value)
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
    public bool AddUserToLobby(int id, string username)
    {
        if (lobbies.ContainsKey(id))
        {
            lobbies[id].AddUser(username);
            return true;
        }
        return false;
    }
    public Dictionary<string, string> GetUsersInLobby(int id)
    {
        return lobbies[id].GetMappedUsersInLobby();
    }


}

public class Lobby
{
    public int Id { get; set; }
    public List<string> Usernames { get; set; } = new List<string>();
    private Dictionary<string, bool> UsersReady = new();

    public void AddUser(string username)
    {
        Usernames.Add(username);
        UsersReady.Add(username, false);
    }
    public void UserReady(string username)
    {
        UsersReady[username] = !UsersReady[username];

    }

    public bool IsUserReady(string username)
    {
        return UsersReady[username];
    }
    public bool CheckIfAllUsersReady()
    {
        foreach (var user in Usernames)
        {
            if (!UsersReady.ContainsKey(user))
            {
                return false;
            }
        }
        return true;
    }

    public Dictionary<string, string> GetMappedUsersInLobby()
    {
        Dictionary<string, string> users = new();

        foreach (var u in Usernames)
        {
            users.Add(u, IsUserReady(u).ToString());
        }
        return users;
    }


}