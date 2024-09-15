using AutoMapper;
using IpDeputyApi.Authentication;
using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Bot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

using ILogger = Serilog.ILogger;

namespace IpDeputyApi.Controllers.Bot;

[Tags("Bot:Submission Controller")]
[Route("bot/submission")]
[ApiController]
public class SubmissionController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<SubmissionController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public SubmissionController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("submissions-configs")]
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    public async Task<ActionResult<IEnumerable<SubmissionsConfigDataDto>>> GetSubmissionsConfigsAsync(CancellationToken cancellationToken)
    {
        var submissionsConfigs = await _context.SubmissionsConfigs.ToListAsync(cancellationToken);


        var dtos = submissionsConfigs.Select(submissionsConfig =>
        {
            return new SubmissionsConfigDataDto
            {
                Id = submissionsConfig.Id,
                Name = submissionsConfig.CustomName ?? submissionsConfig.Subject!.ShortName,
                Type = submissionsConfig.CustomType ?? submissionsConfig.SubjectType!.ShortName,
                Subgroup = submissionsConfig.Subgroup?.Name,
                ClearedAt = submissionsConfig.SubmissionStudents.OrderBy(x => x.Id).FirstOrDefault()?.SubmittedAt.ToString() ?? "",
                Submissions = submissionsConfig.SubmissionStudents
                    .OrderBy(x => x.PreferredPosition)
                    .ThenBy(x => x.StudentId)
                    .ThenBy(x => x.SubmissionWork.Index)
                    .Select(x => new SubmissionDto() { Name = x.SubmissionWork.Name, Student = $"{x.Student.Surname} {x.Student.Name}" })
            };
        })
        .ToList();

        return StatusCode(StatusCodes.Status200OK, dtos);
    }
}