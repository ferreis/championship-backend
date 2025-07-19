public class AtletaUpdateDto
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Cpf { get; set; }
    public string? Genero { get; set; }
    public DateTime? DataNascimento { get; set; }
    public int? PaisId { get; set; }
}