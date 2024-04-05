
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

app.MapPost("/createLobby", (LobbyHandler lb, BallCount BC) =>
{
    var id = lb.CreateLobby(BC.NbrOfBalls);
    return TypedResults.Created($"/lobby/{id}", new { id, BC.NbrOfBalls });
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

app.MapPost("/lobby/{id}/addUser", (LobbyHandler lb, int id, UserRequest userRequest) =>
{
    lb.GetLobby(id).AddUser(userRequest.Username);
    return TypedResults.Ok(new { id, userRequest.Username });

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

app.MapPost("/lobby/{id}/userReady", (LobbyHandler lb, int id, UserRequest userRequest) =>
{
    var lobby = lb.GetLobby(id);
    lobby.GetUser(userRequest.Username).Ready = !lobby.GetUser(userRequest.Username).Ready;
    return TypedResults.Ok(new { id, userRequest.Username });
}
);

app.MapGet("/lobby/{id}/NbrOfBalls", (LobbyHandler lb, int id) =>
{
    var lobby = lb.GetLobby(id);
    return TypedResults.Ok(new { id, lobby.NbrOfBalls });
});

app.MapPost("/lobby/{id}/ballChoice", (LobbyHandler lb, int id, UsersBallChoice UBC) =>
{
    var lobby = lb.GetLobby(id);
    lobby.GetUser(UBC.Username).Balls = UBC.Balls;
    return TypedResults.Ok(new { id, UBC.Username, UBC.Balls });
});


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




