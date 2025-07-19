using System;

public class Competicao
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int Ano { get; set; }
    public int PaisId { get; set; }
    public int EstadoId { get; set; }
    public int CidadeId { get; set; }
}