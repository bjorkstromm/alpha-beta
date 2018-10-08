using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alpha_beta.core
{
    public class WordService
    {
        private readonly Random _random;
        private readonly IReadOnlyList<string> _words;
        private readonly int _wordsCount;

        public WordService(Configuration configuration)
        {
            _words = System.IO.File.ReadAllLines(configuration.WordFile);
            _random = new Random();
            _wordsCount = _words.Count;
        }

        public string GetRandomWord()
        {
            return _words[_random.Next(0, _wordsCount)];
        }
    }
}
