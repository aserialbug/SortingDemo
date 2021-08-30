using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using sort.Infrastructure;

namespace sort.Application
{
    public class SortingService
    {
        private readonly FileSplitter _fileSplitter;
        private readonly FileMerger _fileMerger;
        private readonly FileSorter _fileSorter;

        public SortingService(FileSplitter fileSplitter, FileMerger fileMerger, FileSorter fileSorter)
        {
            _fileSplitter = fileSplitter;
            _fileMerger = fileMerger;
            _fileSorter = fileSorter;
        }

        public async Task Sort(string sourceFileName, string destinationFileName)
        {
            if (!File.Exists(sourceFileName))
            {
                throw new FileNotFoundException($"Source file {sourceFileName} was not foud");
            }

            Dictionary<char, string> fileMap;

            // 1. Читаем исходный файл
            using (var sourceFileReader = new StreamReader(sourceFileName))
            {
                using (var tempStorage = _fileSplitter.BuildTemporaryStorage())
                {
                    // Читая исходный файл построчно преобразуем строки 
                    // к виду более подходящему для сортировки и записываем
                    // строки в разные файлы в зависимости от первой буквы строки
                    string line;
                    while ((line = sourceFileReader.ReadLine()) != null)
                    {
                        _fileSplitter.ProcessLine(line, tempStorage);
                    }
                    fileMap = tempStorage.GetSplitFilesMap();
                }
            }
            
            // 2. Сортируем файлы по отдельности по возможности паралельно
            // Запускаем задачи сортировки для каждого файла и дожидаемся 
            // окончания всех запущеных задач
            await Task.WhenAll(fileMap.Select(pair => _fileSorter.SortFile(pair.Value)));

            // 3. Сливаем отсортированные файлы в один
            _fileMerger.Merge(destinationFileName, fileMap);
        }
    }
}