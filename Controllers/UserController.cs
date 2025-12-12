using Microsoft.AspNetCore.Mvc;
using pigeon_api.Contexts;
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
            var user = await service.Get(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await service.GetAll();
            return Ok(users);
        }
    }
}
