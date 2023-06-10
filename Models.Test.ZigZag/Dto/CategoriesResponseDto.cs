

namespace ZigZag.Test.Dto;

public enum CategoriesResultEnum
{
    Success=0,
    [EnumResultDescription("Error calling external API to get API categories!")]
    External_Api_Error =1
}
public class CategoriesResponseDto:BaseResponseDto<CategoriesResultEnum>
{
    public List<string> categories { get; set; } = new();
}
