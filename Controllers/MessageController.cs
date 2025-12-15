using Microsoft.AspNetCore.Mvc;
using pigeon_api.Dtos;
using pigeon_api.Services;

namespace pigeon_api.Controllers;

[ApiController]
[Route("api/messages")]
public class MessageController : ControllerBase
{

    private readonly MessageService _service;

    public MessageController (MessageService service)
    {
        _service = service;
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserMessages(int userId)
    {
        var messages = await _service.GetUserMessages(userId);
        return Ok(messages);
    }

    [HttpGet("friendship/{userId:int}&{friendId:int}")]
    public async Task<IActionResult> GetFriendshipMessages(int userId, int friendId)
    {
        var messages = await _service.GetFriendshipMessages(userId, friendId);
        return Ok(messages);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateMessage([FromBody] MessageDto messageDto)
    {
        await _service.CreateMessage(messageDto);
        return Created();
    }

    [HttpPut("update/markAsViewed/{messageId:int}")]
    public async Task<IActionResult> MarkMessageAsViewed(int messageId)
    {
        await _service.MarkMessageAsViewed(messageId);
        return Ok();
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateMessageContent([FromBody] UpdateMessageDto updateMessageDto)
    {
        await _service.UpdateMessageContent(updateMessageDto);
        return Ok();
    }

    [HttpDelete("delete/{messageId:int}")]
    public async Task<IActionResult> DeleteMessage(int messageId)
    {
        await _service.DeleteMessage(messageId);
        return Ok();
    }
}