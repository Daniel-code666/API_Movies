using API_Movies.Models;
using API_Movies.Models.Dtos;
using API_Movies.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using XAct.Users;

namespace API_Movies.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _ctRepo;
        private readonly IMapper _mapper;
        private ApiResponse _apiResponse;

        public CategoryController(ICategoryRepository ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
            this._apiResponse = new();
        }

        [HttpGet]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllCategory()
        {
            var catList = _ctRepo.GetAllCategory();
            var catListDto = new List<CategoryDto>();

            foreach (var value in catList)
            {
                catListDto.Add(_mapper.Map<CategoryDto>(value));
            }

            return Ok(catListDto);
        }

        [HttpGet("{id}", Name = "GetSingleCategory")]
        [ResponseCache(CacheProfileName = "Default10Sec")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetSingleCategory(int id)
        {
            var cat = _ctRepo.GetSingleCategory(id);

            var catDto = _mapper.Map<CategoryDto>(cat);

            if (cat != null)
            {
                return Ok(catDto);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid || createCategoryDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_ctRepo.CategoryExist(createCategoryDto.name))
            {
                ModelState.AddModelError("", "La categoría ya existe");
                return StatusCode(409, ModelState);
            }

            var cat = _mapper.Map<Category>(createCategoryDto);

            if (_ctRepo.CreateCategory(cat))
            {
                return CreatedAtRoute("GetSingleCategory", new { id = cat.id }, cat);
            }

            return StatusCode(500, "Hubo un error en el servidor");
        }

        [HttpPatch("{id}", Name = "UpdateCategory")]
        public IActionResult UpdateCategory([FromRoute]int id, [FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid || categoryDto == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Faltan datos");
                return BadRequest(_apiResponse);
            } 
            else if (_ctRepo.CategoryExists(categoryDto.id) && _ctRepo.CategoryExists(id))
            {
                var category = _ctRepo.GetSingleCategory(id);

                if (category.name == categoryDto.name)
                {
                    _apiResponse.StatusCode = HttpStatusCode.OK;
                    _apiResponse.IsSuccess = true;
                    _apiResponse.ErrorMessages.Add("El nombre es el mismo");
                    _apiResponse.Result = categoryDto.name;
                    return Ok(_apiResponse);
                }
                else if (_ctRepo.CategoryExist(categoryDto.name))
                {
                    _apiResponse.StatusCode = HttpStatusCode.Conflict;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add("Categoría ya existente");
                    return Conflict(_apiResponse);
                }

                if (id != categoryDto.id)
                {
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add("Los id no coinciden");
                    return BadRequest(_apiResponse);
                }

                var cat = _mapper.Map<Category>(categoryDto);
                if (_ctRepo.UpdtCategory(cat))
                {
                    _apiResponse.StatusCode = HttpStatusCode.OK;
                    _apiResponse.IsSuccess = true;
                    _apiResponse.ErrorMessages.Add("Categoría actualizada");
                    _apiResponse.Result = categoryDto.name;
                    return Ok(_apiResponse);
                }
            }

            _apiResponse.StatusCode = HttpStatusCode.NotFound;
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages.Add("No existe categoría con ese id");
            _apiResponse.Result = categoryDto;
            return NotFound(_apiResponse);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory([FromRoute] int id)
        {
            try
            {
                if (_ctRepo.CategoryExists(id))
                {
                    if (_ctRepo.DeleteCategory(id))
                    {
                        _apiResponse.StatusCode = HttpStatusCode.OK;
                        _apiResponse.IsSuccess = true;
                        _apiResponse.Result = true;
                        return Ok(_apiResponse);
                    }

                    _apiResponse.StatusCode = HttpStatusCode.ServiceUnavailable;
                    _apiResponse.IsSuccess = false;
                    _apiResponse.ErrorMessages.Add("Hubo un problema...");
                    return BadRequest(_apiResponse);
                }

                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("No hay categoría con ese id");
                return BadRequest(_apiResponse);
            }
            catch(Exception ex)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add(ex.Message);
                return BadRequest(_apiResponse);
            }
        }
    }
}
