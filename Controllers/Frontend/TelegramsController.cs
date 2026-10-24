using IpDeputyApi.Authorization;
using IpDeputyApi.Dto.Frontend;
using IpDeputyApi.Paging;
using IpDeputyApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IpDeputyApi.Controllers.Frontend;

[Tags("Frontend:Telegrams Controller")]
[Route("frontend/telegram")]
[RequirePermission(Permissions.ManageStudents)]
public class TelegramsController : CrudControllerBase<TelegramDto, PageRequest>
{
    public TelegramsController(ITelegramService service) : base(service)
    {
    }
}
