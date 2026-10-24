using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Paging;

namespace IpDeputyApi.Services.Interfaces;

public interface ISubgroupService : ICrudService<SubgroupDto, PageRequest>;

public interface IStudentService : ICrudService<StudentDto, StudentFilter>;

public interface ITeacherService : ICrudService<TeacherDto, PageRequest>;

public interface ISubjectService : ICrudService<SubjectDto, PageRequest>;

public interface ISubjectTypeService : ICrudService<SubjectTypeDto, PageRequest>;

public interface ILinkService : ICrudService<LinkDto, PageRequest>;

public interface ITelegramService : ICrudService<TelegramDto, PageRequest>;

public interface ICoupleService : ICrudService<CoupleDto, CoupleFilter>;

public interface IAdditionalCoupleService : ICrudService<AdditionalCoupleDto, AdditionalCoupleFilter>;

public interface ISubmissionsConfigService : ICrudService<SubmissionsConfigDto, SubmissionsConfigFilter>
{
    Task<IReadOnlyList<SubmissionsConfigDto>> GetForStudentAsync(int studentId, bool otherSubgroups, CancellationToken cancellationToken);
}

public interface ISubmissionStudentService : ICrudService<SubmissionStudentDto, SubmissionStudentFilter>
{
    Task<IReadOnlyList<SubmissionStudentDto>> GetForStudentAsync(int studentId, CancellationToken cancellationToken);
    Task<IReadOnlyList<SubmissionStudentDto>> CreateForStudentAsync(int studentId, MySubmissionCreateDto dto, bool enforceSubgroup, CancellationToken cancellationToken);
    Task DeleteForStudentAsync(int studentId, IReadOnlyCollection<int> ids, CancellationToken cancellationToken);
    Task UpdatePreferredPositionAsync(int studentId, PreferredPositionDto dto, CancellationToken cancellationToken);
}

public interface ILookupService
{
    Task<IReadOnlyList<CoupleTimeDto>> GetCoupleTimesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<DayOfWeekDto>> GetDaysOfWeekAsync(CancellationToken cancellationToken);
}
