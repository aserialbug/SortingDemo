using System;
using generate.Infrastructure;

namespace generate.Application
{
    public class LineBuilder
    {
        private readonly WordSource _wordSource;
        private readonly Random _random;
        private readonly char[] _currentLine;

        public LineBuilder(WordSource wordSource, Options options)
        {
            _wordSource = wordSource;
            _random = new Random();
            _currentLine = new char[options.MaxLineLength];
        }

        public ReadOnlySpan<char> BuildLine()
        {
            var line = _currentLine.AsSpan();
            int position = 0;
            _random.Next().TryFormat(line, out var written);
            position += written;
            line[position ] = '.';
            line[position + 1] = ' ';
            position += 2;
            var word = _wordSource.GetNextRandomWord().AsSpan();
            word.CopyTo(line.Slice(position, word.Length));
            position += word.Length;
            line[position] = '\r';
            line[position + 1] = '\n';
            position += 2;
            
            return (new ReadOnlySpan<char>(_currentLine)).Slice(0, position);
        }
    }
}