using Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICounterRepository, CounterRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseCors(builder =>
{
    builder.WithOrigins("https://localhost:7173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


//app.MapPost("/api/person", async (PersonDto dto, HttpContext ctx) =>
//{
//    var idemKey = ctx.Request.Headers["X-Idempotency-Key"].FirstOrDefault();
//    // TODO: vérifier en base/cache si idemKey déjà traité
//    await Task.Delay(100);
//    return Results.Created($"/api/person/{Guid.NewGuid()}", dto);
//});


//public record PersonDto(string FullName, string Email);
