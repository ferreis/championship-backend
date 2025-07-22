using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class EstadoService
{
    private readonly string _connectionString;
    public EstadoService(string connectionString) => _connectionString = connectionString;

    // Adicione estes métodos à classe EstadoService
    public void Atualizar(int id, EstadoUpdateDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"UPDATE Estado SET nome = COALESCE($nome, nome), sigla = COALESCE($sigla, sigla), pais_id = COALESCE($pais_id, pais_id) WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$nome", (object?)dto.Nome ?? DBNull.Value);
        command.Parameters.AddWithValue("$sigla", (object?)dto.Sigla ?? DBNull.Value);
        command.Parameters.AddWithValue("$pais_id", (object?)dto.PaisId ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        // Adicionar lógica para deletar cidades dependentes primeiro
        command.CommandText = "DELETE FROM Cidade WHERE estado_id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();

        command.CommandText = "DELETE FROM Estado WHERE id = $id";
        command.ExecuteNonQuery();
    }
    public List<Estado> Listar()
    {
        var lista = new List<Estado>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, sigla, pais_id FROM Estado";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Estado
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Sigla = reader.GetString(2),
                PaisId = reader.GetInt32(3)
            });
        }

        return lista;
    }
}