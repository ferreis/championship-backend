public class PontuacaoUpdateDto
{
    public int Id { get; set; }
    public int? ParticipacaoProvaId { get; set; } // Pode ser Nullable se for opcional
    public int? Pontos { get; set; } // Pode ser Nullable
}