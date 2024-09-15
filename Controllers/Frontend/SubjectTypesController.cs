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
    [Tags("Frontend:SubjectTypes Controller")]
    [Route("frontend/subject-type")]
    [ApiController]
    public class SubjectTypeTypesController : ControllerBase
    {
        private static ILogger Logger => Log.ForContext<SubjectTypeTypesController>();
        private readonly IpDeputyDbContext _context;
        private readonly IMapper _mapper;

        public SubjectTypeTypesController(IpDeputyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SubjectTypeDto>>> GetAsync(CancellationToken cancellationToken)
        {
            var dtos = await _context.SubjectTypes
                .Select(x => _mapper.Map<SubjectTypeDto>(x))
                .ToListAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, dtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SubjectTypeDto>> AddAsync([FromBody] SubjectTypeDto dto, CancellationToken cancellationToken)
        {
            var model = _mapper.Map<SubjectType>(dto);

            await _context.AddAsync(model, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            dto.Id = model.Id;

            return StatusCode(StatusCodes.Status201Created, dto);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<SubjectTypeDto>> UpdateAsync([FromBody] SubjectTypeDto dto, CancellationToken cancellationToken)
        {
            if (!_context.SubjectTypes.Any(x => x.Id == dto.Id))
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            var model = _mapper.Map<SubjectType>(dto);

            _context.Update(model);
            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, dto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<ActionResult<int>> DeleteAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var model = await _context.SubjectTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (model == null)
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            _context.Remove(model);
            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, id);
        }
    }
}
