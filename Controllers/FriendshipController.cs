using Microsoft.AspNetCore.Mvc;
using pigeon_api.Dtos;
using pigeon_api.Models;
using pigeon_api.Services;

namespace pigeon_api.Controllers;

[ApiController]
[Route("api/friendships")]
public class FriendshipController : ControllerBase
{
    private readonly FriendshipService _service;

    public FriendshipController(FriendshipService service)
    {
        _service = service;
    }

    [HttpGet("user/{id:int}")]
    public async Task<IActionResult> GetUserFriendships(int id)
    {
        var friendships = await _service.GetUserFriendships(id);
        return Ok(friendships);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetFriendship(int id)
    {
        var friendship = await _service.GetFriendship(id)
            ?? throw new NotFoundException("Friendship not found");

        return Ok(friendship);
    }

    [HttpGet("mutual/{userId:int}&{friendId:int}")]
    public async Task<IActionResult> IsFriendshipMutual(int userId, int friendId)
    {
        var isMutual = await _service.IsFrendshipMutual(userId, friendId);
        return Ok(isMutual);
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestFriendship([FromBody] FriendshipDto friendship)
    {
        await _service.RequestFriendship(friendship);
        return Created();
    }

    [HttpPost("accept")]
    public async Task<IActionResult> AcceptFriendship([FromBody] FriendshipDto friendship)
    {
        await _service.AcceptFriendship(friendship);
        return Created();
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] FriendshipDto friendship)
    {
        await _service.Update(friendship);
        return Ok();
    }

    [HttpDelete("delete/{userId:int}&{friendId:int}")]
    public async Task<IActionResult> Delete(int userId, int friendId)
    {
        await _service.Delete(userId, friendId);
        return Ok();
    }
}
