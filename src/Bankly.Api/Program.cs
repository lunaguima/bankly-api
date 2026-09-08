using System.Reflection;
using System.Text.Json.Serialization;
using Bankly.Api.Exceptions;
using Bankly.Api.HealthChecks;
using Bankly.Infrastructure.Persistence;
using Bankly.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Bankly.Application.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Oracle.EntityFrameworkCore.Infrastructure; // <-- Importante para o banco reconhecer a versão 19!

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 🚀 Configuração Mágica para o Oracle da FIAP (Versão 19)
// ---------------------------------------------------------
builder.Services.AddDbContext<BanklyContext>(options =>
    options.UseOracle(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        oracleOptions => oracleOptions.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19)
    ));

// ---------------------------------------------------------
// 💉 Injeção de Dependências (Clean Architecture)
// ---------------------------------------------------------
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountTypeRepository, AccountTypeRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

// ---------------------------------------------------------
// 🛑 Tratamento Global de Exceções
// ---------------------------------------------------------
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ---------------------------------------------------------
// 🩺 Health Checks (CP4)
// ---------------------------------------------------------
builder.Services.AddBanklyHealthChecks();

// ---------------------------------------------------------
// 📦 Controllers + Enums como texto (ex: "DEPOSITO" em vez de 0)
// ---------------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// ---------------------------------------------------------
// 📘 Swagger completo (título, versão, descrição + comentários XML)
// ---------------------------------------------------------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bankly API",
        Version = "v1",
        Description = "API REST do Bankly, sistema bancário desenvolvido para o Checkpoint 3 da FIAP. " +
                       "Expõe operações de usuários, endereços, contas, tipos de conta, cartões e transações, " +
                       "seguindo Clean Architecture e persistindo os dados em banco Oracle."
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Middleware de tratamento global de exceções (tem que vir antes dos demais)
app.UseExceptionHandler(_ => { });

// Configuração do Swagger para ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ---------------------------------------------------------
// 🩺 Endpoint /health (CP4)
// ---------------------------------------------------------
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.WriteResponse,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
}).ExcludeFromDescription();

app.Run();