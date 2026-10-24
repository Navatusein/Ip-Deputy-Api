using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Bot;

[Tags("Bot:Authentication Controller")]
[Route("bot/authentication")]
public class AuthenticationController : BotControllerBase
{
    private readonly IBotStudentService _studentService;

    public AuthenticationController(IBotStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpPost("authorize")]
    public async Task<ActionResult<string>> AuthorizeAsync([FromBody] StudentContactDto contactDto, CancellationToken cancellationToken)
    {
        await _studentService.AuthorizeAsync(contactDto, cancellationToken);
        return Ok("Ok");
    }
}
