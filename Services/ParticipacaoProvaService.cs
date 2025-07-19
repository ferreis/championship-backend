// Services/ParticipacaoProvaService.cs
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Data;

public class ParticipacaoProvaService
{
    private readonly string _connectionString;

    public ParticipacaoProvaService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public ParticipacaoProva? ObterPorId(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, atleta_id, equipe_id, prova_id, competicao_id, tempo, colocacao FROM Participacao_Prova WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new ParticipacaoProva
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                AtletaId = reader.GetInt32(reader.GetOrdinal("atleta_id")),
                EquipeId = reader.IsDBNull(reader.GetOrdinal("equipe_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("equipe_id")),
                ProvaId = reader.GetInt32(reader.GetOrdinal("prova_id")),
                CompeticaoId = reader.GetInt32(reader.GetOrdinal("competicao_id")),
                Tempo = reader.IsDBNull(reader.GetOrdinal("tempo")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("tempo")),
                Colocacao = reader.IsDBNull(reader.GetOrdinal("colocacao")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("colocacao"))
            };
        }
        return null;
    }

    public List<ParticipacaoProva> Listar()
    {
        var participacoes = new List<ParticipacaoProva>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, atleta_id, equipe_id, prova_id, competicao_id, tempo, colocacao FROM Participacao_Prova";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            participacoes.Add(new ParticipacaoProva
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                AtletaId = reader.GetInt32(reader.GetOrdinal("atleta_id")),
                EquipeId = reader.IsDBNull(reader.GetOrdinal("equipe_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("equipe_id")),
                ProvaId = reader.GetInt32(reader.GetOrdinal("prova_id")),
                CompeticaoId = reader.GetInt32(reader.GetOrdinal("competicao_id")),
                Tempo = reader.IsDBNull(reader.GetOrdinal("tempo")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("tempo")),
                Colocacao = reader.IsDBNull(reader.GetOrdinal("colocacao")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("colocacao"))
            });
        }
        return participacoes;
    }

    public int Adicionar(ParticipacaoProvaDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Participacao_Prova (atleta_id, equipe_id, prova_id, competicao_id, tempo, colocacao)
            VALUES ($atletaId, $equipeId, $provaId, $competicaoId, $tempo, $colocacao)";
        command.Parameters.AddWithValue("$atletaId", dto.AtletaId);
        command.Parameters.AddWithValue("$equipeId", (object?)dto.EquipeId ?? DBNull.Value);
        command.Parameters.AddWithValue("$provaId", dto.ProvaId);
        command.Parameters.AddWithValue("$competicaoId", dto.CompeticaoId);
        command.Parameters.AddWithValue("$tempo", (object?)dto.Tempo ?? DBNull.Value);
        command.Parameters.AddWithValue("$colocacao", (object?)dto.Colocacao ?? DBNull.Value);
        command.ExecuteNonQuery();

        command.CommandText = "SELECT last_insert_rowid()";
        var newId = (long?)command.ExecuteScalar();
        return (int)(newId ?? 0);
    }

    public void Atualizar(ParticipacaoProvaUpdateDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Participacao_Prova
            SET atleta_id = COALESCE($atletaId, atleta_id),
                equipe_id = COALESCE($equipeId, equipe_id),
                prova_id = COALESCE($provaId, prova_id),
                competicao_id = COALESCE($competicaoId, competicao_id),
                tempo = COALESCE($tempo, tempo),
                colocacao = COALESCE($colocacao, colocacao)
            WHERE id = $id";
        command.Parameters.AddWithValue("$id", dto.Id);
        command.Parameters.AddWithValue("$atletaId", (object?)dto.AtletaId ?? DBNull.Value);
        command.Parameters.AddWithValue("$equipeId", (object?)dto.EquipeId ?? DBNull.Value);
        command.Parameters.AddWithValue("$provaId", (object?)dto.ProvaId ?? DBNull.Value);
        command.Parameters.AddWithValue("$competicaoId", (object?)dto.CompeticaoId ?? DBNull.Value);
        command.Parameters.AddWithValue("$tempo", (object?)dto.Tempo ?? DBNull.Value);
        command.Parameters.AddWithValue("$colocacao", (object?)dto.Colocacao ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    // ##### MÉTODO CORRIGIDO #####
    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            // 1. Deleta a pontuação associada primeiro
            var cmdDeletePontuacao = connection.CreateCommand();
            cmdDeletePontuacao.Transaction = transaction;
            cmdDeletePontuacao.CommandText = "DELETE FROM Pontuacao WHERE participacao_prova_id = $id";
            cmdDeletePontuacao.Parameters.AddWithValue("$id", id);
            cmdDeletePontuacao.ExecuteNonQuery();

            // 2. Agora deleta a participação
            var cmdDeleteParticipacao = connection.CreateCommand();
            cmdDeleteParticipacao.Transaction = transaction;
            cmdDeleteParticipacao.CommandText = "DELETE FROM Participacao_Prova WHERE id = $id";
            cmdDeleteParticipacao.Parameters.AddWithValue("$id", id);
            cmdDeleteParticipacao.ExecuteNonQuery();

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw; // Re-lança a exceção para que o controller saiba do erro
        }
    }
}