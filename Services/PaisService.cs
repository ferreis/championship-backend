using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class PaisService
{
    private readonly string _connectionString;

    public PaisService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Pais> Listar()
    {
        var lista = new List<Pais>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, nome, nacionalidade FROM Pais";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Pais
            {
                Id = reader.GetInt32(0),
                Nome = reader.GetString(1),
                Nacionalidade = reader.GetString(2)
            });
        }

        return lista;
    }

    public void Adicionar(PaisDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Pais (nome, nacionalidade) VALUES ($nome, $nacionalidade)";
        command.Parameters.AddWithValue("$nome", dto.Nome);
        command.Parameters.AddWithValue("$nacionalidade", dto.Nacionalidade);
        command.ExecuteNonQuery();
    }

    public void Atualizar(int id, PaisUpdateDto dto)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE Pais SET 
                nome = COALESCE($nome, nome),
                nacionalidade = COALESCE($nacionalidade, nacionalidade)
            WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$nome", (object?)dto.Nome ?? DBNull.Value);
        command.Parameters.AddWithValue("$nacionalidade", (object?)dto.Nacionalidade ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public void Deletar(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Pais WHERE id = $id";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }
}
