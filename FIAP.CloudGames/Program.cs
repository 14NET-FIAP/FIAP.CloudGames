using FIAP.CloudGames.API.Config;

var builder = WebApplication.CreateBuilder(args);

//Adicionar Injeção de Dependência
builder.Services.AddDependencyInjection();
builder.Services.AddAppConnections(builder.Configuration);
builder.Services.AddAuthenticationJwt(builder.Configuration);
builder.Services.AddSwaggerConfig(builder.Configuration);


// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Habilita a interface do Swagger apenas em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
