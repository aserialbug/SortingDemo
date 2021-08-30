using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace generate.Infrastructure
{
    public class WordSource
    {
        private const string RESOURCE_NAME = "generate.Resources.english3.txt";
        
        private readonly string[] _words;
        private readonly Random _random;
        public WordSource()
        {
            using var resourceStream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(RESOURCE_NAME);

            using var resourceStreamReader = new StreamReader(resourceStream);

            var word = String.Empty;
            var wordsList = new List<string>();
            while ((word = resourceStreamReader.ReadLine()) != null)
            {
                wordsList.Add(word);
            }

            _words = wordsList.ToArray();
            _random = new Random();
        }
        public string GetNextRandomWord()
        {
            return _words[_random.Next(_words.Length)];
        }
    }
}