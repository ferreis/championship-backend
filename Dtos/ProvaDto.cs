// Dtos/ProvaDto.cs
using System.ComponentModel.DataAnnotations;

public class ProvaDto
{
    [Required(ErrorMessage = "O nome da prova é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O tipo da prova é obrigatório.")]
    // Use um validador personalizado ou enum para as CHECK CONSTRAINTS
    public string Tipo { get; set; } 

    [Required(ErrorMessage = "A modalidade da prova é obrigatória.")]
    public string Modalidade { get; set; }

    [Required(ErrorMessage = "A forma de avaliação (tempo ou colocação) é obrigatória.")]
    public string TempoOuColocacao { get; set; }

    [Required(ErrorMessage = "O gênero da prova é obrigatório.")]
    public string Genero { get; set; }

    [Required(ErrorMessage = "A categoria etária da prova é obrigatória.")]
    public string CategoriaEtaria { get; set; }
}