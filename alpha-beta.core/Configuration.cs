using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alpha_beta.core
{
    public class Configuration
    {
        public string Locale { get; }
        public string SearchKey { get; }
        public string SpeechKey { get; }
        public string WordFile { get; }

        public Configuration(string locale, string searchKey, string speechKey, string wordFile = "words.txt")
        {
            Locale = locale;
            SearchKey = searchKey;
            SpeechKey = speechKey;
            WordFile = wordFile;
        }
    }
}
