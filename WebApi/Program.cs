using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var movieDatabaseConfigSection = builder.Configuration.GetSection("DatabaseSettings");
builder.Services.Configure<DatabaseSettings>(movieDatabaseConfigSection);
builder.Services.AddSingleton<IMovieService, MongoMovieService>();
var app = builder.Build();

app.MapPost("/api/movies", (IMovieService movieservice, Movie movie) =>
{
    try
    {
        movieservice.Create(movie);
        return Results.Ok(movie);
    }
    catch (MongoWriteException)
    {
        return Results.BadRequest("Fehler beim Speichern. Existiert die ID bereits?");
    }
    catch (Exception)
    {
        return Results.Problem("Ein interner Serverfehler ist aufgetreten.");
    }
});

app.MapPut("/api/movies/{id}", (IMovieService movieservice,string id, Movie movie) =>
{
    if (id != movie.Id)
    {
        return Results.BadRequest("Movie konnte nicht gefunden werden!");
    }

    try
    {
        movieservice.Update(id, movie);
        return Results.NoContent();
    }
    catch (Exception)
    {
        return Results.Problem("Movie konnte nicht bearbeitet werden!");
    }

});

app.MapDelete("/api/movies/{id}", (IMovieService movieservice,string id) =>
{
    try
    {
        movieservice.Delete(id);
        return Results.NoContent();
    }
    catch (Exception)
    {
        return Results.Problem("Ein interner Serverfehler ist aufgetreten.");
    }
});

app.MapGet("/api/movies/{id}", (IMovieService movieservice,string id) =>
{
    try
    {
        return Results.Ok(movieservice.Get(id));
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
});


app.MapGet("/api/movies", (IMovieService movieservice) =>
{
    try
    {
        return Results.Ok(movieservice.Get());
    }
    catch (Exception)
    {
        return Results.NotFound();
    }
});

app.Run();
