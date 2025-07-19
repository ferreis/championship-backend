using System;
using System.Collections.Generic;

public class CompeticaoDetalhesDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int Ano { get; set; }
    public List<ProvaComInscritosDto> Provas { get; set; } = new List<ProvaComInscritosDto>();
}

public class ProvaComInscritosDto
{
    public int ProvaId { get; set; }
    public string NomeProva { get; set; } = string.Empty;
    public string Modalidade { get; set; } = string.Empty;
    public List<AtletaInscritoDto> AtletasInscritos { get; set; } = new List<AtletaInscritoDto>();
}

public class AtletaInscritoDto
{
    public int AtletaId { get; set; }
    public string NomeAtleta { get; set; } = string.Empty;
    public int? ParticipacaoId { get; set; }
    public string? EquipeNome { get; set; }
}