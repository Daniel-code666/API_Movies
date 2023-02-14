namespace API_Movies.Models.Dtos
{
    public class UserLoginResponseDto
    {
        public UserDataDto User { get; set; }

        public string Token { get; set; }
    }
}
