using System;
using System.ComponentModel.DataAnnotations;

public class CompeticaoDto
{
    [Required(ErrorMessage = "O nome da competição é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "A data de início é obrigatória.")]
    [DataType(DataType.Date)]
    public DateTime DataInicio { get; set; }

    [Required(ErrorMessage = "A data de fim é obrigatória.")]
    [DataType(DataType.Date)]
    public DateTime DataFim { get; set; }

    [Required(ErrorMessage = "O ano da competição é obrigatório.")]
    [Range(1900, 2100, ErrorMessage = "O ano deve ser entre 1900 e 2100.")]
    public int Ano { get; set; }

    [Required(ErrorMessage = "O ID do país é obrigatório.")]
    public int PaisId { get; set; }

    [Required(ErrorMessage = "O ID do estado é obrigatório.")]
    public int EstadoId { get; set; }

    [Required(ErrorMessage = "O ID da cidade é obrigatório.")]
    public int CidadeId { get; set; }
}