using System;
using System.IO;

namespace sort.Application
{
    public class LineTransformer
    {
        private readonly char[] _string = new char[1024];

        public ReadOnlySpan<char> FromOriginal(string source)
        {
            var split = GetSplitIndex(source);
            if (!split.HasValue)
            {
                throw new InvalidDataException($"Line '{source}' is not in '<NUmber>. <String>' format");
            }

            var stringPartLength = source.Length - split.Value - 2;
            source.CopyTo(split.Value + 2, _string, 0, stringPartLength);
            _string[stringPartLength] = '.';
            source.CopyTo(0, _string, stringPartLength + 1, split.Value);
            
            return (new ReadOnlySpan<char>(_string))
                .Slice(0, stringPartLength + split.Value + 1);
        }

        public ReadOnlySpan<char> ToOriginal(string source)
        {
            var split = GetSplitIndex(source);
            if (!split.HasValue)
            {
                throw new InvalidDataException($"Line '{source}' is not in '<NUmber>. <String>' format");
            }
            
            var numberPartLength = source.Length - split.Value - 1;
            source.CopyTo(split.Value + 1, _string, 0, numberPartLength);
            _string[numberPartLength] = '.';
            _string[numberPartLength + 1] = ' ';
            source.CopyTo(0, _string, numberPartLength + 2, split.Value);
            
            return (new ReadOnlySpan<char>(_string))
                .Slice(0, numberPartLength + split.Value + 2);
        }

        private int? GetSplitIndex(string str)
        {
            int? result = null;
            for (int idx = 0; idx < str.Length; idx++)
            {
                if (str[idx] == '.')
                {
                    result = idx;
                    break;
                }
            }

            return result;
        }
    }
}