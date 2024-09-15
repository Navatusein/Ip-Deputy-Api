using AutoMapper;
using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Dto.Frontend;

using DayOfWeek = IpDeputyApi.Database.Models.DayOfWeek;

namespace IpDeputyApi.Service
{
    public class AppMappingService : Profile
    {
        public AppMappingService() 
        {
            // Frontend mapping
            CreateMap<AdditionalCouple, AdditionalCoupleDto>().ReverseMap();
            CreateMap<Couple, CoupleDto>().ReverseMap();
            CreateMap<CoupleDate, CoupleDateDto>().ReverseMap();
            CreateMap<CoupleTime, CoupleTimeDto>().ReverseMap();
            CreateMap<DayOfWeek, DayOfWeekDto>().ReverseMap();
            CreateMap<Link, LinkDto>().ReverseMap();
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Subgroup, SubgroupDto>().ReverseMap();
            CreateMap<Subject, SubjectDto>().ReverseMap();
            CreateMap<SubjectType, SubjectTypeDto>().ReverseMap();
            CreateMap<SubmissionsConfig, SubmissionsConfigDto>()
                .ForMember(x => x.SubmissionWorks, opt => opt.Ignore())
                .ForMember(x => x.SubmissionStudents, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.SubmissionWorks, opt => opt.Ignore())
                .ForMember(x => x.SubmissionStudents, opt => opt.Ignore());
            CreateMap<SubmissionStudent, SubmissionStudentDto>().ReverseMap();
            CreateMap<SubmissionWork, SubmissionWorkDto>().ReverseMap();
            CreateMap<Teacher, TeacherDto>().ReverseMap();
            CreateMap<Telegram, TelegramDto>().ReverseMap();
     

            // Bot mapping
            CreateMap<Telegram, StudentSettingsDto>()
                .ReverseMap()
                .ForMember(x => x.Id, opt => opt.MapFrom(src => 0));

            CreateMap<Student, StudentInformationDto>()
                .ForMember(x => x.Subgroup, opt => opt.MapFrom(src => src.Subgroup!.Name));

            CreateMap<Teacher, TeacherInformationDto>();

            CreateMap<Link, LinkInformationDto>();
        }
    }
}
