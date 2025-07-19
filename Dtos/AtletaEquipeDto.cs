public class AtletaEquipeDto
{
    public int? Id { get; set; }
    public int? AtletaId { get; set; }
    public int? EquipeId { get; set; }
    public int? CompeticaoId { get; set; }
}
public class AtletaComEquipeDto
{
    public int AtletaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public int PaisId { get; set; }
    public int? EquipeId { get; set; }
    public string? Nacionalidade { get; set; }
    public string? EquipeNome { get; set; }
    public string? EquipeTipo { get; set; }
    public string? EquipeEstado { get; set; }
    public string? EquipeNacionalidade { get; set; }
    public int? CompeticaoId { get; set; }
}