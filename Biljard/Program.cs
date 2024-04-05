
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

    if (!lb.ContainsLobby((int)id))
    {
        errors.Add("id", ["Lobby not ContainsLobby"]);
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
    lb.GetLobby(id).AddUser(username);
    return TypedResults.Ok(new { id, username });

}).AddEndpointFilter(async (context, next) =>
{
    var id = context.GetArgument<int>(1);
    var lb = context.GetArgument<LobbyHandler>(0);
    var errors = new Dictionary<string, string[]>();

    if (!lb.ContainsLobby((int)id))
    {
        errors.Add("id", ["Lobby not ContainsLobby"]);
        return Results.ValidationProblem(errors);
    }
    return await next(context);
});

app.MapPost("/lobby/{id}/userReady", (LobbyHandler lb, int id, [FromBody] string Username) =>
{
    var lobby = lb.GetLobby(id);
    lobby.GetUser(Username).Ready = !lobby.GetUser(Username).Ready;
    return TypedResults.Ok(new { id, Username });
}
);

/* app.MapPost("/loby/{id}/startGame", (LobbyHandler lb, int id) =>
{
    var lobby = lb.GetLobby(id);
    if (lobby.CheckIfAllUsersReady())
    {
        return TypedResults.Ok(new { id });
    }
    return TypedResults.BadRequest(new { id });
}); */




app.MapGet("/lobby/{id}/queue", (LobbyHandler lb, int id) =>
{
    var users = lb.GetLobby(id).Users;
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

public class Lobby
{
    public int Id { get; set; }
    public List<User> Users { get; set; } = new List<User>();

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
