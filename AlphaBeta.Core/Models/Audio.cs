using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaBeta.Core
{
    public sealed class Audio
    {
        public string Text { get; set; }
        public byte[] Data { get; set; }

        public int Length => Data?.Length ?? 0;
    }
}
