using System;
using System.Linq;
using Chiaki;

namespace Tsukuru.SourcePawn
{
    public static class CompilationMessageParser
    {
        public static CompilationMessage ParseFromString(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return null;
            }

            if (!IsLineErrorOrWarning(line))
            {
                return new CompilationMessage
                {
                    Message = line.Trim(),
                    RawLine = line
                };
            }

            // FILENAME(FIRSTLINE -- LASTLINE) : PREFIX NUMBER: TEXT
            // or
            // FILENAME(LASTLINE) : PREFIX NUMBER: TEXT

            int lineInfoStartIdx = line.IndexOf("(", StringComparison.InvariantCultureIgnoreCase);

            if (lineInfoStartIdx == -1)
            {
                return null;
            }

            int lineInfoEndIdx = line.IndexOf(")", lineInfoStartIdx, StringComparison.InvariantCultureIgnoreCase);

            if (lineInfoEndIdx == -1)
            {
                return null;
            }

            int prefixStartIdx = line.IndexOf(" : ", lineInfoEndIdx + 1, StringComparison.InvariantCultureIgnoreCase) + " : ".Length;
            int prefixEndIdx = line.IndexOf(":", prefixStartIdx + 1, StringComparison.InvariantCultureIgnoreCase);

            var message = new CompilationMessage
            {
                RawLine = line,
                FileName = line.Substring(0, lineInfoStartIdx)
            };

            // Try to get line numbers
            string buffer = line.Substring(
                startIndex: lineInfoStartIdx + 1, 
                length: lineInfoEndIdx - lineInfoStartIdx - 1);

            const string multipleLineSeparator = "--";

            if (buffer.Contains(multipleLineSeparator))
            {
                int? firstLineIdx = buffer.Substring(
                    startIndex: 0,
                    length: buffer.IndexOf(multipleLineSeparator, StringComparison.InvariantCultureIgnoreCase) - 1)
                        .Trim()
                        .TryParseInt32();

                int? lastLineIdx = buffer.Substring(
                    startIndex: buffer.IndexOf(multipleLineSeparator, StringComparison.InvariantCultureIgnoreCase) + 1,
                    length: buffer.Length - buffer.IndexOf(multipleLineSeparator, StringComparison.InvariantCultureIgnoreCase))
                    .TryParseInt32();

                message.FirstLine = firstLineIdx;
                message.LastLine = lastLineIdx.GetValueOrDefault();
            }
            else
            {
                int? lastLineIdx = buffer.TryParseInt32();

                message.FirstLine = null;
                message.LastLine = lastLineIdx.GetValueOrDefault();
            }

            message.Prefix = line.Substring(prefixStartIdx, (prefixEndIdx - prefixStartIdx));
            message.Message = line.Substring(prefixEndIdx + 1);

            return message;
        }

        public static bool IsLineErrorOrWarning(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            string[] prefixes =
            {
                "error",
                "fatal error",
                "warning"
            };

            return prefixes.Any(prefix => line.ToLower().Contains(prefix));
        }

        public static bool IsLineError(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            string[] prefixes =
            {
                "error",
                "fatal error"
            };

            return prefixes.Any(prefix => line.ToLower().Contains(prefix));
        }

        public static bool IsLineWarning(string line)
        {
            return !string.IsNullOrWhiteSpace(line)
                && line.ToLower().Contains("warning");
        }
    }

    public enum ECompilationMessageType
    {
        Standard = 0,
        Warning = 1,
        Error = 2,
        FatalError = 3
    }
}
