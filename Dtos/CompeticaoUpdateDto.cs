// Dtos/CompeticaoUpdateDto.cs
using System;
using System.ComponentModel.DataAnnotations;

public class CompeticaoUpdateDto
{
    [Required(ErrorMessage = "O ID da competição é obrigatório para atualização.")]
    public int Id { get; set; }

    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
    public string? Nome { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DataInicio { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DataFim { get; set; }

    [Range(1900, 2100, ErrorMessage = "O ano deve ser entre 1900 e 2100.")]
    public int? Ano { get; set; }

    public int? PaisId { get; set; }
    public int? EstadoId { get; set; }
    public int? CidadeId { get; set; }
}