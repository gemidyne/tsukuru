using System;

namespace Tsukuru.Core.SourceEngine
{
    public static class VProjectHelper
    {
        private static string _path;

        public static string Path
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_path))
                {
                    _path = Environment.GetEnvironmentVariable("VPROJECT", EnvironmentVariableTarget.User) ?? Environment.GetEnvironmentVariable("VPROJECT");
                }

                return _path;
            }
        }
    }
}