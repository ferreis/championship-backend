// DataSeeder.cs
using Microsoft.Data.Sqlite;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DataSeeder
{
    public static void Seed(string connectionString)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var cmdCheck = connection.CreateCommand();
        cmdCheck.CommandText = "SELECT COUNT(*) FROM Atleta";
        if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0) return;

        Console.WriteLine("Populando o banco de dados com um conjunto completo de dados de teste...");
        var faker = new Faker("pt_BR");

        // --- Dados Fundamentais ---
        long brasilId;
        using (var c = connection.CreateCommand()) { c.CommandText = "INSERT INTO Pais (nome, nacionalidade) VALUES ('Brasil', 'Brasileiro'); SELECT last_insert_rowid();"; brasilId = (long)c.ExecuteScalar(); }
        long scId;
        using (var c = connection.CreateCommand()) { c.CommandText = "INSERT INTO Estado (nome, sigla, pais_id) VALUES ('Santa Catarina', 'SC', $p); SELECT last_insert_rowid();"; c.Parameters.AddWithValue("$p", brasilId); scId = (long)c.ExecuteScalar(); }
        long itajaiId;
        using (var c = connection.CreateCommand()) { c.CommandText = "INSERT INTO Cidade (nome, estado_id) VALUES ('Itajaí', $e); SELECT last_insert_rowid();"; c.Parameters.AddWithValue("$e", scId); itajaiId = (long)c.ExecuteScalar(); }

        // --- Provas ---
        var provas = new List<(string Nome, string Tipo, string Modalidade, string Avaliacao, string Genero, string Categoria)>
        {
            ("Rescue Tube Race", "Praia", "Individual", "Tempo", "unissex", "Open"),
            ("200m Obstáculos", "Piscina", "Individual", "Tempo", "M", "Senior"),
            ("Line Throw", "Praia", "Dupla", "Tempo", "unissex", "Todas"),
            ("Beach Flag", "Praia", "Individual", "Colocacao", "unissex", "Open"),
            ("50m Carregando o Manequim", "Piscina", "Individual", "Tempo", "F", "A 18 a 24 anos"),
            ("Revezamento 4x50m Medley", "Piscina", "Revezamento", "Tempo", "M", "Open"),
            ("Corrida a Nadadeira", "Praia", "Individual", "Colocacao", "F", "Open")
        };

        var provaIds = new List<long>();
        foreach (var p in provas)
        {
            using (var c = connection.CreateCommand())
            {
                c.CommandText = "INSERT INTO Prova (nome, tipo, modalidade, tempo_ou_colocacao, genero, categoria_etaria) VALUES ($n, $t, $m, $a, $g, $c); SELECT last_insert_rowid();";
                c.Parameters.AddWithValue("$n", p.Nome); c.Parameters.AddWithValue("$t", p.Tipo); c.Parameters.AddWithValue("$m", p.Modalidade);
                c.Parameters.AddWithValue("$a", p.Avaliacao); c.Parameters.AddWithValue("$g", p.Genero); c.Parameters.AddWithValue("$c", p.Categoria);
                provaIds.Add((long)c.ExecuteScalar());
            }
        }

        // --- Competição ---
        long competicaoId;
        using (var c = connection.CreateCommand())
        {
            c.CommandText = "INSERT INTO Competicao (nome, data_inicio, data_fim, ano, pais_id, estado_id, cidade_id) VALUES ('Campeonato Sobrasa de Teste 2024', '2024-10-20', '2024-10-22', 2024, $p, $e, $c); SELECT last_insert_rowid();";
            c.Parameters.AddWithValue("$p", brasilId); c.Parameters.AddWithValue("$e", scId); c.Parameters.AddWithValue("$c", itajaiId);
            competicaoId = (long)c.ExecuteScalar();
        }

        // --- Equipes e Atletas ---
        var atletaEquipeMap = new Dictionary<long, long>(); // Mapa para saber a equipe de cada atleta
        for (int i = 0; i < 10; i++)
        {
            long equipeId;
            using (var c = connection.CreateCommand())
            {
                c.CommandText = "INSERT INTO Equipe (nome, tipo, estado_id, pais_id) VALUES ($n, 'Clube', $e, $p); SELECT last_insert_rowid();";
                c.Parameters.AddWithValue("$n", $"Equipe {faker.Company.CompanyName()}"); c.Parameters.AddWithValue("$e", scId); c.Parameters.AddWithValue("$p", brasilId);
                equipeId = (long)c.ExecuteScalar();
            }

            for (int j = 0; j < 5; j++)
            {
                long atletaId;
                using (var c = connection.CreateCommand())
                {
                    var g = faker.PickRandom<Bogus.DataSets.Name.Gender>();
                    c.CommandText = "INSERT INTO Atleta (nome, cpf, genero, data_nascimento, pais_id) VALUES ($n, $cpf, $g, $dn, $p); SELECT last_insert_rowid();";
                    c.Parameters.AddWithValue("$n", faker.Name.FullName(g)); c.Parameters.AddWithValue("$cpf", faker.Random.ReplaceNumbers("###########" + i + j));
                    c.Parameters.AddWithValue("$g", g == Bogus.DataSets.Name.Gender.Male ? "M" : "F");
                    c.Parameters.AddWithValue("$dn", faker.Date.Past(20, DateTime.Now.AddYears(-18)).ToString("yyyy-MM-dd"));
                    c.Parameters.AddWithValue("$p", brasilId);
                    atletaId = (long)c.ExecuteScalar();
                    atletaEquipeMap[atletaId] = equipeId; // Guarda a equipe do atleta
                }

                using (var c = connection.CreateCommand())
                {
                    c.CommandText = "INSERT INTO Atleta_Equipe (atleta_id, equipe_id, competicao_id) VALUES ($a, $e, $c);";
                    c.Parameters.AddWithValue("$a", atletaId); c.Parameters.AddWithValue("$e", equipeId); c.Parameters.AddWithValue("$c", competicaoId);
                    c.ExecuteNonQuery();
                }
            }
        }

        // --- Inscrições e Resultados ---
        var rnd = new Random();
        var pontosMap = new Dictionary<int, int> { { 1, 20 }, { 2, 18 }, { 3, 16 }, { 4, 14 }, { 5, 13 }, { 6, 12 }, { 7, 11 }, { 8, 10 }, { 9, 8 } };

        foreach (var provaId in provaIds)
        {
            var atletasInscritos = atletaEquipeMap.Keys.OrderBy(a => rnd.Next()).Take(faker.Random.Int(8, 15)).ToList();
            for (int colocacao = 1; colocacao <= atletasInscritos.Count; colocacao++)
            {
                var atletaId = atletasInscritos[colocacao - 1];
                var equipeId = atletaEquipeMap[atletaId]; // Pega a equipe do atleta
                long participacaoId;
                using (var c = connection.CreateCommand())
                {
                    c.CommandText = "INSERT INTO Participacao_Prova (atleta_id, equipe_id, prova_id, competicao_id, tempo, colocacao) VALUES ($a, $e, $p, $c, $t, $col); SELECT last_insert_rowid();";
                    c.Parameters.AddWithValue("$a", atletaId);
                    c.Parameters.AddWithValue("$e", equipeId); // <-- INSERINDO A EQUIPE
                    c.Parameters.AddWithValue("$p", provaId);
                    c.Parameters.AddWithValue("$c", competicaoId);
                    c.Parameters.AddWithValue("$t", Math.Round(faker.Random.Double(30, 180), 2));
                    c.Parameters.AddWithValue("$col", colocacao);
                    participacaoId = (long)c.ExecuteScalar();
                }

                if (pontosMap.ContainsKey(colocacao))
                {
                    using (var c = connection.CreateCommand())
                    {
                        c.CommandText = "INSERT INTO Pontuacao (participacao_prova_id, pontos) VALUES ($p_id, $pts);";
                        c.Parameters.AddWithValue("$p_id", participacaoId);
                        c.Parameters.AddWithValue("$pts", pontosMap[colocacao]);
                        c.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}