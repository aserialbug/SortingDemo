using System;
using System.Diagnostics;
using generate.Application;
using generate.Infrastructure;

namespace generate
{
    static class Program
    {
        private const int SUCCESS = 0;
        private const int ERROR = 1;
        
        static int Main(string[] args)
        {
            // 1. Проверяем входные параметры команды
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid argument count. There must be 2 arguments: output file name anf file size in Gb");
                return ERROR;
            }
            if (string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Invalid output file name");
                return ERROR;
            }
            var outputFileName = args[0];

            if (string.IsNullOrWhiteSpace(args[1]) || !int.TryParse(args[1], out var outputFileSize))
            {
                Console.WriteLine("Invalid destination file size");
                return ERROR;
            }
            
            // 2. Создаем и собираем компоненты системы
            var options = new Options
            {
                MaxLineLength = 1024
            };

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var wordSource = new WordSource();
                using var output = new OutputContainer(outputFileName);
                var lineBuilder = new LineBuilder(wordSource, options);
                var generatorService = new GeneratorService(lineBuilder, output);

                // 3. Запускаем создание файла
                generatorService.Generate(outputFileSize * 1073741824L);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error generation file {outputFileName}: {e.Message}");
                Console.WriteLine(e.StackTrace);
                return ERROR;
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"Completed in {stopwatch.Elapsed}");
            }

            return SUCCESS;
        }
    }
}