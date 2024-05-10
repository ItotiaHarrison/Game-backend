﻿using GameStore.Data;
using GameStore.Dtos;
using GameStore.Entities;

namespace GameStore.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";

    private static readonly List<GameDto> games = [
        new (1, "Super Mario Bros", "Platformer", 59.99m, new DateOnly(1985, 9, 13)),
        new (2, "The Legend of Zelda", "Action-adventure", 59.99m, new DateOnly(1986, 2, 21)),
        new (3, "Minecraft", "Sandbox", 26.95m, new DateOnly(2011, 11, 18))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app){

        var group = app.MapGroup("games")
        .WithParameterValidation();

    // GET /games
    group.MapGet("/",  () => games);

    // GET /games/1
    group.MapGet("/{id}", (int id) => 
    {
        GameDto? game = games.Find(game => game.Id == id);
        return game is null ? Results.NotFound() : Results.Ok(game);
    })
        .WithName(GetGameEndpointName);

    // POST /games
    group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) => {

        Game game = new(){
            Name = newGame.Name,
            Genre = dbContext.Genres.Find(newGame.GenreId),
            GenreId = newGame.GenreId,
            Price = newGame.Price,
            ReleaseDate = newGame.ReleaseDate
        };

        dbContext.Games.Add(game);
        dbContext.SaveChanges();

        GameDto gameDto = new(
            game.Id,
            game.Name,
            game.Genre!.Name,
            game.Price,
            game.ReleaseDate
        );

        return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, gameDto);
    });



    // PUT /games
    group.MapPut("/{id}", (int id, UpdateGameDto updatedGame)  => {
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
    group. MapDelete("/{id}",  (int id) => {
        games.RemoveAll(game => game.Id == id);

        return Results.NoContent();
    });

    return group;
    }

    private static void WithParameterValidation()
    {
        throw new NotImplementedException();
    }
}
