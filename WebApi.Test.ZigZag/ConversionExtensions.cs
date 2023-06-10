using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Intrinsics.Arm;

namespace ZigZag.Test;

internal static class ConversionExtensions
{
    public static string ToSha256String (this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        else
        {
            using(var hashEngine=SHA256.Create())
            {
                var hashBytes = hashEngine.ComputeHash(Encoding.UTF8.GetBytes(value));

                var resultBuilder=new StringBuilder();

                foreach (var byteItem in hashBytes)
                    resultBuilder.Append($"{byteItem:X2}");

                return resultBuilder.ToString();
            }
        }
    }
}
