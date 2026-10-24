using IpDeputyApi.Database;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Repositories;

public class SubmissionsConfigRepository : RepositoryBase<SubmissionsConfig>, ISubmissionsConfigRepository
{
    public SubmissionsConfigRepository(IpDeputyDbContext context) : base(context)
    {
    }

    public Task<SubmissionsConfig?> GetWithWorksAsync(int id, CancellationToken cancellationToken)
    {
        return Set.Include(x => x.SubmissionWorks).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<SubmissionsConfig>> GetWithSubmissionsAsync(CancellationToken cancellationToken)
    {
        return Set.AsNoTracking()
            .Include(x => x.Subject)
            .Include(x => x.SubjectType)
            .Include(x => x.Subgroup)
            .Include(x => x.SubmissionStudents).ThenInclude(x => x.SubmissionWork)
            .Include(x => x.SubmissionStudents).ThenInclude(x => x.Student)
            .OrderBy(x => x.Id)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }
}
