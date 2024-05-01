using GameStore.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const string GetGameEndpointName = "GetGame";

List<GameDto> games = [
    new (1, "Super Mario Bros", "Platformer", 59.99m, new DateOnly(1985, 9, 13)),
    new (2, "The Legend of Zelda", "Action-adventure", 59.99m, new DateOnly(1986, 2, 21)),
    new (3, "Minecraft", "Sandbox", 26.95m, new DateOnly(2011, 11, 18))
];


// GET /games
app.MapGet("games",  () => games);

// GET /games/1
app.MapGet("games/{id}", (int id) => 
{
    GameDto? game = games.Find(game => game.Id == id);
    return game is null ? Results.NotFound() : Results.Ok(game);
})
    .WithName(GetGameEndpointName);

// POST /games
app.MapPost("games", (CreateGameDto newGame) => {
    GameDto game = new(
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleaseDate
    );
    games.Add(game);

    return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
});



// PUT /games
app.MapPut("games/{id}", (int id, UpdateGameDto updatedGame)  => {
    var index = games.FindIndex(game => game.Id == id);

    if (index == -1){
        return Results.NotFound();
    }

    games[index] = new GameDto(
        id,
        updatedGame.Name,
        updatedGame.Genre,
        updatedGame.Price,
        updatedGame.ReleaseDate
    );

    return Results.NoContent();
});


// DELETE /games/1
app. MapDelete("games/{id}",  (int id) => {
    games.RemoveAll(game => game.Id == id);

    return Results.NoContent();
});

app.Run();
