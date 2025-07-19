using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class CidadeService
{
    private readonly string _connectionString;
    public CidadeService(string connectionString) => _connectionString = connectionString;

    public void Adicionar(Cidade cidade)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Cidade (nome, estado_id) VALUES ($nome, $estado_id)";
        command.Parameters.AddWithValue("$nome", cidade.Nome);
        command.Parameters.AddWithValue("$estado_id", cidade.EstadoId);
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