using Microsoft.AspNetCore.Mvc;
using pigeon_api.Contexts;
using pigeon_api.Dtos;
using pigeon_api.Services;

namespace pigeon_api.Controllers
{
    [ApiController]
    [Route("api/friendships")]
    public class FriendshipController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly FriendshipService service;

        public FriendshipController(AppDbContext context)
        {
            _context = context;
            service = new(_context);
        }

        [HttpGet("user/{id:int}")]
        public async Task<IActionResult> GetUserFriendships(int id)
        {
            var friendships = await service.GetUserFriendships(id);
            return Ok(friendships);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetFriendship(int id)
        {
            var friendship = await service.GetFriendship(id);
            if (friendship == null)
                return NotFound();
            return Ok(friendship);
        }

        [HttpGet("mutual/{userId:int}&{friendId:int}")]
        public async Task<IActionResult> IsFriendshipMutual(int userId, int friendId)
        {
            var isMutual = await service.IsFrendshipMutual(userId, friendId);
            return Ok(isMutual);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] FriendshipDto friendship)
        {
            await service.Create(friendship);
            return Created();
        }

        // meant for updating the save option of the friendship (e.g. if the messages should be saved in DB or cache)
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] FriendshipDto friendship)
        {
            await service.Update(friendship);
            return Ok();
        }

        // the delete method deletes both sides of the friendship
        [HttpDelete("delete/{userId:int}&{friendId:int}")]
        public async Task<IActionResult> Delete(int userId, int friendId)
        {
            await service.Delete(userId, friendId);
            return Ok();
        }
    }
}