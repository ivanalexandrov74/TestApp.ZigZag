

namespace ZigZag.Test;


public sealed class InvalidAppSettingException : Exception
{
    internal InvalidAppSettingException(string appSettingName, string appSettingTextValue, string appSettingTypeName)
        : base($"Invalid application setting value. Unable to cast to '{appSettingTypeName}' '{appSettingName}' setting value '{appSettingTextValue}'")
    {

    }

    internal InvalidAppSettingException(string appSettingName, string appSettingTextValue, string appSettingTypeName, Exception exception)
    : base($"Invalid application setting value. Unable to cast to '{appSettingTypeName}' '{appSettingName}' setting value '{appSettingTextValue}'", exception)
    {

    }
}
