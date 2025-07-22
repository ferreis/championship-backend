using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class CidadeService
{
    private readonly string _connectionString;
    public CidadeService(string connectionString) => _connectionString = connectionString;

    // Adicione estes métodos à classe CidadeService
    public void Atualizar(int id, CidadeUpdateDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"UPDATE Cidade SET nome = COALESCE($nome, nome), estado_id = COALESCE($estado_id, estado_id) WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$nome", (object?)dto.Nome ?? DBNull.Value);
        command.Parameters.AddWithValue("$estado_id", (object?)dto.EstadoId ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Cidade WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public List<Cidade> Listar()
    {
        var lista = new List<Cidade>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, estado_id FROM Cidade";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Cidade
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                EstadoId = reader.GetInt32(2)
            });
        }

        return lista;
    }
}