using API_Movies.Models;
using API_Movies.Models.Dtos;

namespace API_Movies.Repository.IRepository
{
    public interface IUserRepository
    {
        ICollection<AppUser> GetUsers();

        AppUser GetSingleUser(string userId);

        bool IsUniqueUser(string userName);

        bool UserExists(string userId);

        Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);

        // Task<User> Register(UserRegisterDto userRegisterDto);

        Task<UserDataDto> Register(UserRegisterDto userRegisterDto);

        Task<UserLoginResponseDto> ValidateToken(string token);

    }
}
