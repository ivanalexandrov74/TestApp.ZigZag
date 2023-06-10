using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Newtonsoft.Json.Linq;


namespace ZigZag.Test;

public abstract class BaseConfigMgr
{
    private readonly IConfiguration _configurationManager;

    private readonly IConfigurationSection _appSettingsSection;

    private readonly JToken _jsonConfig;

    public BaseConfigMgr(IConfiguration appConfiguration)
    {
        _configurationManager = appConfiguration;
        _jsonConfig = JToken.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json")));
        _appSettingsSection = appConfiguration.GetSection("AppSettings");
    }

    protected TResult Get<TResult>(string appSettingName, bool isConnectionString = false)
    {
        if (string.IsNullOrWhiteSpace(appSettingName))
        {
            throw new ArgumentNullException("appSettingName");
        }

        string text = (isConnectionString ? _configurationManager.GetConnectionString(appSettingName) : _appSettingsSection[appSettingName]);

        if (text == null)
        {
            throw new MissingAppSettingException(appSettingName);
        }

        try
        {
            object obj = ((typeof(TResult) == typeof(string)) ? text : ((object)JsonSerializer.Deserialize<TResult>(text)));

            if (obj != null)
            {
                return (TResult)obj;
            }
        }
        catch (Exception exception)
        {
            throw new InvalidAppSettingException(appSettingName, text, typeof(TResult)!.FullName ?? string.Empty, exception);
        }

        throw new InvalidAppSettingException(appSettingName, text, typeof(TResult)!.FullName ?? string.Empty);
    }

    protected TResult Get<TResult>(string appSettingName, TResult defaultValue)
    {
        if (string.IsNullOrWhiteSpace(appSettingName))
        {
            throw new ArgumentNullException("appSettingName");
        }

        var text = _appSettingsSection[appSettingName];
        if (text == null)
        {
            return defaultValue;
        }

        try
        {
            object obj = JsonSerializer.Deserialize<TResult>(text);
            if (obj != null)
            {
                return (TResult)obj;
            }
        }
        catch (Exception exception)
        {
            throw new InvalidAppSettingException(appSettingName, text, typeof(TResult)!.FullName ?? string.Empty, exception);
        }

        throw new InvalidAppSettingException(appSettingName, text, typeof(TResult)!.FullName ?? string.Empty);
    }

    protected TResult GetJson<TResult>(string appSettingName)
    {
        if (string.IsNullOrWhiteSpace(appSettingName))
        {
            throw new ArgumentNullException("appSettingName");
        }

        return _jsonConfig["AppSettings"]![appSettingName]!.ToObject<TResult>();
    }
}
