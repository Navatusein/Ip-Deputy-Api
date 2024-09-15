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
    [Tags("Frontend:SubmissionStudents Controller")]
    [Route("frontend/submission-student")]
    [ApiController]
    public class SubmissionStudentController : ControllerBase
    {
        private static ILogger Logger => Log.ForContext<SubmissionStudentController>();
        private readonly IpDeputyDbContext _context;
        private readonly IMapper _mapper;

        public SubmissionStudentController(IpDeputyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("by-student/{studentId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SubmissionStudentDto>>> GetByStudentAsync([FromRoute] int studentId, CancellationToken cancellationToken)
        {
            var dtos = await _context.SubmissionStudents
                .Where(x => x.StudentId == studentId)
                .OrderBy(x => x.SubmissionsConfigId)
                .ThenBy(x => x.SubmissionWork.Name)
                .Select(x => _mapper.Map<SubmissionStudentDto>(x))
                .ToListAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, dtos);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SubmissionStudentDto>>> GetAsync(CancellationToken cancellationToken)
        {
            var dtos = await _context.SubmissionStudents
                .OrderBy(x => x.SubmissionsConfigId)
                .ThenBy(x => x.SubmissionWork.Name)
                .Select(x => _mapper.Map<SubmissionStudentDto>(x))
                .ToListAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, dtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<SubmissionStudentDto>> AddAsync([FromBody] SubmissionStudentDto dto, CancellationToken cancellationToken)
        {
            var model = _mapper.Map<SubmissionStudent>(dto);

            await _context.AddAsync(model, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var others = _context.SubmissionStudents
                .Where(x => x.SubmissionsConfigId == dto.SubmissionsConfigId && x.StudentId == dto.StudentId)
                .ToList()
                .Select(x => { x.PreferredPosition = dto.PreferredPosition; return x; });

            _context.UpdateRange(others);
            await _context.SaveChangesAsync(cancellationToken);

            dto.Id = model.Id;

            return StatusCode(StatusCodes.Status201Created, dto);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<SubmissionStudentDto>> UpdateAsync([FromBody] SubmissionStudentDto dto, CancellationToken cancellationToken)
        {
            if (!_context.SubmissionStudents.Any(x => x.Id == dto.Id))
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            var model = _mapper.Map<SubmissionStudent>(dto);

            _context.Update(model);
            await _context.SaveChangesAsync(cancellationToken);

            var others = _context.SubmissionStudents
                .Where(x => x.SubmissionsConfigId == dto.SubmissionsConfigId && x.StudentId == dto.StudentId)
                .ToList()
                .Select(x => { x.PreferredPosition = dto.PreferredPosition; return x; });

            _context.UpdateRange(others);
            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, model);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<ActionResult<int>> DeleteAsync([FromRoute] int id, CancellationToken cancellationToken)
        {
            var model = await _context.SubmissionStudents.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (model == null)
                throw new HttpException("Invalid id", StatusCodes.Status404NotFound);

            _context.Remove(model);
            await _context.SaveChangesAsync(cancellationToken);

            return StatusCode(StatusCodes.Status200OK, model);
        }
    }
}
