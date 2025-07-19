// Services/PontuacaoService.cs
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

// using SeuProjeto.Models; // Ajuste o namespace
// using SeuProjeto.Dtos;   // Ajuste o namespace

public class PontuacaoService
{
    private readonly string _connectionString;

    public PontuacaoService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Pontuacao ObterPorId(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, participacao_prova_id, pontos FROM Pontuacao WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Pontuacao
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                ParticipacaoProvaId = reader.GetInt32(reader.GetOrdinal("participacao_prova_id")),
                // Verifica se 'pontos' pode ser nulo no DB antes de tentar GetInt32
                Pontos = reader.IsDBNull(reader.GetOrdinal("pontos")) ? 0 : reader.GetInt32(reader.GetOrdinal("pontos"))
            };
        }
        return null;
    }

    public List<Pontuacao> Listar()
    {
        var pontuacoes = new List<Pontuacao>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, participacao_prova_id, pontos FROM Pontuacao";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            pontuacoes.Add(new Pontuacao
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                ParticipacaoProvaId = reader.GetInt32(reader.GetOrdinal("participacao_prova_id")),
                Pontos = reader.IsDBNull(reader.GetOrdinal("pontos")) ? 0 : reader.GetInt32(reader.GetOrdinal("pontos"))
            });
        }
        return pontuacoes;
    }

    public void Adicionar(Pontuacao pontuacao)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Pontuacao (participacao_prova_id, pontos) VALUES ($participacaoProvaId, $pontos)";
        command.Parameters.AddWithValue("$participacaoProvaId", pontuacao.ParticipacaoProvaId);
        command.Parameters.AddWithValue("$pontos", pontuacao.Pontos); // Certifique-se de que o tipo de 'pontos' no DB é compatível
        command.ExecuteNonQuery();
    }

    public void Atualizar(Pontuacao pontuacao)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Pontuacao SET participacao_prova_id = $participacaoProvaId, pontos = $pontos WHERE id = $id";
        command.Parameters.AddWithValue("$id", pontuacao.Id);
        command.Parameters.AddWithValue("$participacaoProvaId", pontuacao.ParticipacaoProvaId);
        command.Parameters.AddWithValue("$pontos", pontuacao.Pontos);
        command.ExecuteNonQuery();
    }

    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Pontuacao WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}