
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Lobby>(new Lobby());
var app = builder.Build();

var users = new List<InMemoryUser>();

app.MapPost("/users/add", (UserData data) =>
{
    users.Add(new InMemoryUser(data.Username));
    return TypedResults.Created($"/users/{data.Username}", data);
});

app.MapGet("/users/getUsers", () =>
{
    return TypedResults.Ok(users);

});

app.MapPost("/lobby/add", (LobbyData data, Lobby lobby) =>
{
    lobby.addUser(new InMemoryUser(data.Username), data.Lobby);
    return TypedResults.Created($"/lobby/{data.Username}", data);
});

app.MapGet("/lobby/getLobby", (Lobby lobby) =>
{
    return TypedResults.Ok(lobby.lobbys);
});


app.Run();

public record UserData(string Username);
public record LobbyData(string Username, int Lobby);
