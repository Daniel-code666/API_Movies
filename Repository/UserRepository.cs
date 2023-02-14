using API_Movies.Data;
using API_Movies.Models;
using API_Movies.Models.Dtos;
using API_Movies.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XAct.Users;
using XSystem.Security.Cryptography;

namespace API_Movies.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private string secretKey;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDbContext db, IConfiguration config, RoleManager<IdentityRole> roleManager, 
            UserManager<AppUser> userManager, IMapper mapper)
        {
            _db = db;
            secretKey = config.GetValue<string>("ApiSettings:Secret");
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public AppUser GetSingleUser(string userId)
        {
            return _db.AppUser.FirstOrDefault(u => u.Id == userId);
        }

        public ICollection<AppUser> GetUsers()
        {
            return _db.AppUser.ToList();
        }

        public bool IsUniqueUser(string userName)
        {
            return _db.AppUser.Any(u => u.UserName == userName);
        }

        public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
        {
            // var encrPass = obtainMd5(userLoginDto.password);

            //var user = _db.User.FirstOrDefault(
            //    u => u.nickName.ToLower() == userLoginDto.nickName.ToLower()
            //    && u.password == encrPass
            //);
            var user = _db.AppUser.FirstOrDefault(u => u.UserName.ToLower() == userLoginDto.nickName.ToLower());

            bool isUserValid = await _userManager.CheckPasswordAsync(user, userLoginDto.password);

            if (user == null || !isUserValid)
            {
                return new UserLoginResponseDto()
                {
                    Token = "",
                    User = null
                };
            }

            var roles = await _userManager.GetRolesAsync(user);

            var tknHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)                
            };

            var token = tknHandler.CreateToken(tokenDescriptor);

            UserLoginResponseDto userLoginResponseDto = new UserLoginResponseDto()
            {
                Token = tknHandler.WriteToken(token),
                User = _mapper.Map<UserDataDto>(user)
            };

            return userLoginResponseDto;
        }

        public async Task<UserDataDto> Register(UserRegisterDto userRegisterDto)
        {
            // var encrPass = obtainMd5(userRegisterDto.password);

            AppUser user = new AppUser
            {
                Email = userRegisterDto.nickName,
                UserName = userRegisterDto.nickName,
                NormalizedEmail = userRegisterDto.nickName.ToUpper(),
                name = userRegisterDto.userName
            };

            //_db.User.Add(user);
            //await _db.SaveChangesAsync();
            //user.password = encrPass;
            //return user;

            var result = await _userManager.CreateAsync(user, userRegisterDto.password);

            if (result.Succeeded)
            {
                // crea los roles
                if(!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    await _roleManager.CreateAsync(new IdentityRole("user"));
                }

                await _userManager.AddToRoleAsync(user, "user");
                var userRetorned = _db.AppUser.FirstOrDefault(u => u.UserName == userRegisterDto.nickName);

                return _mapper.Map<UserDataDto>(userRetorned);
            }

            return new UserDataDto();
        }

        //private static string obtainMd5(string password)
        //{
        //    MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
        //    byte[] data = System.Text.Encoding.UTF8.GetBytes(password);
        //    data = x.ComputeHash(data);
        //    string resp = "";
        //    for (int i = 0; i < data.Length; i++)
        //        resp += data[i].ToString("x2").ToLower();

        //    return resp;
        //}

        public bool UserExists(string userId)
        {
            return _db.AppUser.Any(u => u.Id == userId);
        }

        public async Task<UserLoginResponseDto> ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var SecretKey = secretKey;
                var key = Encoding.ASCII.GetBytes(SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken) validatedToken;
                var userName = jwtToken.Claims.First(x => x.Type == "unique_name").Value;
                var userRol = jwtToken.Claims.First(x => x.Type == "role").Value;

                // var userData = _db.AppUser.FirstOrDefault(u => u.UserName == userName);

                UserLoginResponseDto userLoginResponseDto = new UserLoginResponseDto()
                {
                    Token = token,
                    User = new UserDataDto
                    {
                        Id = "ok",
                        UserName = userName,
                        name = userRol
                    }
                };

                return userLoginResponseDto;
            }
            catch (Exception ex)
            {
                UserLoginResponseDto userLoginResponseDto = new UserLoginResponseDto()
                {
                    Token = token,
                    User = new UserDataDto()
                    {
                        Id = ex.Message,
                        name = "error"
                    }
                };

                return userLoginResponseDto;
            }
        }
    }
}
