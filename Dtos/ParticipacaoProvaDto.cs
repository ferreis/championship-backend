// Dtos/ParticipacaoProvaDto.cs
using System;
using System.ComponentModel.DataAnnotations;

public class ParticipacaoProvaDto
{
    [Required(ErrorMessage = "O ID do atleta é obrigatório.")]
    public int AtletaId { get; set; }

    public int? EquipeId { get; set; } // Opcional, dependendo da modalidade da prova

    [Required(ErrorMessage = "O ID da prova é obrigatório.")]
    public int ProvaId { get; set; }

    [Required(ErrorMessage = "O ID da competição é obrigatório.")]
    public int CompeticaoId { get; set; }

    public decimal? Tempo { get; set; }
    public int? Colocacao { get; set; }

    // Validação customizada para garantir que ou Tempo ou Colocacao sejam preenchidos
    // ou ambos, se a prova permitir. Mas se a prova_id indicar "tempo_ou_colocacao",
    // a validação deve ser mais complexa e feita no serviço ou controller.
    // Por enquanto, apenas os campos são nullables
}