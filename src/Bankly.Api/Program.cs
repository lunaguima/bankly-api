using Bankly.Infrastructure.Persistence;
using Bankly.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Bankly.Application.Services;
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do Swagger para ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();