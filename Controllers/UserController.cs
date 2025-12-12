using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using pigeon_api.Contexts;
using pigeon_api.Models;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int? id) 
        {
           User user = await service.Get(id);
           return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll ()
        {
            List<User> users = await service.GetAll();
            return Ok(users);
        }
    }
}