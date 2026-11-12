using FIAP.CloudGames.API.Config;

var builder = WebApplication.CreateBuilder(args);

//Adicionar Injeção de Dependência
builder.Services.AddDependencyInjection();
builder.Services.AddAppConnections(builder.Configuration);
builder.Services.AddAuthenticationJwt(builder.Configuration);


// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
