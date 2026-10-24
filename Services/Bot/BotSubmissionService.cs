using System.Globalization;
using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;

namespace IpDeputyApi.Services.Bot;

public class BotSubmissionService : IBotSubmissionService
{
    private readonly ISubmissionsConfigRepository _configs;

    public BotSubmissionService(ISubmissionsConfigRepository configs)
    {
        _configs = configs;
    }

    public async Task<IReadOnlyList<SubmissionsConfigDataDto>> GetSubmissionsConfigsAsync(CancellationToken cancellationToken)
    {
        var configs = await _configs.GetWithSubmissionsAsync(cancellationToken);

        return configs.Select(config =>
        {
            var firstSubmission = config.SubmissionStudents.OrderBy(x => x.Id).FirstOrDefault();

            return new SubmissionsConfigDataDto
            {
                Id = config.Id,
                Name = config.CustomName ?? config.Subject?.ShortName ?? "",
                Type = config.CustomType ?? config.SubjectType?.ShortName ?? "",
                Subgroup = config.Subgroup?.Name,
                ClearedAt = firstSubmission == null
                    ? null
                    : firstSubmission.SubmittedAt.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                Submissions = config.SubmissionStudents
                    .OrderBy(x => x.PreferredPosition)
                    .ThenBy(x => x.Id)
                    .GroupBy(x => x.StudentId)
                    .SelectMany(x => x.OrderBy(s => s.SubmissionWork.Index).ThenBy(s => s.Id))
                    .Select(x => new SubmissionDto
                    {
                        Name = x.SubmissionWork.Name,
                        Student = $"{x.Student.Surname} {x.Student.Name}"
                    })
                    .ToList()
            };
        }).ToList();
    }
}
