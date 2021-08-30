using System.Collections.Generic;
using System.IO;
using sort.Application;

namespace sort.Infrastructure
{
    public class FileMerger
    {
        private readonly LineTransformer _lineTransformer;
        private readonly Options _options;

        public FileMerger(LineTransformer lineTransformer, Options options)
        {
            _lineTransformer = lineTransformer;
            _options = options;
        }

        public void Merge(string outputFileName, Dictionary<char, string> splitFilesMap)
        {
            using var outputFile = new StreamWriter(outputFileName);

            for (char letter = (char)0; letter <= 127; letter++)
            {
                if(!splitFilesMap.TryGetValue(letter, out var splitFile))
                    continue;
                
                using (var reader = new StreamReader(splitFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var original = _lineTransformer.ToOriginal(line);
                        outputFile.WriteLine(original);
                    }
                }

                if (_options.ShouldDeleteTemporaryFiles)
                {
                    File.Delete(splitFile);
                }
            }
            
            if (_options.ShouldDeleteTemporaryFiles)
            {
                Directory.Delete(_options.TemporaryDirectory);
            }
        }
    }
}