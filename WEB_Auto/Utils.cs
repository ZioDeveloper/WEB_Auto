using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEB_Auto
{
    public static class Utils
    {
        public static string Right(this string str, int length)
        {
            str = (str ?? string.Empty);
            return (str.Length >= length)
                ? str.Substring(str.Length - length, length)
                : str;
        }

        public static string Left(this string str, int length)
        {
            str = (str ?? string.Empty);
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static string DQuotedStr(this string aString)
        {
            return "\"" + aString + "\"";
        }

        public static string QuotedStr(this string aString)
        {
            return "'" + aString + "'";
        }
    }
}