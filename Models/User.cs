using System.ComponentModel.DataAnnotations;

namespace API_Movies.Models
{
    public class User
    {
        [Key]
        public int userId { get; set; }

        public string nickName { get; set; }

        public string userName { get; set; }

        public string password { get; set; }

        public string role { get; set; }
    }
}
