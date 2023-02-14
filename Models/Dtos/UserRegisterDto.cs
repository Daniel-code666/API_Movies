using System.ComponentModel.DataAnnotations;

namespace API_Movies.Models.Dtos
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string nickName { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string userName { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string password { get; set; }

        public string role { get; set; }
    }
}
