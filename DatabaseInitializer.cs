// DatabaseInitializer.cs
using Microsoft.Data.Sqlite;

public static class DatabaseInitializer
{
    public static void Initialize(string dbPath)
    {
        var connectionString = $"Data Source={dbPath}";
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var command = connection.CreateCommand();

        // ** DEFINIÇÕES DE TABELAS (SCHEMA) **
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Pais (id INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT NOT NULL, nacionalidade TEXT NOT NULL);
            CREATE TABLE IF NOT EXISTS Estado (id INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT NOT NULL, sigla TEXT NOT NULL, pais_id INTEGER NOT NULL, FOREIGN KEY (pais_id) REFERENCES Pais(id));
            CREATE TABLE IF NOT EXISTS Cidade (id INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT NOT NULL, estado_id INTEGER NOT NULL, FOREIGN KEY (estado_id) REFERENCES Estado(id));
            CREATE TABLE IF NOT EXISTS Atleta (id INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT NOT NULL, cpf TEXT NOT NULL UNIQUE, genero TEXT CHECK (genero IN ('M', 'F')), data_nascimento DATE NOT NULL, pais_id INTEGER NOT NULL, FOREIGN KEY (pais_id) REFERENCES Pais(id));
            CREATE TABLE IF NOT EXISTS Equipe (id INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT NOT NULL, tipo TEXT CHECK (tipo IN ('Estadual', 'Nacional', 'Internacional', 'Clube', 'Força Armada')), estado_id INTEGER, pais_id INTEGER, FOREIGN KEY (estado_id) REFERENCES Estado(id), FOREIGN KEY (pais_id) REFERENCES Pais(id));
            CREATE TABLE IF NOT EXISTS Competicao (id INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT NOT NULL, data_inicio DATE, data_fim DATE, ano INTEGER, pais_id INTEGER, estado_id INTEGER, cidade_id INTEGER, FOREIGN KEY (pais_id) REFERENCES Pais(id), FOREIGN KEY (estado_id) REFERENCES Estado(id), FOREIGN KEY (cidade_id) REFERENCES Cidade(id));
            CREATE TABLE IF NOT EXISTS Atleta_Equipe (id INTEGER PRIMARY KEY AUTOINCREMENT, atleta_id INTEGER NOT NULL, equipe_id INTEGER NOT NULL, competicao_id INTEGER NOT NULL, FOREIGN KEY (atleta_id) REFERENCES Atleta(id), FOREIGN KEY (equipe_id) REFERENCES Equipe(id), FOREIGN KEY (competicao_id) REFERENCES Competicao(id));
            CREATE TABLE IF NOT EXISTS Prova (id INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT NOT NULL, tipo TEXT CHECK (tipo IN ('Praia', 'Piscina')), modalidade TEXT CHECK (modalidade IN ('Individual', 'Dupla', 'Revezamento')), tempo_ou_colocacao TEXT CHECK (tempo_ou_colocacao IN ('Tempo', 'Colocacao')), genero TEXT CHECK (genero IN ('M', 'F', 'unissex')), categoria_etaria TEXT);
            CREATE TABLE IF NOT EXISTS Participacao_Prova (id INTEGER PRIMARY KEY AUTOINCREMENT, atleta_id INTEGER, equipe_id INTEGER, prova_id INTEGER, competicao_id INTEGER, tempo NUMERIC, colocacao INTEGER, FOREIGN KEY (atleta_id) REFERENCES Atleta(id), FOREIGN KEY (equipe_id) REFERENCES Equipe(id), FOREIGN KEY (prova_id) REFERENCES Prova(id), FOREIGN KEY (competicao_id) REFERENCES Competicao(id));
            CREATE TABLE IF NOT EXISTS Pontuacao (id INTEGER PRIMARY KEY AUTOINCREMENT, participacao_prova_id INTEGER, pontos INTEGER, FOREIGN KEY (participacao_prova_id) REFERENCES Participacao_Prova(id));
            CREATE TABLE IF NOT EXISTS Categoria (id INTEGER PRIMARY KEY AUTOINCREMENT, descricao TEXT, idade_min INTEGER, idade_max INTEGER);
        ";
        command.ExecuteNonQuery();
    }
}