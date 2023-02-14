using API_Movies.Models;
using API_Movies.Models.Dtos;
using API_Movies.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using XAct.Security;

namespace API_Movies.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private ApiResponse _apiResponse;

        public UserController(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            this._apiResponse = new();
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult GetUsers()
        {
            var usersList = _userRepo.GetUsers();
            var userListDto = new List<UserDto>();

            foreach(var item in usersList)
            {
                userListDto.Add(_mapper.Map<UserDto>(item));
            }

            return Ok(userListDto);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{userId}", Name = "GetSingleUser")]
        public IActionResult GetSingleUser(string userId)
        {
            if (_userRepo.UserExists(userId))
            {
                var user = _userRepo.GetSingleUser(userId);
                var userDto = _mapper.Map<UserDto>(user);

                return Ok(userDto);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            bool userRegisterValidate = _userRepo.IsUniqueUser(userRegisterDto.nickName);

            if(userRegisterValidate)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest(_apiResponse);
            }

            var user = await _userRepo.Register(userRegisterDto);

            if (user == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = user;
            return Ok(_apiResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var user = await _userRepo.Login(userLoginDto);

            if (user.User == null || string.IsNullOrEmpty(user.Token))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("El nombre de usuario o contraseña son incorrectos");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            _apiResponse.Result = user;
            return Ok(_apiResponse);
        }

        [HttpPost]
        public async Task<IActionResult> ValidateToken(string token)
        {
            if (token != null)
            {
                var user = await _userRepo.ValidateToken(token);

                if (user.User.name != "error")
                {
                    _apiResponse.StatusCode = HttpStatusCode.OK;
                    _apiResponse.IsSuccess = true;
                    _apiResponse.Result = user;
                    return Ok(_apiResponse);
                }

                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Hubo un problema");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add("El token está vacío");
            return BadRequest(_apiResponse);
        }
    }
}
