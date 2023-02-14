using System.ComponentModel.DataAnnotations;

namespace API_Movies.Models.Dtos
{
    public class CategoryDto
    {
        public int id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(60, ErrorMessage = "El número máximo de caracteres es de 100")]
        public string? name { get; set; }

        public DateTime created_at { get; set; }
    }
}

