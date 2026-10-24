using System.Globalization;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Mapping;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using IpDeputyApi.Services.Schedule;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Services.Bot;

public class BotInformationService : IBotInformationService
{
    private static readonly int[] CountedTypes = [(int)SubjectTypes.Laboratory, (int)SubjectTypes.Practice, (int)SubjectTypes.Lecture];

    private readonly IRepository<Student> _students;
    private readonly IRepository<Teacher> _teachers;
    private readonly IRepository<Subject> _subjects;
    private readonly IRepository<Link> _links;
    private readonly ICoupleRepository _couples;
    private readonly IAdditionalCoupleRepository _additionalCouples;
    private readonly IAppClock _clock;

    public BotInformationService(
        IRepository<Student> students,
        IRepository<Teacher> teachers,
        IRepository<Subject> subjects,
        IRepository<Link> links,
        ICoupleRepository couples,
        IAdditionalCoupleRepository additionalCouples,
        IAppClock clock)
    {
        _students = students;
        _teachers = teachers;
        _subjects = subjects;
        _links = links;
        _couples = couples;
        _additionalCouples = additionalCouples;
        _clock = clock;
    }

    public async Task<IReadOnlyList<StudentInformationDto>> GetStudentsAsync(CancellationToken cancellationToken)
    {
        var students = await _students.Query()
            .Include(x => x.Subgroup)
            .OrderBy(x => x.Index)
            .ToListAsync(cancellationToken);

        return students.Select(x => new StudentInformationDto
        {
            Name = x.Name,
            Surname = x.Surname,
            Patronymic = x.Patronymic,
            Subgroup = x.Subgroup?.Name,
            ContactPhone = x.ContactPhone,
            TelegramPhone = x.TelegramPhone,
            Email = x.Email,
            FitEmail = x.FitEmail,
            TelegramNickname = x.TelegramNickname,
            Birthday = x.Birthday.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture)
        }).ToList();
    }

    public async Task<IReadOnlyList<TeacherInformationDto>> GetTeachersAsync(CancellationToken cancellationToken)
    {
        return await _teachers.Query()
            .OrderBy(x => x.Surname)
            .ThenBy(x => x.Name)
            .ProjectToInformationDto()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LinkInformationDto>> GetLinksAsync(CancellationToken cancellationToken)
    {
        return await _links.Query()
            .OrderBy(x => x.Name)
            .ProjectToInformationDto()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SubjectInformationDto>> GetSubjectsAsync(CancellationToken cancellationToken)
    {
        var today = _clock.Today;
        var horizon = today.AddYears(1);

        var subjects = await _subjects.Query().OrderBy(x => x.Name).ToListAsync(cancellationToken);
        var couples = await _couples.GetBySubjectTypesAsync(CountedTypes, cancellationToken);
        var additionalCouples = await _additionalCouples.Query()
            .Where(x => x.Date >= today && CountedTypes.Contains(x.SubjectTypeId))
            .ToListAsync(cancellationToken);

        int CountDays(int subjectId, SubjectTypes type)
        {
            var dates = new HashSet<DateOnly>();

            foreach (var couple in couples.Where(x => x.SubjectId == subjectId && x.SubjectTypeId == (int)type))
                dates.UnionWith(ScheduleCalculator.GetDates(couple, today, horizon));

            dates.UnionWith(additionalCouples
                .Where(x => x.SubjectId == subjectId && x.SubjectTypeId == (int)type)
                .Select(x => x.Date));

            return dates.Count;
        }

        return subjects.Select(subject => new SubjectInformationDto
        {
            Name = subject.Name,
            ShortName = subject.ShortName,
            LaboratoryCount = subject.LaboratoryCount,
            PracticalCount = subject.PracticalCount,
            LaboratoryDaysCount = CountDays(subject.Id, SubjectTypes.Laboratory),
            PracticalDaysCount = CountDays(subject.Id, SubjectTypes.Practice),
            LecturesDaysCount = CountDays(subject.Id, SubjectTypes.Lecture)
        }).ToList();
    }
}
