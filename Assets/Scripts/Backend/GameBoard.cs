using System.Collections.Generic;
using System.Linq;

namespace Backend
{
    public class GameBoard
    {
        public  int            MaxNumberOfTries = 6;
        private List<WordData> _wordList;
        private int            _currentWordIndex = 0;

        public GameBoard()
        {
            _wordList = new List<WordData>();
            this.ResetBoard();
        }

        public void ResetBoard()
        {
            //Initiate board with empty words.
            for (int i = 0; i < MaxNumberOfTries - 1; i++)
            {
                _wordList.Add(new WordData());
            }

            _currentWordIndex = 0;
        }

        public WordData GetCurrentWord()
        {
            //todo increment _currentWordIndex when a 'enter' is pressed a a full guess was made.
            WordData currentWord = _wordList[_currentWordIndex];

            return _wordList[_currentWordIndex];
        }

        public bool AddLetter(LetterData letterToAdd)
        {
            return this._wordList[_currentWordIndex].AddLetter(letterToAdd);
        }

        public bool RemoveLetter()
        {
            return this._wordList[_currentWordIndex].RemoveLetter();
        }


        public List<LetterData> GetAllLetters()
        {
            return _wordList.SelectMany(word => word.LetterList).ToList();
        }


        public override string ToString()
        {
            string output = "Board Words: \n";
            foreach (var word in _wordList)
            {
                output += $"{word}\n";
            }

            return output;
        }
        
    }
}