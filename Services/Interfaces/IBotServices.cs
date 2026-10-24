using IpDeputyApi.Dto.Bot;

namespace IpDeputyApi.Services.Interfaces;

public interface IBotScheduleService
{
    Task<ScheduleDayDto> GetDayAsync(ulong telegramId, DateOnly date, CancellationToken cancellationToken);
    Task<ScheduleWeekDto> GetWeekAsync(ulong telegramId, DateOnly date, CancellationToken cancellationToken);
}

public interface IBotInformationService
{
    Task<IReadOnlyList<StudentInformationDto>> GetStudentsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<TeacherInformationDto>> GetTeachersAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<SubjectInformationDto>> GetSubjectsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<LinkInformationDto>> GetLinksAsync(CancellationToken cancellationToken);
}

public interface IBotStudentService
{
    Task AuthorizeAsync(StudentContactDto contactDto, CancellationToken cancellationToken);
    Task<StudentSettingsDto> GetSettingsAsync(ulong telegramId, CancellationToken cancellationToken);
    Task<StudentSettingsDto> UpdateSettingsAsync(StudentSettingsDto settingsDto, CancellationToken cancellationToken);
    Task UpdateLastActivityAsync(ulong telegramId, CancellationToken cancellationToken);
}

public interface IBotSubmissionService
{
    Task<IReadOnlyList<SubmissionsConfigDataDto>> GetSubmissionsConfigsAsync(CancellationToken cancellationToken);
}
