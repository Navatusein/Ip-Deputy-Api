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
    [Tags("Frontend:SubmissionsConfigs Controller")]
    [Route("frontend/submissions-config")]
    [ApiController]
    public class SubmissionsConfigsController : ControllerBase
    {
        private static ILogger Logger => Log.ForContext<SubmissionsConfigsController>();
        private readonly IpDeputyDbContext _context;
        private readonly IMapper _mapper;

        public SubmissionsConfigsController(IpDeputyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("for-student/{studentId}/{otherSubgroups}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SubmissionsConfigDto>>> GetForStudentAsync([FromRoute] int studentId, [FromRoute] bool otherSubgroups, CancellationToken cancellationToken)
        {
            var student = await _context.Students.FirstOrDefaultAsync(x => x.Id == studentId, cancellationToken);

            if (student == null)
                throw new HttpException("Invalid student id", StatusCodes.Status400BadRequest);

            var models = await _context.SubmissionsConfigs
                .Where(x => x.SubgroupId == null || x.SubgroupId == student.SubgroupId || otherSubgroups)
                .ToListAsync(cancellationToken);

            var dtos = new List<SubmissionsConfigDto>();

            foreach (var model in models)
            {
                var dto = _mapper.Map<SubmissionsConfigDto>(model);

                dto.SubmissionWorks = model.SubmissionWorks
                 .OrderBy(x => x.Index)
                 .Select(x => _mapper.Map<SubmissionWorkDto>(x))
                 .ToList();

                dto.SubmissionStudents = model.SubmissionStudents
                .Select(x => _mapper.Map<SubmissionStudentDto>(x))
                .ToList();

                dtos.Add(dto);
            }

            return StatusCode(StatusCodes.Status200OK, dtos);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SubmissionsConfigDto>>> GetAsync(CancellationToken cancellationToken)
        {
            var models = await _context.SubmissionsConfigs
                .ToListAsync(cancellationToken);


            var dtos = new List<SubmissionsConfigDto>();

            foreach (var model in models)
            {
                var dto = _mapper.Map<SubmissionsConfigDto>(model);

                dto.SubmissionWorks = model.SubmissionWorks
                 .OrderBy(x => x.Index)
                 .Select(x => _mapper.Map<SubmissionWorkDto>(x))
                 .ToList();

                dto.SubmissionStudents = model.SubmissionStudents
                .Select(x => _mapper.Map<SubmissionStudentDto>(x))
                .ToList();

                dtos.Add(dto);
            }

            return StatusCode(StatusCodes.Status200OK, dtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SubmissionsConfigDto>> AddAsync([FromBody] SubmissionsConfigDto dto, CancellationToken cancellationToken)
        {
            dto.SubmissionWorks = dto.SubmissionWorks.Select(x => { x.Id = 0; return x; });

            var model = _mapper.Map<SubmissionsConfig>(dto);

            await _context.AddAsync(model, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _context.AddRangeAsync(dto.SubmissionWorks
               .Select(x => _mapper.Map<SubmissionWork>(x, opt =>
               {
                   opt.AfterMap((_, dest) =>
                   {
                       dest.SubmissionsConfigId = model.Id;
                   });
               })).ToList(), cancellationToken
            );

            dto.Id = model.Id;

            return StatusCode(StatusCodes.Status201Created, dto);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<SubmissionsConfigDto>> UpdateAsync([FromBody] SubmissionsConfigDto dto, CancellationToken cancellationToken)
        {
            if (!_context.SubmissionsConfigs.Any(x => x.Id == dto.Id))
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            dto.SubmissionWorks = dto.SubmissionWorks.Select(x => { x.SubmissionsConfigId = dto.Id; return x; }).ToList();

            var model = _mapper.Map<SubmissionsConfig>(dto);

            _context.Update(model);
            await _context.SaveChangesAsync(cancellationToken);

            _context.RemoveRange(await _context.SubmissionWorks.Where(x => x.SubmissionsConfigId == dto.Id).ToListAsync(cancellationToken));

            await _context.AddRangeAsync(dto.SubmissionWorks
               .Select(x => _mapper.Map<SubmissionWork>(x, opt =>
               {
                   opt.AfterMap((_, dest) =>
                   {
                       dest.SubmissionsConfigId = model.Id;
                   });
               })).ToList(), cancellationToken
            );

            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, dto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<ActionResult<int>> DeleteAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var model = await _context.SubmissionsConfigs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (model == null)
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            _context.Remove(model);
            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, model);
        }
    }
}
