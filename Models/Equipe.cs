public class Equipe
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int? EstadoId { get; set; }
    public int? PaisId { get; set; }
}