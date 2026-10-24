using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Dto.Frontend;
using Riok.Mapperly.Abstractions;
using DayOfWeek = IpDeputyApi.Database.Models.DayOfWeek;

namespace IpDeputyApi.Mapping;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class AppMapper
{
    public static partial IQueryable<SubgroupDto> ProjectToDto(this IQueryable<Subgroup> query);

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    [MapperIgnoreSource(nameof(SubgroupDto.Id))]
    public static partial void ApplyTo(this SubgroupDto dto, [MappingTarget] Subgroup entity);

    public static partial IQueryable<StudentDto> ProjectToDto(this IQueryable<Student> query);

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    [MapperIgnoreSource(nameof(StudentDto.Id))]
    public static partial void ApplyTo(this StudentDto dto, [MappingTarget] Student entity);

    public static partial IQueryable<TeacherDto> ProjectToDto(this IQueryable<Teacher> query);

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    [MapperIgnoreSource(nameof(TeacherDto.Id))]
    public static partial void ApplyTo(this TeacherDto dto, [MappingTarget] Teacher entity);

    public static partial IQueryable<SubjectDto> ProjectToDto(this IQueryable<Subject> query);

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    [MapperIgnoreSource(nameof(SubjectDto.Id))]
    public static partial void ApplyTo(this SubjectDto dto, [MappingTarget] Subject entity);

    public static partial IQueryable<SubjectTypeDto> ProjectToDto(this IQueryable<SubjectType> query);

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    [MapperIgnoreSource(nameof(SubjectTypeDto.Id))]
    public static partial void ApplyTo(this SubjectTypeDto dto, [MappingTarget] SubjectType entity);

    public static partial IQueryable<LinkDto> ProjectToDto(this IQueryable<Link> query);

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    [MapperIgnoreSource(nameof(LinkDto.Id))]
    public static partial void ApplyTo(this LinkDto dto, [MappingTarget] Link entity);

    public static partial IQueryable<CoupleTimeDto> ProjectToDto(this IQueryable<CoupleTime> query);

    public static partial IQueryable<DayOfWeekDto> ProjectToDto(this IQueryable<DayOfWeek> query);

    public static partial IQueryable<AdditionalCoupleDto> ProjectToDto(this IQueryable<AdditionalCouple> query);

    [MapperRequiredMapping(RequiredMappingStrategy.Source)]
    [MapperIgnoreSource(nameof(AdditionalCoupleDto.Id))]
    [MapperIgnoreSource(nameof(AdditionalCoupleDto.DayOfWeekId))]
    public static partial void ApplyTo(this AdditionalCoupleDto dto, [MappingTarget] AdditionalCouple entity);

    public static partial IQueryable<SubmissionStudentDto> ProjectToDto(this IQueryable<SubmissionStudent> query);

    public static partial SubmissionStudentDto ToDto(this SubmissionStudent entity);

    public static partial IQueryable<TeacherInformationDto> ProjectToInformationDto(this IQueryable<Teacher> query);

    public static partial IQueryable<LinkInformationDto> ProjectToInformationDto(this IQueryable<Link> query);

    public static partial StudentSettingsDto ToSettingsDto(this Telegram entity);
}
