using IpDeputyApi.Repositories;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services;
using IpDeputyApi.Services.Bot;
using IpDeputyApi.Services.Interfaces;

namespace IpDeputyApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICoupleRepository, CoupleRepository>();
        services.AddScoped<IAdditionalCoupleRepository, AdditionalCoupleRepository>();
        services.AddScoped<ITelegramRepository, TelegramRepository>();
        services.AddScoped<ISubmissionsConfigRepository, SubmissionsConfigRepository>();

        return services;
    }

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IAppClock, AppClock>();

        services.AddScoped<ISubgroupService, SubgroupService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<ISubjectTypeService, SubjectTypeService>();
        services.AddScoped<ILinkService, LinkService>();
        services.AddScoped<ITelegramService, TelegramService>();
        services.AddScoped<ICoupleService, CoupleService>();
        services.AddScoped<IAdditionalCoupleService, AdditionalCoupleService>();
        services.AddScoped<ISubmissionsConfigService, SubmissionsConfigService>();
        services.AddScoped<ISubmissionStudentService, SubmissionStudentService>();
        services.AddScoped<ILookupService, LookupService>();

        services.AddScoped<IBotScheduleService, BotScheduleService>();
        services.AddScoped<IBotInformationService, BotInformationService>();
        services.AddScoped<IBotStudentService, BotStudentService>();
        services.AddScoped<IBotSubmissionService, BotSubmissionService>();

        return services;
    }
}
