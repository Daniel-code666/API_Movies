using System.ComponentModel.DataAnnotations;

namespace API_Movies.Models.Dtos
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(60, ErrorMessage = "El número máximo de caracteres es de 100")]
        public string? name { get; set; }
    }
}
