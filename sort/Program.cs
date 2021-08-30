using System;
using System.Diagnostics;
using System.Threading.Tasks;
using sort.Application;
using sort.Infrastructure;

namespace sort
{
    static class Program
    {
        private const int SUCCESS = 0;
        private const int ERROR = 1;
        
        static async Task<int> Main(string[] args)
        {
            // 1. Проверяем входные параметры команды
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid argument count. There must be 2 arguments: source and destination files");
                return ERROR;
            }

            if (string.IsNullOrWhiteSpace(args[0]))
            {
                Console.WriteLine("Invalid source file name");
                return ERROR;
            }
            var sourceFile = args[0];
            
            if (string.IsNullOrWhiteSpace(args[1]))
            {
                Console.WriteLine("Invalid destination file name");
                return ERROR;
            }
            var destinationFile = args[1];

            // 2. Создаем и собираем компоненты системы
            Options options = new Options
            {
                TemporaryDirectory = "tmp",
                ShouldDeleteTemporaryFiles = true
            };

            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                var transformer = new LineTransformer();
                var splitter = new FileSplitter(transformer, options);
                var merger = new FileMerger(transformer, options);
                var sorter = new FileSorter();
                var sortingService = new SortingService(splitter, merger, sorter);

                // 3. Сортируем
                await sortingService.Sort(sourceFile, destinationFile);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sorting {sourceFile}: {e.Message}");
                Console.WriteLine(e.StackTrace);
                return ERROR;
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"Completed in {stopwatch.Elapsed}");
            }

            // 4. Успршная сортировка
            return SUCCESS;
        }
    }
}