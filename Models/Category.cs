using System.ComponentModel.DataAnnotations;

namespace API_Movies.Models
{
    public class Category
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string? name { get; set; }

        [Required]
        public DateTime created_at { get; set; }
    }
}
