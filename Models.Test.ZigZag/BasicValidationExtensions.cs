

namespace ZigZag.Test;

public static class BasicValidationExtensions
{
    public static bool IsValidUserName(this string? value) => !string.IsNullOrWhiteSpace(value) && value.Length>4;

    public static bool IsValidPassword(this string? value) => !string.IsNullOrWhiteSpace(value) && value.Length > 4;
}
