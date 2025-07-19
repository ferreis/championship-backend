// Program.cs

var builder = WebApplication.CreateBuilder(args);

// Habilita CORS para aceitar qualquer origem, método e header
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Habilita controllers
builder.Services.AddControllers();

var app = builder.Build();

// Ativa o uso de CORS
app.UseCors();

// Mapeia as rotas dos controllers
app.MapControllers();

// Define o caminho e a string de conexão para o banco de dados
string dbPath = "Database/sobrasa_banco_de_dados.db";
string connectionString = $"Data Source={dbPath}";

// Garante que o diretório do banco de dados exista
Directory.CreateDirectory(Path.GetDirectoryName(dbPath));

// 1. Inicializa o schema do banco de dados (cria as tabelas)
DatabaseInitializer.Initialize(dbPath);

// 2. Popula o banco de dados com dados de teste (apenas se estiver vazio)
DataSeeder.Seed(connectionString);

app.Run();