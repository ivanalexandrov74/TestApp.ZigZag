using ZigZag.Test.Dto;

namespace ZigZag.Test.Pages;

public partial class LoginPage
{

    private async Task OnLoginButton_Click()
    {
        if (!state.userName.IsValidUserName())
            await ShowErrorMessage("User name field does not met requirements - minimum 5 symbols!");
        else if (!state.password.IsValidPassword())
                await ShowErrorMessage("Password field does not met requirements - minimum 5 symbols!");
        else
            await CallWebApiAsync<UserLoginResponseDto>("api/UserLogin", HttpMethodEnum.Post, state, OnLoginSuccess);
    }

    private void OnLoginSuccess(UserLoginResponseDto responseData)
    {
        appData.accessTokenUid = responseData.accessTokenUid;

        ShowPage(typeof(MainPage));
    }


}
