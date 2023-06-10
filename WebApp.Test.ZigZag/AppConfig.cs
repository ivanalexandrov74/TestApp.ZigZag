namespace ZigZag.Test;

internal sealed class AppConfig : BaseConfigMgr
{
    public AppConfig(IConfigurationRoot appConfiguration) : base(appConfiguration) { }


    public string webApiUrl => Get<string>("WebApiUrl", false);
}
