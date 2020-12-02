namespace AlphaBeta.Core
{
    public class Configuration
    {
        public string Locale { get; set; } = "en-GB";
        public string SearchKey { get; set; } = null;
        public string SpeechKey { get; set; } = null;
        public string SpeechRegion { get; set; } = null;
        public string WordFile { get; set; } = "words.txt";
    }
}
