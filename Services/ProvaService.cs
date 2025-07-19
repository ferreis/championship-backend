// Services/ProvaService.cs
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite; // Certifique-se de ter o pacote NuGet Microsoft.Data.Sqlite instalado
using System.Data; // Para IDataReader

public class ProvaService
{
    private readonly string _connectionString;

    public ProvaService(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Obtém uma prova pelo seu Id.
    /// </summary>
    /// <param name="id">O Id da prova.</param>
    /// <returns>A prova encontrada ou null se não existir.</returns>
    public Prova ObterPorId(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            "SELECT id, nome, tipo, modalidade, tempo_ou_colocacao, genero, categoria_etaria FROM Prova WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Prova
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Tipo = reader.GetString(reader.GetOrdinal("tipo")),
                Modalidade = reader.GetString(reader.GetOrdinal("modalidade")),
                TempoOuColocacao = reader.GetString(reader.GetOrdinal("tempo_ou_colocacao")),
                Genero = reader.GetString(reader.GetOrdinal("genero")),
                CategoriaEtaria = reader.GetString(reader.GetOrdinal("categoria_etaria"))
            };
        }

        return null;
    }

    /// <summary>
    /// Lista todas as provas.
    /// </summary>
    /// <returns>Uma lista de todas as provas.</returns>
    public List<Prova> Listar()
    {
        var provas = new List<Prova>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            "SELECT id, nome, tipo, modalidade, tempo_ou_colocacao, genero, categoria_etaria FROM Prova";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            provas.Add(new Prova
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Tipo = reader.GetString(reader.GetOrdinal("tipo")),
                Modalidade = reader.GetString(reader.GetOrdinal("modalidade")),
                TempoOuColocacao = reader.GetString(reader.GetOrdinal("tempo_ou_colocacao")),
                Genero = reader.GetString(reader.GetOrdinal("genero")),
                CategoriaEtaria = reader.GetString(reader.GetOrdinal("categoria_etaria"))
            });
        }

        return provas;
    }

    /// <summary>
    /// Adiciona uma nova prova ao banco de dados e retorna o ID gerado.
    /// </summary>
    /// <param name="prova">A prova a ser adicionada.</param>
    /// <returns>O ID da prova recém-adicionada.</returns>
    public int Adicionar(Prova prova) // <-- Retornando int
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Prova (nome, tipo, modalidade, tempo_ou_colocacao, genero, categoria_etaria)
            VALUES ($nome, $tipo, $modalidade, $tempoOuColocacao, $genero, $categoriaEtaria)";
        command.Parameters.AddWithValue("$nome", prova.Nome);
        command.Parameters.AddWithValue("$tipo", prova.Tipo);
        command.Parameters.AddWithValue("$modalidade", prova.Modalidade);
        command.Parameters.AddWithValue("$tempoOuColocacao", prova.TempoOuColocacao);
        command.Parameters.AddWithValue("$genero", prova.Genero);
        command.Parameters.AddWithValue("$categoriaEtaria", prova.CategoriaEtaria);
        command.ExecuteNonQuery();

        // Obter o ID da última linha inserida (específico para SQLite)
        command.CommandText = "SELECT last_insert_rowid()";
        var newId = (long)command.ExecuteScalar();
        return (int)newId; // Retorna o ID gerado
    }

    /// <summary>
    /// Atualiza uma prova existente no banco de dados.
    /// </summary>
    /// <param name="prova">A prova com os dados atualizados.</param>
    public void Atualizar(Prova prova)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Prova
            SET nome = $nome, tipo = $tipo, modalidade = $modalidade, tempo_ou_colocacao = $tempoOuColocacao, genero = $genero, categoria_etaria = $categoriaEtaria
            WHERE id = $id";
        command.Parameters.AddWithValue("$id", prova.Id);
        command.Parameters.AddWithValue("$nome", prova.Nome);
        command.Parameters.AddWithValue("$tipo", prova.Tipo);
        command.Parameters.AddWithValue("$modalidade", prova.Modalidade);
        command.Parameters.AddWithValue("$tempoOuColocacao", prova.TempoOuColocacao);
        command.Parameters.AddWithValue("$genero", prova.Genero);
        command.Parameters.AddWithValue("$categoriaEtaria", prova.CategoriaEtaria);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Deleta uma prova do banco de dados pelo seu Id.
    /// </summary>
    /// <param name="id">O Id da prova a ser deletada.</param>
    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Prova WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}