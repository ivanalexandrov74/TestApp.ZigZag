

namespace ZigZag.Test.Dto;

public enum ExternalApisResultEnum
{
    Success=0,
    [EnumResultDescription("Error calling external API to get specific category APIs!")]
    External_Api_Error =1,
}

public class ExternalApisResponseDto:BaseResponseDto<ExternalApisResultEnum>
{
    public List<ExternalApiNodeDto> externalApis { get; set; } = new();

    public PageInfoGraphQlDto pageInfo { get; set; } = new();
}
