

namespace ZigZag.Test.Dto;

public enum UserLoginResultEnum
{
    Success = 0,
    [EnumResultDescription("Invalid user name!")]
    Invalid_user_name = -1,
    [EnumResultDescription("Invalid user password!")]
    Invalid_user_password = -2,
    [EnumResultDescription("This user account waits for system administrator authorisation!")]
    User_waits_admin_confirmation = -3,
    [EnumResultDescription("This user account waits for user email confirmation! Please check your email and click on user email confirmation link.")]
    User_waits_user_email_confirmation = -4,
    [EnumResultDescription("This user account is disabled! Please, contact system administrator to enable your account.")]
    Disabled_user = -5,
}

public class UserLoginResponseDto : BaseResponseDto<UserLoginResultEnum>
{
    public Guid accessTokenUid { get; set; } = Guid.Empty;
}
