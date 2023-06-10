using ZigZag.Test.Dto;

namespace ZigZag.Test.Data;

public class ExternalApiResponseDto
{
    public int count { get; set; }
    public List<ExternalApiDto> entries { get; set; } = new();
}
