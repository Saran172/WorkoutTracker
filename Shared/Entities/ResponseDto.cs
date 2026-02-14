namespace Shared.Entities
{
    public class ResponseDto
    {
        public string? RespType { get; set; }
        public string? RespCode {  get; set; }
        public string? RespMessage { get; set; }
        public object? RespData { get; set; }
    }
}
