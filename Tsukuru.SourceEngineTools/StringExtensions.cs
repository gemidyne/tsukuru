using System;

namespace Tsukuru.SourceEngineTools
{
    public static class StringExtensions
    {
        public static string Reverse(this string str)
        {
            var chars = str.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }
    }
}
