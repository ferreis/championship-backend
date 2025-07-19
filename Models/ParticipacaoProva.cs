using System;

public class ParticipacaoProva
{
    public int Id { get; set; }
    public int AtletaId { get; set; }
    public int? EquipeId { get; set; }       // Pode ser nulo se for prova individual
    public int ProvaId { get; set; }
    public int CompeticaoId { get; set; }   // Associando a uma Competição específica
    public decimal? Tempo { get; set; }     // NUMERIC no DB, pode ser nulo se a prova for por colocação
    public int? Colocacao { get; set; }     // INTEGER no DB, pode ser nulo se a prova for por tempo
}