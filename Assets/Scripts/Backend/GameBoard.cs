using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Backend
{
    public class GameBoard
    {
        private int            _maxNumberOfTries;
        private List<WordData> _wordList;
        private int            _currentWordIndex = 0;

        public GameBoard(int maxNumberOfTries)
        {
            _maxNumberOfTries = maxNumberOfTries;
            _wordList = new List<WordData>();
            
            this.InitializeBoard();
        }

        public int CurrentWordIndex
        {
            get => _currentWordIndex;
            set => _currentWordIndex = value;
        }

        
        private void InitializeBoard()
        {
            //Initiate board with empty words.
            for (var i = 0; i < _maxNumberOfTries; i++)
            {
                _wordList.Add(new WordData());
            }

            CurrentWordIndex = 0;
        }

        public void ResetBoard()
        {
            foreach (var word in _wordList)
            {
                word.ResetWord();
            }

            CurrentWordIndex = 0;
        }

        public WordData GetCurrentWord()
        {
            return _wordList[CurrentWordIndex];
        }

        public bool AddLetter(LetterData letterToAdd)
        {
            // Debug.Log($"Adding letter {letterToAdd} at CurrentWordIndex: {CurrentWordIndex}");
            return this._wordList[CurrentWordIndex].AddLetter(letterToAdd);
        }

        public bool RemoveLetter()
        {
            return this._wordList[CurrentWordIndex].RemoveLetter();
        }

        // returns the number of tries left
        public int NumberOfTriesLeft()
        {
            return this._maxNumberOfTries - this._currentWordIndex;
        }


        public List<LetterData> GetAllLetters()
        {
            return _wordList.SelectMany(word => word.LetterList).ToList();
        }


        public override string ToString()
        {
            return _wordList.Aggregate("Board Words: \n", (current, word) => current + $"{word}\n");
        }
        
    }
}