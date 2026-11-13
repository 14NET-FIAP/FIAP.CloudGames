using FIAP.CloudGames.API.Config;

var builder = WebApplication.CreateBuilder(args);

//Adicionar Injeção de Dependência
builder.Services.AddDependencyInjection();
builder.Services.AddAppConnections(builder.Configuration);
builder.Services.AddAuthenticationJwt(builder.Configuration);
builder.Services.AddSwaggerConfig(builder.Configuration);

builder.Services.AddControllers();

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
