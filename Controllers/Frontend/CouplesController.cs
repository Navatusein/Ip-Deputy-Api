using AutoMapper;
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
    [Tags("Frontend:Couples Controller")]
    [Route("frontend/couple")]
    [ApiController]
    public class CouplesController : ControllerBase
    {
        private static ILogger Logger => Log.ForContext<CouplesController>();
        private readonly IpDeputyDbContext _context;
        private readonly IMapper _mapper;

        public CouplesController(IpDeputyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{dayOfWeekId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CoupleDto>>> GetByDayOfWeekIdAsync([FromRoute] int dayOfWeekId, CancellationToken cancellationToken)
        {
            var models = await _context.Couples
               .Where(x => x.DayOfWeekId == dayOfWeekId)
               .OrderBy(x => x.CoupleTimeId)
               .ThenBy(x => x.StartDate)
               .ToListAsync(cancellationToken);

            var dtos = new List<CoupleDto>();

            foreach (var model in models)
            {
                var dto = _mapper.Map<CoupleDto>(model);

                dto.AdditionalDates = model.CoupleDates
                    .Where(x => x.IsRemovedDate == false)
                    .Select(x => _mapper.Map<CoupleDateDto>(x))
                    .ToList();

                dto.RemovedDates = model.CoupleDates
                    .Where(x => x.IsRemovedDate == true)
                    .Select(x => _mapper.Map<CoupleDateDto>(x))
                    .ToList();

                dtos.Add(dto);
            }

            return StatusCode(StatusCodes.Status200OK, dtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CoupleDto>> AddAsync([FromBody] CoupleDto dto, CancellationToken cancellationToken)
        {
            var model = _mapper.Map<Couple>(dto);

            await _context.AddAsync(model, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _context.AddRangeAsync(dto.AdditionalDates
                .Select(x => _mapper.Map<CoupleDate>(x, opt =>
                {
                    opt.AfterMap((_, dest) =>
                    {
                        dest.CoupleId = model.Id;
                        dest.IsRemovedDate = false;
                    });
                })).ToList(), cancellationToken
            );

            await _context.AddRangeAsync(dto.RemovedDates
                .Select(x => _mapper.Map<CoupleDate>(x, opt =>
                {
                    opt.AfterMap((_, dest) =>
                    {
                        dest.CoupleId = model.Id;
                        dest.IsRemovedDate = true;
                    });
                }))
                .ToList(), cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);

            dto.Id = model.Id;

            return StatusCode(StatusCodes.Status201Created, dto);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<CoupleDto>> UpdateAsync([FromBody] CoupleDto dto, CancellationToken cancellationToken)
        {
            if (!_context.Couples.Any(x => x.Id == dto.Id))
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            var model = _mapper.Map<Couple>(dto);

            _context.Update(model);
            await _context.SaveChangesAsync(cancellationToken);

            _context.RemoveRange(await _context.CoupleDates.Where(x => x.CoupleId == dto.Id).ToListAsync(cancellationToken));

            await _context.AddRangeAsync(dto.AdditionalDates
                .Select(x => _mapper.Map<CoupleDate>(x, opt =>
                {
                    opt.AfterMap((_, dest) =>
                    {
                        dest.CoupleId = dto.Id;
                        dest.IsRemovedDate = false;
                    });
                }))
                .ToList(), cancellationToken
            );

            await _context.AddRangeAsync(dto.RemovedDates
                .Select(x => _mapper.Map<CoupleDate>(x, opt =>
                {
                    opt.AfterMap((_, dest) =>
                    {
                        dest.CoupleId = dto.Id;
                        dest.IsRemovedDate = true;
                    });
                }))
                .ToList(), cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, dto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<ActionResult<int>> DeleteAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var model = await _context.Couples.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (model == null)
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            _context.Remove(model);
            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, id);
        }
    }
}
