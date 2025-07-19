using System.Collections.Generic;
using Microsoft.Data.Sqlite;

public class EstadoService
{
    private readonly string _connectionString;
    public EstadoService(string connectionString) => _connectionString = connectionString;

    public void Adicionar(Estado estado)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Estado (nome, sigla, pais_id) VALUES ($nome, $sigla, $pais_id)";
        command.Parameters.AddWithValue("$nome", estado.Nome);
        command.Parameters.AddWithValue("$sigla", estado.Sigla);
        command.Parameters.AddWithValue("$pais_id", estado.PaisId);
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