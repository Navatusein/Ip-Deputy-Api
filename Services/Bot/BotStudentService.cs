using IpDeputyApi.Database.Models;
using IpDeputyApi.Dto.Bot;
using IpDeputyApi.Exceptions;
using IpDeputyApi.Mapping;
using IpDeputyApi.Repositories.Interfaces;
using IpDeputyApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IpDeputyApi.Services.Bot;

public class BotStudentService : IBotStudentService
{
    private readonly ITelegramRepository _telegrams;
    private readonly IRepository<Student> _students;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppClock _clock;

    public BotStudentService(ITelegramRepository telegrams, IRepository<Student> students, IUnitOfWork unitOfWork, IAppClock clock)
    {
        _telegrams = telegrams;
        _students = students;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task AuthorizeAsync(StudentContactDto contactDto, CancellationToken cancellationToken)
    {
        var student = await FindStudentByPhoneAsync(contactDto.Phone, cancellationToken)
            ?? throw new NotFoundException("Студента з таким номером телефону не знайдено");

        var byTelegram = await _telegrams.GetByTelegramIdAsync(contactDto.TelegramId, cancellationToken);

        if (byTelegram != null)
        {
            if (byTelegram.StudentId == student.Id)
                return;

            throw new ConflictException("Цей Telegram акаунт уже прив'язаний до іншого студента");
        }

        if (await _telegrams.GetByStudentIdAsync(student.Id, cancellationToken) != null)
            throw new ConflictException("Студент уже прив'язаний до іншого Telegram акаунта");

        await _telegrams.AddAsync(new Telegram
        {
            StudentId = student.Id,
            TelegramId = contactDto.TelegramId,
            Language = TelegramService.DefaultLanguage,
            ScheduleCompact = false,
            LastActivity = _clock.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<StudentSettingsDto> GetSettingsAsync(ulong telegramId, CancellationToken cancellationToken)
    {
        var telegram = await GetTelegramAsync(telegramId, cancellationToken);
        return telegram.ToSettingsDto();
    }

    public async Task<StudentSettingsDto> UpdateSettingsAsync(StudentSettingsDto settingsDto, CancellationToken cancellationToken)
    {
        var language = settingsDto.Language?.Trim().ToLowerInvariant() ?? "";

        if (!TelegramService.Languages.Contains(language))
            throw new ValidationException("language", ValidationMessages.InvalidValue);

        var telegram = await GetTelegramAsync(settingsDto.TelegramId, cancellationToken);

        telegram.Language = language;
        telegram.ScheduleCompact = settingsDto.ScheduleCompact;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return telegram.ToSettingsDto();
    }

    public async Task UpdateLastActivityAsync(ulong telegramId, CancellationToken cancellationToken)
    {
        var telegram = await GetTelegramAsync(telegramId, cancellationToken);

        telegram.LastActivity = _clock.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Telegram> GetTelegramAsync(ulong telegramId, CancellationToken cancellationToken)
    {
        return await _telegrams.GetByTelegramIdAsync(telegramId, cancellationToken)
            ?? throw new NotFoundException("Студента не знайдено");
    }

    private async Task<Student?> FindStudentByPhoneAsync(string phone, CancellationToken cancellationToken)
    {
        var trimmed = phone.Trim();

        var exact = await _students.Query().FirstOrDefaultAsync(x => x.TelegramPhone == trimmed, cancellationToken);

        if (exact != null)
            return exact;

        var digits = OnlyDigits(trimmed);

        if (digits.Length == 0)
            return null;

        var students = await _students.Query().ToListAsync(cancellationToken);

        return students.FirstOrDefault(x => OnlyDigits(x.TelegramPhone) == digits);
    }

    private static string OnlyDigits(string value)
    {
        return new string(value.Where(char.IsDigit).ToArray());
    }
}
