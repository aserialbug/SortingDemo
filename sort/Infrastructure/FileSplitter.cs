using System;
using System.Collections.Generic;
using System.IO;
using sort.Application;

namespace sort.Infrastructure
{
    public class FileSplitter
    {
        private readonly Options _options;
        private readonly LineTransformer _lineTransformer;

        public FileSplitter(LineTransformer lineTransformer, Options options)
        {
            _lineTransformer = lineTransformer;
            _options = options;
        }

        public Storage BuildTemporaryStorage()
        {
            return new Storage(_options.TemporaryDirectory);
        }

        public void ProcessLine(string line, Storage storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException(
                    nameof(storage),
                    "Cannot process line without temporary storage");
            }
            
            var reversed = _lineTransformer.FromOriginal(line);
            storage.Put(reversed);
        }

        public class Storage : IDisposable
        {
            private readonly string _directory;
            private readonly Dictionary<char, StreamWriter> _files;
            private readonly Dictionary<char, string> _fileNamesMap;

            public Storage(string directory)
            {
                _directory = directory;
                _files = new Dictionary<char, StreamWriter>();
                _fileNamesMap = new Dictionary<char, string>();
                
                if (!string.IsNullOrWhiteSpace(_directory))
                {
                    if (!Directory.Exists(_directory))
                        Directory.CreateDirectory(_directory);
                }
            }

            public void Put(ReadOnlySpan<char> line)
            {
                var letter = GetIndexingChar(line);
                if (!_files.TryGetValue(letter, out var writer))
                {
                    var fileName = GetFileName(letter);
                    writer = new StreamWriter(fileName);
                    _files.Add(letter, writer);
                    _fileNamesMap.Add(letter, fileName);
                }
                writer.WriteLine(line);
            }

            private char GetIndexingChar(ReadOnlySpan<char> line)
            {
                // Сначала ищем первый не пробельный символ
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] != ' ')
                        return char.ToLower(line[i]);
                }
                
                // Если строка пустая или состоит из одних пробелов
                return ' ';
            }
            
            private string GetFileName(char letter)
            {
                var fileName = $"tmp_{((byte)letter):X2}.txt";
                if (!string.IsNullOrWhiteSpace(_directory))
                {
                    fileName = $"{_directory}\\{fileName}";
                }
                return fileName;
            }
            
            public Dictionary<char, string> GetSplitFilesMap() => _fileNamesMap;

            public void Dispose()
            {
                foreach (var (_ , writer) in _files)
                {
                    writer.Dispose();
                }
            }
        }
    }
}