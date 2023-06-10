

namespace ZigZag.Test;

public sealed class MissingAppSettingException : Exception
{
    internal MissingAppSettingException(string appSettingName) : base($"Missing application setting. '{appSettingName}' setting has no value!")
    {

    }
}
