using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    [HttpPost("log")]
    public IActionResult Log([FromBody] ChatLogDto dto)
    {
        Console.WriteLine($"[{dto.Timestamp}] {dto.ClientId}: {dto.Message}");
        return Ok();
    }
}

public record ChatLogDto(string ClientId, string Message, DateTime Timestamp);