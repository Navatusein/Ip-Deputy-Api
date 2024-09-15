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

[Tags("Bot:Information Controller")]
[Route("bot/information")]
[ApiController]
public class InformationController : ControllerBase
{
    private static ILogger Logger => Log.ForContext<InformationController>();
    private readonly IpDeputyDbContext _context;
    private readonly IMapper _mapper;

    public InformationController(IpDeputyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("students")]
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    public async Task<ActionResult<IEnumerable<StudentInformationDto>>> GetStudentsInformationAsync(CancellationToken cancellationToken)
    {
        var students = await _context.Students
            .Include(x => x.Subgroup)
            .OrderBy(x => x.Index)
            .Select(x => _mapper.Map<StudentInformationDto>(x))
            .ToListAsync(cancellationToken);

        return StatusCode(StatusCodes.Status200OK, students);
    }

    [HttpGet]
    [Route("teachers")]
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    public async Task<ActionResult<IEnumerable<TeacherInformationDto>>> GetTeachersInformationAsync(CancellationToken cancellationToken)
    {
        var teachers = await _context.Teachers
            .OrderBy(x => x.Surname)
            .Select(x => _mapper.Map<TeacherInformationDto>(x))
            .ToListAsync(cancellationToken);

        return StatusCode(StatusCodes.Status200OK, teachers);
    }

    [HttpGet]
    [Route("subjects")]
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    public async Task<ActionResult<IEnumerable<SubjectInformationDto>>> GetSubjectsInformationAsync(CancellationToken cancellationToken)
    {
        var subjects = await _context.Subjects.ToListAsync(cancellationToken);
        var subjectInformationDtos = new List<SubjectInformationDto>();
        
        foreach (var subject in subjects)
        {
            var subjectInformationDto = new SubjectInformationDto
            {
                Name = subject.Name,
                ShortName = subject.ShortName,
                LaboratoryCount = subject.LaboratoryCount,
                PracticalCount = subject.PracticalCount,
                LaboratoryDaysCount = await CalculateDaysAsync(subject, (int)SubjectTypes.Laboratory, cancellationToken),
                PracticalDaysCount = await CalculateDaysAsync(subject, (int)SubjectTypes.Practice, cancellationToken),
                LecturesDaysCount = await CalculateDaysAsync(subject, (int)SubjectTypes.Lecture, cancellationToken)
            };
            
            subjectInformationDtos.Add(subjectInformationDto);
        }

        return StatusCode(StatusCodes.Status200OK, subjectInformationDtos);
    }

    [HttpGet]
    [Route("links")]
    [Authorize(AuthenticationSchemes = BotAuthenticationSchemeOptions.DefaultSchemeName)]
    public async Task<ActionResult<IEnumerable<LinkInformationDto>>> GetLinksInformationAsync(CancellationToken cancellationToken)
    {
        return await _context.Links.Select(x => _mapper.Map<LinkInformationDto>(x)).ToListAsync(cancellationToken);
    }
    

    private async Task<int> CalculateDaysAsync(Subject subject, int subjectTypeId, CancellationToken cancellationToken)
    {
        var couples = subject.Couples
            .Where(x => x.SubjectTypeId == subjectTypeId)
            .GroupBy(x => x.DayOfWeek)
            .Select(x => x.First());
        
        var days = new HashSet<DateTime>();

        foreach (var couple in couples)
        {
            if (couple.StartDate != null)
            {
                var startDate = couple.StartDate.Value.ToDateTime(new TimeOnly());
                var endDate = couple.EndDate!.Value.ToDateTime(new TimeOnly());
                
                for (var i = startDate; i <= endDate; i = i.AddDays(couple.IsRolling ? 14 : 7))
                {
                    if (i >= DateTime.Today)
                        days.Add(i);
                }
            }

            days.UnionWith(couple.CoupleDates
                .Where(x => !x.IsRemovedDate)
                .Select(x => x.Date.ToDateTime(new TimeOnly()))
            );

            days.RemoveWhere(x => couple.CoupleDates.Any(y => y.Date == DateOnly.FromDateTime(x)));
        }

        var additionalCouples = await _context.AdditionalCouples
            .Where(x => x.SubjectId == subject.Id && x.SubjectTypeId == subjectTypeId)
            .ToListAsync(cancellationToken);
        
        days.UnionWith(additionalCouples
            .Select(x => x.Date.ToDateTime(new TimeOnly()))
        );

        return days.Count;
    }
}