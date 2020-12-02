using System.Diagnostics;

namespace AlphaBeta.Core
{
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class Word
    {
        public string Text { get; set; }
        public string Query { get; set; }

        public Word(string text)
            : this(text, text)
        {
        }

        public Word(string text, string query)
        {
            Text = text;
            Query = query;
        }

        public override string ToString()
        {
            if (Text == Query)
            {
                return Text;
            }

            return $"{Text} [{Query}]";
        }
    }
}
