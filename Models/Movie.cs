using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Movies.Models
{
    public class Movie
    {
        [Key]
        public int idMovie { get; set; }
        public string title { get; set; }
        public string? imgRoute { get; set; }
        public string description { get; set; }

        public int duration { get; set; }

        public enum movieRating { siete, trece, dieciseis, dieciocho }
        
        public movieRating rating { get; set; }

        public DateTime movie_created_at { get; set; }

        [ForeignKey("categoryId")]
        public int categoryId { get; set; }

        public Category category { get; set; }

    }
}
