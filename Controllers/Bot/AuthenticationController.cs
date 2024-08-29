using AutoMapper;
using IpDeputyApi.Authentication;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Database;
using IpDeputyApi.Dto.Bot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IpDeputyApi.Exceptions;

namespace IpDeputyApi.Controllers.Bot
{
    [Tags("Bot Authentication Controller")]
    [Route("bot/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private static Serilog.ILogger Logger => Serilog.Log.ForContext<AuthenticationController>();
        private readonly IpDeputyDbContext _context;
        private readonly IMapper _mapper;

        public AuthenticationController(IpDeputyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
        [Route("authorize")]
        [HttpPost]
        public async Task<ActionResult<string>> Authorize([FromBody] StudentContactDto contactDto)
        {
            var student = await _context.Students.FirstOrDefaultAsync(x => x.TelegramPhone == contactDto.Phone);

            if (student == null)
            {
                var code = Guid.NewGuid().ToString()[..8];
                var phone = contactDto.Phone;
         
                throw new HttpException($"[{code}] Student with phone: {phone} not found!", StatusCodes.Status404NotFound);
            }

            var telegram = new Telegram()
            {
                StudentId = student.Id,
                TelegramId = contactDto.TelegramId,
                Language = "uk",
                ScheduleCompact = false
            };

            await _context.AddAsync(telegram);
            await _context.SaveChangesAsync();
            return Ok("Ok");
        }
    }
}
