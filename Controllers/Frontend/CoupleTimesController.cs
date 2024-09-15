using AutoMapper;
using IpDeputyApi.Database;
using IpDeputyApi.Dto.Frontend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

using ILogger = Serilog.ILogger;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Controllers.Frontend
{
    [Tags("Frontend:CoupleTimes Controller")]
    [Route("frontend/couple-time")]
    [ApiController]
    public class CoupleTimesController : ControllerBase
    {
        private static ILogger Logger => Log.ForContext<CoupleTimesController>();
        private readonly IpDeputyDbContext _context;
        private readonly IMapper _mapper;

        public CoupleTimesController(IpDeputyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CoupleTimeDto>>> GetAsync(CancellationToken cancellationToken)
        {
            var dtos = await _context.CoupleTimes
                .OrderBy(x => x.TimeStart)
                .Select(x => _mapper.Map<CoupleTimeDto>(x))
                .ToListAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, dtos);
        }
    }
}
