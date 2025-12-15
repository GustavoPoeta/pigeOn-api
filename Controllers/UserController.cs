using Microsoft.AspNetCore.Mvc;
using NATS.Client.Service;
using pigeon_api.Contexts;
using pigeon_api.Dtos;
using pigeon_api.Models;

namespace pigeon_api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _service.Get(id) ?? throw new NotFoundException("User not found");;

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _service.GetAll();
            return Ok(users);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create ([FromBody] UserDto user) 
        {
            await _service.Create(user);
            return Created();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update ([FromBody] UserDto user) 
        {
            await _service.Update(user);
            return Ok();
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete (int id)
        {
            await _service.Delete(id);
            return Ok();
        }
    }
}
