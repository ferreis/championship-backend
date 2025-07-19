// Dtos/ProvaUpdateDto.cs
using System.ComponentModel.DataAnnotations;

public class ProvaUpdateDto
{
    [Required(ErrorMessage = "O ID da prova é obrigatório para atualização.")]
    public int Id { get; set; }

    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
    public string? Nome { get; set; }

    public string? Tipo { get; set; } 
    public string? Modalidade { get; set; }
    public string? TempoOuColocacao { get; set; }
    public string? Genero { get; set; }
    public string? CategoriaEtaria { get; set; }
}