using FIAP.CloudGames.API.Config;
using FIAP.CloudGames.API.Middlewares;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Adicionar Configs
builder.Services.AddDependencyInjection();
builder.Services.AddAppConnections(builder.Configuration);
builder.Services.AddAuthenticationJwt(builder.Configuration);
builder.Services.AddControllerConfig(builder.Configuration);
builder.Services.AddMongoDbLogging(builder.Configuration);


var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
