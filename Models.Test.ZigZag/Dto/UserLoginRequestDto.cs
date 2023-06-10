
namespace ZigZag.Test.Dto;

public class UserLoginRequestDto : BaseRequestDto
{
    public string userName { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
}
