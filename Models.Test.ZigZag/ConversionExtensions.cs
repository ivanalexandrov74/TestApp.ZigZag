using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZigZag.Test
{
    public static class ConversionExtensions
    {
        public static string GetExcetionFullText(this Exception? exception)
        {
            if (Environment.GetEnvironmentVariable(     "ASPNETCORE_ENVIRONMENT")==  "Development")
            {
                if (exception == null) return string.Empty;
                else return $"{exception}{exception.InnerException.GetExcetionFullText()}";
            }
            else
                return string.Empty;
        }
    }
}
