using System; // Necessário para DBNull.Value
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class EquipeService
{
    private readonly string _connectionString;

    public EquipeService(string connectionString)
    {
        _connectionString = connectionString;
    }

    // NOVO MÉTODO: ObterPorId
    public Equipe ObterPorId(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, tipo, estado_id, pais_id FROM Equipe WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Equipe
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Tipo = reader.GetString(reader.GetOrdinal("tipo")),
                EstadoId = reader.IsDBNull(reader.GetOrdinal("estado_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("estado_id")),
                PaisId = reader.IsDBNull(reader.GetOrdinal("pais_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("pais_id"))
            };
        }
        return null; // Retorna null se a equipe não for encontrada
    }

    public void Adicionar(Equipe equipe)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Equipe (nome, tipo, estado_id, pais_id)
            VALUES ($nome, $tipo, $estado_id, $pais_id)";
        command.Parameters.AddWithValue("$nome", equipe.Nome);
        command.Parameters.AddWithValue("$tipo", equipe.Tipo);
        command.Parameters.AddWithValue("$estado_id", (object?)equipe.EstadoId ?? DBNull.Value);
        command.Parameters.AddWithValue("$pais_id", (object?)equipe.PaisId ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public List<Equipe> Listar()
    {
        var equipes = new List<Equipe>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, tipo, estado_id, pais_id FROM Equipe";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            equipes.Add(new Equipe
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Nome = reader.GetString(reader.GetOrdinal("nome")),
                Tipo = reader.GetString(reader.GetOrdinal("tipo")),
                EstadoId = reader.IsDBNull(reader.GetOrdinal("estado_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("estado_id")),
                PaisId = reader.IsDBNull(reader.GetOrdinal("pais_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("pais_id"))
            });
        }
        return equipes;
    }

    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Equipe WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public void Atualizar(int id, EquipeUpdateDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Não é necessário obter a equipe existente aqui se o COALESCE for usado corretamente no SQL,
        // mas o controlador já chama ObterPorId antes de chamar este método, então não tem problema.
        // Se você quiser que este método seja mais robusto e não dependa do controlador já ter obtido a equipe,
        // você pode descomentar a linha abaixo:
        // var existente = ObterPorId(id);
        // if (existente == null) { throw new InvalidOperationException($"Equipe com ID {id} não encontrada para atualização."); }

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Equipe
            SET nome = COALESCE($nome, nome),
                tipo = COALESCE($tipo, tipo),
                estado_id = COALESCE($estado_id, estado_id),
                pais_id = COALESCE($pais_id, pais_id)
            WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        // Usar (object?)prop ?? DBNull.Value é correto para passar valores nullable (strings ou ints) para o parâmetro SQL
        command.Parameters.AddWithValue("$nome", (object?)dto.Nome ?? DBNull.Value);
        command.Parameters.AddWithValue("$tipo", (object?)dto.Tipo ?? DBNull.Value);
        command.Parameters.AddWithValue("$estado_id", (object?)dto.EstadoId ?? DBNull.Value);
        command.Parameters.AddWithValue("$pais_id", (object?)dto.PaisId ?? DBNull.Value);
        command.ExecuteNonQuery();
    }
}