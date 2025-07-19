public class Atleta
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public int PaisId { get; set; }
    public string? Nacionalidade { get; set; }
}