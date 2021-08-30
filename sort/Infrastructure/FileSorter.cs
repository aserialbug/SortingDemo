using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace sort.Infrastructure
{
    public class FileSorter
    {
        public Task SortFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) 
                throw new ArgumentNullException(nameof(fileName));

            if (!File.Exists(fileName))
                throw new FileNotFoundException($"File {fileName} was not found");
            
            return Task.Run(() => ReadAndSort(fileName));
        }

        private void ReadAndSort(string file)
        {
            var stringList = new List<string>();

            using (var reader = new StreamReader(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    stringList.Add(line);
                }
            }

            // Старый добрый Array.Sort().
            // В итоге все сводится к сортировке массива строк в памяти
            // алгоритмом QuickSort. 
            stringList.Sort();

            using (var writer = new StreamWriter(file))
            {
                foreach (string s in stringList)
                {
                    writer.WriteLine(s);
                }
            }
        }
    }
}