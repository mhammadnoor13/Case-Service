using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class StringExtensions
    {
        public static string Truncate(this string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || maxLength < 0)
                return value ?? string.Empty;

            return value.Length <= maxLength
                ? value
                : value.Substring(0, maxLength) + "...";
        }
    }
}
