using Microsoft.AspNetCore.Mvc;
using pigeon_api.Contexts;
using pigeon_api.Dtos;
using pigeon_api.Models;

namespace pigeon_api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserService service;

        public UserController(AppDbContext context)
        {
            _context = context;
            service = new(_context);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await service.Get(id) ?? throw new NotFoundException("User not found");;

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await service.GetAll();
            return Ok(users);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create ([FromBody] UserDto user) 
        {
            await service.Create(user);
            return Created();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update ([FromBody] UserDto user) 
        {
            await service.Update(user);
            return Ok();
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> Delete (int id)
        {
            await service.Delete(id);
            return Ok();
        }
    }
}
