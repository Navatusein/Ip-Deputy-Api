using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IpDeputyApi.Dtos
{
    public class ErrorDto
    {
        public ErrorDto(string message, string code)
        {
            Id = Guid.NewGuid().ToString();
            Message = message;
            Code = code;
        }

        public ErrorDto()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string Code { get; set; } = null!;

        public object? Data { get; set; }
    }
}

