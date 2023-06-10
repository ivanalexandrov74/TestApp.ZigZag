using Microsoft.AspNetCore.Components;

namespace ZigZag.Test.Pages;

public partial class ApplicationErrorPopUp
{

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        appData.applicationErrorPopUpObject = this;
    }

    private bool IsVisible => !string.IsNullOrWhiteSpace(appData.errorMessage);

    private async Task OnErrorMessage_Click()
    {
        appData.errorMessage = string.Empty;

        await InvokeAsync(StateHasChanged);
    }
}
