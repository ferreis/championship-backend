// Models/Pontuacao.cs
public class Pontuacao
{
    public int Id { get; set; }
    public int ParticipacaoProvaId { get; set; } // Referencia a Prova (ID da Prova)
    public int Pontos { get; set; } // Ou outro tipo, dependendo do que 'pontos' representa
}