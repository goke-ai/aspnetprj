using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Data(summaries).ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


app.MapGet("/weatherforecast/{id}", (int id) =>
{
    IEnumerable<WeatherForecast> forecast = Data(summaries);

    try
    {
        var item = forecast.First(f => f.Id == id);
        return Results.Ok(item);
    }
    catch (System.Exception)
    {
        return Results.NotFound();
        // throw;
    }
})
.WithName("GetWeatherForecastById")
.WithOpenApi();


app.MapPost("/weatherforecast", (WeatherForecast weatherForecast) =>
{
    List<WeatherForecast> data = Data(summaries).ToList();
    data.Add(weatherForecast);

    return Results.Created($"/weatherforecast/{weatherForecast.Id}", weatherForecast);
})
.WithName("AddWeatherForecast")
.WithOpenApi();

app.MapPut("/weatherforecast/{id}", (int id, WeatherForecast weatherForecast) =>
{
    IEnumerable<WeatherForecast> forecast = Data(summaries);

    return Results.NoContent();
})
.WithName("UpdateWeatherForecast")
.WithOpenApi();

app.MapDelete("/weatherforecast/{id}", (int id) =>
{
    if (Data(summaries).FirstOrDefault(f => f.Id == id) is WeatherForecast forecast)
    {
        Data(summaries).ToList().Remove(forecast);
        return Results.NoContent();
    }

    return Results.NotFound();
})
.WithName("DeleteWeatherForecast")
.WithOpenApi();



app.Run();

static IEnumerable<WeatherForecast> Data(string[] summaries)
{
    return Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            index,
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ));
}
record WeatherForecast(int Id, DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
