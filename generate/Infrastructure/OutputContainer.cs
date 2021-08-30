using System;
using System.IO;
using System.Text;

namespace generate.Infrastructure
{
    public class OutputContainer : IDisposable
    {
        private readonly StreamWriter _streamWriter;
        private readonly StringBuilder _cache;
        private const int CACHE_SIZE = 104857600; // 100 Mb

        public long Length => _streamWriter.BaseStream.Length + _cache.Length;

        public OutputContainer(string fileName)
        {
            _streamWriter = new StreamWriter(fileName);
            _streamWriter.AutoFlush = false;
            _cache = new StringBuilder(CACHE_SIZE, CACHE_SIZE);
        }

        public void AppendLine(ReadOnlySpan<char> line)
        {
            _streamWriter.Write(line);
        }

        public void Dispose()
        {
            _streamWriter?.Dispose();
        }
    }
}