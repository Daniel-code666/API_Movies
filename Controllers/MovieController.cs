using API_Movies.Models;
using API_Movies.Models.Dtos;
using API_Movies.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net;
using XAct.Messages;

namespace API_Movies.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiExplorerSettings(GroupName = "movies")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _ctRepo;
        private IWebHostEnvironment _hostingEnv;
        private ApiResponse _apiResponse;

        public MovieController(IMovieRepository movieRepository, ICategoryRepository ctRepo, IMapper mapper,
            IWebHostEnvironment hostingEnv)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
            _ctRepo = ctRepo;
            _hostingEnv = hostingEnv;
            _apiResponse = new();
        }

        [HttpGet]
        public IActionResult GetAllMovies()
        {
            var movList = _movieRepository.GetAllMovies();
            //var movListDto = new List<MovieDto>();

            //foreach(var value in movList)
            //{
            //    movListDto.Add(_mapper.Map<MovieDto>(value));
            //}

            return Ok(movList);
        }

        [HttpGet("{id}", Name = "GetSingleMovie")]
        public IActionResult GetSingleMovie(int id)
        {
            if (_movieRepository.MovieExists(id))
            {
                var mov = _movieRepository.GetSingleMovie(id);
                // var movDto = _mapper.Map<MovieDto>(mov);

                return Ok(mov);
            }

            return NotFound("No existe película con ese id");
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromForm] MovieCreateDto movieDto)
        {
            if (!ModelState.IsValid || movieDto == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Faltan datos");
                return BadRequest(_apiResponse);
            }

            if (_movieRepository.MovieExist(movieDto.title))
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("La película ya existe");
                return NotFound(_apiResponse);
            }

            if(movieDto.movieImg == null)
            {
                var movie = _mapper.Map<Movie>(movieDto);

                if (_movieRepository.CreateMovie(movie))
                {
                    _apiResponse.StatusCode = HttpStatusCode.OK;
                    _apiResponse.IsSuccess = true;
                    _apiResponse.ErrorMessages.Add("Película creada");
                    _apiResponse.Result = movieDto;
                    return Ok(_apiResponse);
                }
            }
            else
            {
                var img = movieDto.movieImg;
                string rutaPrincipal = _hostingEnv.WebRootPath;
                var imgList = HttpContext.Request.Form.Files;

                var nombreFoto = Guid.NewGuid().ToString();
                var ruta = Path.Combine(rutaPrincipal, @"Images");
                var extension = Path.GetExtension(imgList[0].FileName);

                using (var fileStreams = new FileStream(Path.Combine(ruta, nombreFoto + extension), FileMode.Create))
                {
                    imgList[0].CopyTo(fileStreams);
                }

                movieDto.imgRoute = @"\Images\" + nombreFoto + extension;

                var movie = _mapper.Map<Movie>(movieDto);

                if (_movieRepository.CreateMovie(movie))
                {
                    _apiResponse.StatusCode = HttpStatusCode.OK;
                    _apiResponse.IsSuccess = true;
                    _apiResponse.ErrorMessages.Add("Película creada");
                    _apiResponse.Result = movieDto;
                    return Ok(_apiResponse);
                }
            }

            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add("Faltan datos");
            return BadRequest(_apiResponse);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateMovie(int id, [FromForm] MovieDto movieDto)
        {
            if (!ModelState.IsValid || movieDto == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Faltan datos");
                return BadRequest(_apiResponse);
            }

            if (id != int.Parse(movieDto.idMovie))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Los id's no coinciden");
                return BadRequest(_apiResponse);
            }

            if (movieDto.movieImg == null)
            {
                if (_movieRepository.MovieExists(int.Parse(movieDto.idMovie)))
                {
                    var movie = _mapper.Map<Movie>(movieDto);

                    if (_movieRepository.UpdtMovie(movie))
                    {
                        _apiResponse.StatusCode = HttpStatusCode.OK;
                        _apiResponse.IsSuccess = true;
                        _apiResponse.ErrorMessages.Add("Película actualizada");
                        _apiResponse.Result = movie;
                        return Ok(_apiResponse);
                    }

                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add("Hubo un error");
                    return BadRequest(_apiResponse);
                }
            }
            else
            {
                var img = movieDto.movieImg;
                string rutaPrincipal = _hostingEnv.WebRootPath;
                var imgList = HttpContext.Request.Form.Files;

                var nombreFoto = Guid.NewGuid().ToString();
                var ruta = Path.Combine(rutaPrincipal, @"Images");
                var extension = Path.GetExtension(imgList[0].FileName);

                using (var fileStreams = new FileStream(Path.Combine(ruta, nombreFoto + extension), FileMode.Create))
                {
                    imgList[0].CopyTo(fileStreams);
                }

                movieDto.imgRoute = @"\Images\" + nombreFoto + extension;

                var movie = _mapper.Map<Movie>(movieDto);

                if (_movieRepository.UpdtMovie(movie))
                {
                    _apiResponse.StatusCode = HttpStatusCode.OK;
                    _apiResponse.IsSuccess = true;
                    _apiResponse.ErrorMessages.Add("Película actualizada");
                    _apiResponse.Result = movie;
                    return Ok(_apiResponse);
                }
            }

            _apiResponse.StatusCode = HttpStatusCode.NotFound;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add("Película no encontrada");
            return NotFound(_apiResponse);
        }

        [HttpGet("{id}")]
        public IActionResult GetMoviesOnCategory(int id)
        {
            return _ctRepo.CategoryExists(id) ? Ok(_movieRepository.GetMovieInCategory(id)) : 
                NotFound("No hay categoría con ese id");
        }

        [HttpGet("{searchText}")]
        public IActionResult SearchMovie(string searchText)
        {
            try
            {
                if (searchText == null) return BadRequest();

                var result = _movieRepository.SearchMovie(searchText.Trim());

                return result.Any() ? Ok(result) : NotFound("No hay nada que coincida con la búsqueda"); 
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            return _movieRepository.MovieExists(id) ? Ok(_movieRepository.DeleteMovie(id)) : 
                NotFound("No hay película con ese id");
        }
    }
}
