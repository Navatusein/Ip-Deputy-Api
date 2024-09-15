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
    [Tags("Frontend:Subgroups Controller")]
    [Route("frontend/subgroup")]
    [ApiController]
    public class SubgroupsController : ControllerBase
    {
        private static ILogger Logger => Log.ForContext<SubgroupsController>();
        private readonly IpDeputyDbContext _context;
        private readonly IMapper _mapper;

        public SubgroupsController(IpDeputyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SubgroupDto>>> GetAsync(CancellationToken cancellationToken)
        {
            var dtos = await _context.Subgroups
                .Select(x => _mapper.Map<SubgroupDto>(x))
                .ToListAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, dtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SubgroupDto>> AddAsync([FromBody] SubgroupDto dto, CancellationToken cancellationToken)
        {
            var model = _mapper.Map<Subgroup>(dto);

            await _context.AddAsync(model, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            dto.Id = model.Id;

            return StatusCode(StatusCodes.Status201Created, dto);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<SubgroupDto>> UpdateAsync([FromBody] SubgroupDto dto, CancellationToken cancellationToken)
        {
            if (!_context.Subgroups.Any(x => x.Id == dto.Id))
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            var model = _mapper.Map<Subgroup>(dto);

            _context.Update(model);
            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, dto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<ActionResult<int>> DeleteAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var model = await _context.Subgroups.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (model == null)
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            _context.Remove(model);
            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, id);
        }
    }
}
