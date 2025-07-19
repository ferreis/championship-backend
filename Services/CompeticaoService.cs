// Services/CompeticaoService.cs
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Data; // Para IDataReader

public class CompeticaoService
{
    private readonly string _connectionString;

    public CompeticaoService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Competicao ObterPorId(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, data_inicio, data_fim, ano, pais_id, estado_id, cidade_id FROM Competicao WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Competicao
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                DataInicio = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_inicio"))),
                DataFim = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_fim"))),
                Ano = reader.GetInt32(reader.GetOrdinal("ano")),
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")),
                EstadoId = reader.GetInt32(reader.GetOrdinal("estado_id")),
                CidadeId = reader.GetInt32(reader.GetOrdinal("cidade_id"))
            };
        }
        return null;
    }

    public List<Competicao> Listar()
    {
        var competicoes = new List<Competicao>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, data_inicio, data_fim, ano, pais_id, estado_id, cidade_id FROM Competicao";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            competicoes.Add(new Competicao
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                DataInicio = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_inicio"))),
                DataFim = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_fim"))),
                Ano = reader.GetInt32(reader.GetOrdinal("ano")),
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")),
                EstadoId = reader.GetInt32(reader.GetOrdinal("estado_id")),
                CidadeId = reader.GetInt32(reader.GetOrdinal("cidade_id"))
            });
        }
        return competicoes;
    }

    public int Adicionar(CompeticaoDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Competicao (nome, data_inicio, data_fim, ano, pais_id, estado_id, cidade_id)
            VALUES ($nome, $dataInicio, $dataFim, $ano, $paisId, $estadoId, $cidadeId)";
        command.Parameters.AddWithValue("$nome", dto.Nome);
        command.Parameters.AddWithValue("$dataInicio", dto.DataInicio.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$dataFim", dto.DataFim.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$ano", dto.Ano);
        command.Parameters.AddWithValue("$paisId", dto.PaisId);
        command.Parameters.AddWithValue("$estadoId", dto.EstadoId);
        command.Parameters.AddWithValue("$cidadeId", dto.CidadeId);
        command.ExecuteNonQuery();

        // Obter o ID da última linha inserida usando SQL
        command.CommandText = "SELECT last_insert_rowid()";
        var newId = (long)command.ExecuteScalar();
        return (int)newId;
    }

    public void Atualizar(CompeticaoUpdateDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Competicao
            SET nome = COALESCE($nome, nome),
                data_inicio = COALESCE($dataInicio, data_inicio),
                data_fim = COALESCE($dataFim, data_fim),
                ano = COALESCE($ano, ano),
                pais_id = COALESCE($paisId, pais_id),
                estado_id = COALESCE($estadoId, estado_id),
                cidade_id = COALESCE($cidadeId, cidade_id)
            WHERE id = $id";
        command.Parameters.AddWithValue("$id", dto.Id);
        command.Parameters.AddWithValue("$nome", (object?)dto.Nome ?? DBNull.Value);
        command.Parameters.AddWithValue("$dataInicio", (object?)dto.DataInicio?.ToString("yyyy-MM-dd") ?? DBNull.Value);
        command.Parameters.AddWithValue("$dataFim", (object?)dto.DataFim?.ToString("yyyy-MM-dd") ?? DBNull.Value);
        command.Parameters.AddWithValue("$ano", (object?)dto.Ano ?? DBNull.Value);
        command.Parameters.AddWithValue("$paisId", (object?)dto.PaisId ?? DBNull.Value);
        command.Parameters.AddWithValue("$estadoId", (object?)dto.EstadoId ?? DBNull.Value);
        command.Parameters.AddWithValue("$cidadeId", (object?)dto.CidadeId ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Competicao WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
    // Em Services/CompeticaoService.cs

    public CompeticaoDetalhesDto? ObterDetalhesDaCompeticao(int id)
    {
        var competicaoDetalhes = new CompeticaoDetalhesDto();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var cmdComp = connection.CreateCommand();
        cmdComp.CommandText = "SELECT id, nome, data_inicio, data_fim, ano FROM Competicao WHERE id = $id";
        cmdComp.Parameters.AddWithValue("$id", id);

        using (var readerComp = cmdComp.ExecuteReader())
        {
            if (!readerComp.Read()) return null;
            competicaoDetalhes.Id = readerComp.GetInt32(0);
            competicaoDetalhes.Nome = readerComp.GetString(1);
            competicaoDetalhes.DataInicio = DateTime.Parse(readerComp.GetString(2));
            competicaoDetalhes.DataFim = DateTime.Parse(readerComp.GetString(3));
            competicaoDetalhes.Ano = readerComp.GetInt32(4);
        }

        // CORREÇÃO FINAL: Simplificamos a consulta para pegar a equipe direto da participação
        var cmdProvas = connection.CreateCommand();
        cmdProvas.CommandText = @"
        SELECT 
            p.id as ProvaId,
            p.nome as NomeProva,
            p.modalidade as Modalidade,
            a.id as AtletaId,
            a.nome as NomeAtleta,
            pp.id as ParticipacaoId,
            e.nome as NomeEquipe
        FROM Prova p
        LEFT JOIN Participacao_Prova pp ON p.id = pp.prova_id AND pp.competicao_id = $competicao_id
        LEFT JOIN Atleta a ON pp.atleta_id = a.id
        LEFT JOIN Equipe e ON pp.equipe_id = e.id -- <-- A FONTE CORRETA DA EQUIPE DA PROVA
        ORDER BY p.nome, a.nome";
        cmdProvas.Parameters.AddWithValue("$competicao_id", id);

        using (var readerProvas = cmdProvas.ExecuteReader())
        {
            ProvaComInscritosDto? provaAtual = null;
            while (readerProvas.Read())
            {
                var provaId = readerProvas.GetInt32(readerProvas.GetOrdinal("ProvaId"));

                if (provaAtual == null || provaAtual.ProvaId != provaId)
                {
                    provaAtual = new ProvaComInscritosDto
                    {
                        ProvaId = provaId,
                        NomeProva = readerProvas.GetString(readerProvas.GetOrdinal("NomeProva")),
                        Modalidade = readerProvas.GetString(readerProvas.GetOrdinal("Modalidade"))
                    };
                    competicaoDetalhes.Provas.Add(provaAtual);
                }

                if (!readerProvas.IsDBNull(readerProvas.GetOrdinal("AtletaId")))
                {
                    provaAtual.AtletasInscritos.Add(new AtletaInscritoDto
                    {
                        AtletaId = readerProvas.GetInt32(readerProvas.GetOrdinal("AtletaId")),
                        NomeAtleta = readerProvas.GetString(readerProvas.GetOrdinal("NomeAtleta")),
                        ParticipacaoId = readerProvas.GetInt32(readerProvas.GetOrdinal("ParticipacaoId")),
                        EquipeNome = readerProvas.IsDBNull(readerProvas.GetOrdinal("NomeEquipe")) ? "Individual" : readerProvas.GetString(readerProvas.GetOrdinal("NomeEquipe"))
                    });
                }
            }
        }

        return competicaoDetalhes;
    }
}