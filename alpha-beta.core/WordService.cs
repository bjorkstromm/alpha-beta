using System;
using System.Collections.Generic;

namespace alpha_beta.core
{
    public class WordService
    {
        private readonly IReadOnlyList<string> _words;
        private readonly int _wordsCount;

        public WordService(Configuration configuration)
        {
            _words = System.IO.File.ReadAllLines(configuration.WordFile);
            _wordsCount = _words.Count;
        }

        public string GetRandomWord()
        {
            var random = new Random();
            return _words[random.Next(0, _wordsCount)];
        }
    }
}
