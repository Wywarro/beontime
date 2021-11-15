using System.Text.Json;

namespace Beontime.Application.Common
{

    public sealed class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public string Content { get; set; } = "";

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
