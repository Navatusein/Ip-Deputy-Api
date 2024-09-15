using AutoMapper;
using Humanizer;
using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

using ILogger = Serilog.ILogger;

namespace IpDeputyApi.Controllers.Frontend
{
    [Tags("Frontend:AdditionalCouples Controller")]
    [Route("frontend/additional-couple")]
    [ApiController]
    public class AdditionalCouplesController : ControllerBase
    {
        private static ILogger Logger => Log.ForContext<AdditionalCouplesController>();
        private readonly IpDeputyDbContext _context;
        private readonly IMapper _mapper;

        public AdditionalCouplesController(IpDeputyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AdditionalCoupleDto>>> GetAsync(CancellationToken cancellationToken)
        {
            var dtos = await _context.AdditionalCouples
                .Select(x => _mapper.Map<AdditionalCoupleDto>(x))
                .ToListAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, dtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AdditionalCoupleDto>> AddAsync([FromBody] AdditionalCoupleDto dto, CancellationToken cancellationToken)
        {
            var model = _mapper.Map<AdditionalCouple>(dto);

            await _context.AddAsync(model, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            dto.Id = model.Id;

            return StatusCode(StatusCodes.Status201Created, dto);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<AdditionalCoupleDto>> UpdateAsync([FromBody] AdditionalCoupleDto dto, CancellationToken cancellationToken)
        {
            if (!_context.AdditionalCouples.Any(x => x.Id == dto.Id))
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            var model = _mapper.Map<AdditionalCouple>(dto);

            _context.Update(model);
            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, dto);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<int>> DeleteAsync([FromQuery] int id, CancellationToken cancellationToken)
        {
            var model = await _context.AdditionalCouples.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (model == null)
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            _context.Remove(model);
            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, id);
        }
    }
}
