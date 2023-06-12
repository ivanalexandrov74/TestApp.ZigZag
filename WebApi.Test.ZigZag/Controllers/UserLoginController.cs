using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ZigZag.Test.Data;
using ZigZag.Test.Dto;

namespace ZigZag.Test.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserLoginController : BaseController
{
    private readonly AuthorisationMgr _authorisationMgr;

    public UserLoginController(ApiConfig config, Db db,AuthorisationMgr authorisationMgr) : base(config, db) 
    {
        _authorisationMgr = authorisationMgr;
    }


    [HttpPost]
    public async Task<ActionResult<UserLoginResponseDto>> Post(UserLoginRequestDto requestData)
    {
        var userDbo = await db.Users.Find(item => item.UserName.ToUpper() == requestData.userName.ToUpper()).FirstOrDefaultAsync();

        if (userDbo == null)
            return FailedResult<UserLoginResultEnum, UserLoginResponseDto>(UserLoginResultEnum.Invalid_user_name);
        else if (userDbo.UserPasswordSha != requestData.password.ToSha256String())
            return FailedResult<UserLoginResultEnum, UserLoginResponseDto>(UserLoginResultEnum.Invalid_user_password);
        else 
        {
            var accessTokenTimeoutUserSetting = new TimeSpan(12, 0, 0);// new UserSettingMgr(dbContext).Get<TimeSpan>( app_UserDto.FileId, UserSettingNameEnum.accessTokenTimeout);

            var accessToken = new AccessTokenDbo
            {
                AccessTokenUid = Guid.NewGuid(),
                ApplicationSessionUid = this.applicationSessionUid,
                UserUid=userDbo.UserUid,
                CreatedOnUtc = DateTime.UtcNow,
                ValidToUtc = DateTime.UtcNow.Add(accessTokenTimeoutUserSetting)
            };

            await db.AccessTokens.InsertOneAsync(accessToken);

            _authorisationMgr.PushInCache(accessToken);

            return Ok(new UserLoginResponseDto()
            {
                accessTokenUid = accessToken.AccessTokenUid,
                errorMessage = string.Empty,
            });
        }
    }
}
