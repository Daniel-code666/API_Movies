using System.ComponentModel.DataAnnotations;

namespace API_Movies.Models.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string nickName { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string password { get; set; }
    }
}
