using Microsoft.AspNetCore.Components;
using ZigZag.Test.Pages;

namespace ZigZag.Test;

public class AppData
{
    public Guid applicationSessionUId { get; }= Guid.NewGuid();

    public Guid accessTokenUid { get; set; }

    public Type currentPage { get; set; } = typeof(LoginPage);

    public string errorMessage { get; set; } = string.Empty;

    public IndexPage homePage { get; set; } = null!;

    public ApplicationErrorPopUp applicationErrorPopUpObject { get; set; } = null!;
}
