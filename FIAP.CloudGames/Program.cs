using FIAP.CloudGames.API.Config;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Adicionar Configs
builder.Services.AddDependencyInjection();
builder.Services.AddAppConnections(builder.Configuration);
builder.Services.AddAuthenticationJwt(builder.Configuration);
builder.Services.AddControllerConfig(builder.Configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
