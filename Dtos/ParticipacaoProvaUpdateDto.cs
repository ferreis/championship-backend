// Dtos/ParticipacaoProvaUpdateDto.cs
using System;
using System.ComponentModel.DataAnnotations;

public class ParticipacaoProvaUpdateDto
{
    [Required(ErrorMessage = "O ID da participação é obrigatório para atualização.")]
    public int Id { get; set; }

    public int? AtletaId { get; set; }
    public int? EquipeId { get; set; }
    public int? ProvaId { get; set; }
    public int? CompeticaoId { get; set; }
    public decimal? Tempo { get; set; }
    public int? Colocacao { get; set; }
}