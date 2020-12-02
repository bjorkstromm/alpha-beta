using System;
using System.Collections.Generic;
using System.IO;

namespace AlphaBeta.Core
{
    public class WordService
    {
        private readonly List<Word> _words;
        private readonly Queue<Word> _queue;
        private readonly Random _random;
        private readonly object _lock;

        public WordService(Configuration configuration)
        {
            _words = Load(configuration);
            _random = new Random();
            _queue = new Queue<Word>();
            _lock = new object();
        }

        private List<Word> Load(Configuration configuration)
        {
            var result = new List<Word>();
            var content = File.ReadAllLines(configuration.WordFile);
            foreach (var line in content)
            {
                var separator = line.IndexOf('=');
                if (separator != -1)
                {
                    var text = line.Substring(0, separator).Trim();
                    var query = line.Substring(separator + 1).Trim();
                    result.Add(new Word(text, query));
                }
                else
                {
                    result.Add(new Word(line.Trim()));
                }
            }

            return result;
        }

        public Word GetRandomWord()
        {
            lock (_lock)
            {
                if (_queue.Count == 0)
                {
                    var list = new List<Word>(_words);
                    for (int index = list.Count - 1; index > 0; index--)
                    {
                        int randomIndex = _random.Next(index + 1);
                        var temp = list[randomIndex];
                        list[randomIndex] = list[index];
                        list[index] = temp;
                    }

                    foreach (var word in list)
                    {
                        _queue.Enqueue(word);
                    }
                }

                return _queue.Dequeue();
            }
        }
    }
}
