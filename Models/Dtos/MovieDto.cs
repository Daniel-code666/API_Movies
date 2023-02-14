using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Movies.Models.Dtos
{
    public class MovieDto
    {
        public string idMovie { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        public string title { get; set; }

        public string? imgRoute { get; set; }

        public IFormFile? movieImg { get; set; } 

        [Required(ErrorMessage = "La descripción es requerida")]
        public string description { get; set; }

        [Required(ErrorMessage = "La duración es requerida")]
        public int duration { get; set; }

        public enum movieRating { siete, trece, dieciseis, dieciocho }

        public movieRating rating { get; set; }

        public DateTime? movie_created_at { get; set; }

        public int categoryId { get; set; }
    }
}
