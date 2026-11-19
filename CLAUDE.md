# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Visão geral

API REST ASP.NET Core (.NET 8) do Tech Challenge FIAP — plataforma de jogos (usuários, jogos, promoções, biblioteca do usuário). Autenticação via JWT, persistência em SQL Server.

## Comandos

```bash
dotnet build FIAP.CloudGames.sln
dotnet run --project FIAP.CloudGames/FIAP.CloudGames.API.csproj   # http://localhost:5105 | https://localhost:7161 (Swagger só em Development)
dotnet test FIAP.CloudGames.sln
dotnet test --filter "FullyQualifiedName~NomeDaClasseOuMetodo"     # teste único

# Migrations (EF Core): DbContext no projeto DataBase, startup no API
dotnet ef migrations add <Nome> --project FIAP.CloudGames.DataBase --startup-project FIAP.CloudGames
dotnet ef database update      --project FIAP.CloudGames.DataBase --startup-project FIAP.CloudGames
```

Não há lint/formatter configurado. Testes usam xUnit; os projetos de teste ainda contêm apenas placeholders e não referenciam os projetos de produção (adicione `ProjectReference` ao criar testes reais). Note que a pasta `FCG.Domain.Tests/` contém o projeto `FIAP.CloudGames.Domain.Tests`.

Connection string `SqlServer` em `appsettings.json` aponta para `localhost\SQLEXPRESS` com Trusted Connection. A seção `MongoDb` é resquício da versão anterior (o código Mongo está comentado em `ConnectionsConfiguration`).

O `Dockerfile` referencia `FIAP.CloudGames/FIAP.CloudGames.csproj` / `FIAP.CloudGames.dll`, mas o projeto real é `FIAP.CloudGames.API.csproj` — o build Docker não funciona sem ajuste.

## Arquitetura

Camadas (pasta → assembly):

- `FIAP.CloudGames/` (**API**, assembly `FIAP.CloudGames.API`): controllers, `Program.cs`, e `Config/` com extension methods de `IServiceCollection` chamados pelo `Program.cs` (`AddDependencyInjection`, `AddAppConnections`, `AddAuthenticationJwt`, `AddControllerConfig`). **Todo novo service/repository deve ser registrado em `Config/DependencyInjectionConfiguration.cs`** (scoped, separado por `#region`).
- `FIAP.CloudGames.Application/`: services (`IXxxService` em `Interfaces/`), DTOs por agregado em `DTOs/<Agregado>/`, e `Mappings/` com classes estáticas de mapeamento manual DTO↔entidade (sem AutoMapper).
- `FIAP.CloudGames.Domain/`: entidades, enums, `ValueObjects/` (validações estáticas que lançam `DomainException`), interfaces de repositório (`Repositories/`), `ReadModels/` (projeções para Dapper) e `Services/Result.cs`.
- `FIAP.CloudGames.DataBase/`: `AppDbContext`, `Configurations/` (`IEntityTypeConfiguration<T>`, aplicadas via `ApplyConfigurationsFromAssembly`), `Migrations/` e implementações dos repositórios. Referencia Application (alguns repositórios usam `Application.Mappings`).
- `FIAP.CloudGames.Infra/`: `SqlServerConnectionFactory` (conexões `IDbConnection` para Dapper) e `SecurityHelper` (hash SHA256 de senha e geração de JWT).

### Padrões transversais

- **Result pattern**: services não lançam exceção para fluxos de negócio; retornam `Result`, `Result<T>` ou `ResultList<T>` (`Domain/Services/Result.cs`) com `IsSuccess`/`Message`. Controllers fazem `if (!result.IsSuccess) return BadRequest(result);` e depois `Ok(result.Value)` / `Created(...)`. Services envolvem a lógica em `try/catch` convertendo exceções em `Failure` (relançando `OperationCanceledException`).
- **Acesso a dados híbrido**: escrita e checagens simples via EF Core (`AppDbContext`, `SaveChanges` síncrono); leituras de listagem via Dapper com SQL raw (`const string sql = """..."""` + `CommandDefinition` com `CancellationToken`). Repositórios recebem ambos (`AppDbContext` e `ISqlServerConnectionFactory`).
- **Autorização**: controllers usam `[Authorize]`; ações administrativas usam o filtro `[IsAdmin]` (`API/Attributes`), que lê o tipo de usuário da claim `ClaimTypes.Actor` (gravada por `SecurityHelper.GenerateJwtToken`) comparando com `TypeUserEnum.Admin`.
- **Contexto do usuário**: `IUserContextService` (Domain) / `UserContextService` (Application) e `UserContextMiddleware` existem, mas o middleware **não está registrado** no pipeline nem o service no DI. Ele também lê o tipo de usuário de `ClaimTypes.System`, divergente da claim `Actor` emitida no token.
- **Erros e logs**: `ExceptionHandlingMiddleware` converte exceções não tratadas em `ErrorResult` (um `Result` com `TraceId`): `DomainException` → 400, `KeyNotFoundException` → 404, demais → 500 com mensagem genérica. `RequestLoggingMiddleware` registra método, path, status, duração e userId. Use `ILogger<T>` via DI com message templates (`{Propriedade}`): o provider `MongoLoggerProvider` (Infra/Logging, alias `MongoDb`) grava cada log com propriedades e escopos na collection `ApplicationLogs` (`MongoDb:LogsCollectionName`), de forma assíncrona e em lote via `MongoLogWriterService`. Os níveis gravados são filtrados em `Logging:MongoDb:LogLevel`.
- JSON de resposta: nulls omitidos e indentado (`ControllerConfiguration`).
- Mensagens de erro/validação ao usuário estão majoritariamente em português.
