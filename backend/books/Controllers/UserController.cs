using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser([FromForm] CreateUserDto createUserDto)
        {
            await _service.CreateUserAsync(createUserDto);
            return StatusCode(201,"User Created");
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login([FromForm] UserLoginDto userLoginDto)
        {
            try
            {
                var token = await _service.Login(userLoginDto.Email, userLoginDto.Password);
                return StatusCode(200, token);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _service.GetUserByIdAsync(id);
                return Ok(user);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
