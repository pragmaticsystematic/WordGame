using System;
using System.Collections.Generic;
using Common;

namespace Network
{
    [Serializable]
    public class LetterDataSchema
    {
        public char        Letter      { get; set; }
        public LetterState LetterState { get; set; }
    }

    [Serializable]
    public class WordDataSchema
    {
        public List<LetterDataSchema> LetterData { get; set; }
    }
}