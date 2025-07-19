// Services/AtletaService.cs
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class AtletaService
{
    private readonly string _connectionString;

    public AtletaService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Atleta? ObterPorId(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            "SELECT id, nome, cpf, genero, data_nascimento, pais_id FROM Atleta WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Atleta
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Cpf = reader.GetString(reader.GetOrdinal("cpf")),
                Genero = reader.GetString(reader.GetOrdinal("genero")),
                DataNascimento = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_nascimento"))),
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")),
            };
        }

        return null;
    }

    public List<Atleta> Listar()
    {
        var atletas = new List<Atleta>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            "SELECT id, nome, cpf, genero, data_nascimento, pais_id FROM Atleta";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            atletas.Add(new Atleta
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Cpf = reader.GetString(reader.GetOrdinal("cpf")),
                Genero = reader.GetString(reader.GetOrdinal("genero")),
                DataNascimento = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_nascimento"))),
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")),
            });
        }

        return atletas;
    }

    public void Adicionar(Atleta atleta)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Atleta (nome, cpf, genero, data_nascimento, pais_id)
            VALUES ($nome, $cpf, $genero, $data_nascimento, $pais_id)";
        command.Parameters.AddWithValue("$nome", atleta.Nome);
        command.Parameters.AddWithValue("$cpf", atleta.Cpf);
        command.Parameters.AddWithValue("$genero", atleta.Genero);
        command.Parameters.AddWithValue("$data_nascimento", atleta.DataNascimento.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$pais_id", atleta.PaisId);
        command.ExecuteNonQuery();
    }

    public void Atualizar(Atleta atleta)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Atleta
            SET nome = $nome, cpf = $cpf, genero = $genero, data_nascimento = $data_nascimento, pais_id = $pais_id
            WHERE id = $id";
        command.Parameters.AddWithValue("$id", atleta.Id);
        command.Parameters.AddWithValue("$nome", atleta.Nome);
        command.Parameters.AddWithValue("$cpf", atleta.Cpf);
        command.Parameters.AddWithValue("$genero", atleta.Genero);
        command.Parameters.AddWithValue("$data_nascimento", atleta.DataNascimento.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$pais_id", atleta.PaisId);
        command.ExecuteNonQuery();
    }

    public void VincularAtletaEquipe(AtletaEquipeDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Atleta_Equipe (atleta_id, equipe_id, competicao_id)
            VALUES ($atletaId, $equipeId, $competicaoId)";
        command.Parameters.AddWithValue("$atletaId", dto.AtletaId);
        command.Parameters.AddWithValue("$equipeId", dto.EquipeId);
        command.Parameters.AddWithValue("$competicaoId", dto.CompeticaoId);
        command.ExecuteNonQuery();
    }

    public List<AtletaComEquipeDto> ListarComEquipes(int? competicaoId)
    {
        var lista = new List<AtletaComEquipeDto>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();

        // CORREÇÃO: A consulta agora filtra por competicao_id e seleciona a coluna correta
        var sql = @"
            SELECT
                a.id AS atleta_id, a.nome, a.cpf, a.genero, a.data_nascimento, a.pais_id,
                p.nacionalidade AS pais_nacionalidade,
                e.id as equipe_id, e.nome AS equipe_nome, e.tipo AS equipe_tipo,
                s.nome AS equipe_estado_nome, pe.nacionalidade AS equipe_nacionalidade_nome,
                ae.competicao_id
            FROM Atleta a
            INNER JOIN Pais p ON a.pais_id = p.id
            LEFT JOIN Atleta_Equipe ae ON a.id = ae.atleta_id
            LEFT JOIN Equipe e ON ae.equipe_id = e.id
            LEFT JOIN Estado s ON e.estado_id = s.id
            LEFT JOIN Pais pe ON e.pais_id = pe.id";

        if (competicaoId.HasValue)
        {
            sql += " WHERE ae.competicao_id = $competicaoId OR ae.competicao_id IS NULL";
            command.Parameters.AddWithValue("$competicaoId", competicaoId.Value);
        }

        command.CommandText = sql;

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new AtletaComEquipeDto
            {
                AtletaId = reader.GetInt32(reader.GetOrdinal("atleta_id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Cpf = reader.GetString(reader.GetOrdinal("cpf")),
                Genero = reader.GetString(reader.GetOrdinal("genero")),
                DataNascimento = DateTime.Parse(reader.GetString(reader.GetOrdinal("data_nascimento"))),
                PaisId = reader.GetInt32(reader.GetOrdinal("pais_id")),
                Nacionalidade = reader.GetString(reader.GetOrdinal("pais_nacionalidade")),
                EquipeId = reader.IsDBNull(reader.GetOrdinal("equipe_id")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("equipe_id")),
                EquipeNome = reader.IsDBNull(reader.GetOrdinal("equipe_nome")) ? null : reader.GetString(reader.GetOrdinal("equipe_nome")),
                EquipeTipo = reader.IsDBNull(reader.GetOrdinal("equipe_tipo")) ? null : reader.GetString(reader.GetOrdinal("equipe_tipo")),
                EquipeEstado = reader.IsDBNull(reader.GetOrdinal("equipe_estado_nome")) ? null : reader.GetString(reader.GetOrdinal("equipe_estado_nome")),
                EquipeNacionalidade = reader.IsDBNull(reader.GetOrdinal("equipe_nacionalidade_nome")) ? null : reader.GetString(reader.GetOrdinal("equipe_nacionalidade_nome")),
                // CORREÇÃO: Mapeando a coluna correta do banco para a propriedade do DTO
                // Assumindo que a propriedade no DTO se chama 'CompeticaoId'
                CompeticaoId = reader.IsDBNull(reader.GetOrdinal("competicao_id")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("competicao_id"))
            });
        }

        return lista;
    }

    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;

            var participacaoIds = new List<long>();
            cmd.CommandText = "SELECT id FROM Participacao_Prova WHERE atleta_id = $atleta_id";
            cmd.Parameters.AddWithValue("$atleta_id", id);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    participacaoIds.Add(reader.GetInt64(0));
                }
            }

            if (participacaoIds.Count > 0)
            {
                var idsParaDeletar = participacaoIds.ToList();
                foreach (var participacaoId in idsParaDeletar)
                {
                    var cmdDeletePontuacao = connection.CreateCommand();
                    cmdDeletePontuacao.Transaction = transaction;
                    cmdDeletePontuacao.CommandText = "DELETE FROM Pontuacao WHERE participacao_prova_id = $p_id";
                    cmdDeletePontuacao.Parameters.AddWithValue("$p_id", participacaoId);
                    cmdDeletePontuacao.ExecuteNonQuery();
                }
            }

            cmd.CommandText = "DELETE FROM Participacao_Prova WHERE atleta_id = $atleta_id";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM Atleta_Equipe WHERE atleta_id = $atleta_id";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM Atleta WHERE id = $atleta_id";
            cmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
}