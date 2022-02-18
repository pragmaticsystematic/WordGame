using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Backend
{
    /// <summary>
    /// This class represents the game board. It has a list of words that represent rows in the game grid.
    /// Each word consists of letters. 
    /// </summary>
    public class GameBoard
    {
        //fields
        private readonly int            _maxNumberOfTries;
        private readonly List<WordData> _wordList;


        //properties
        public int CurrentWordIndex { get; set; }


        /// <summary>
        /// Constructor, Initializes the board with empty words.
        /// </summary>
        /// <param name="maxNumberOfTries">The maximum number of tries a player can make.
        /// This corresponds to the number of rows the game grid will have.</param>
        public GameBoard(int maxNumberOfTries)
        {
            _maxNumberOfTries = maxNumberOfTries;
            _wordList         = new List<WordData>();

            this.InitializeBoard();
        }

        /// <summary>
        /// Resets the game board by resetting all the words and letters that are in the board to their initial values.
        /// </summary>
        public void ResetBoard()
        {
            foreach (var word in _wordList)
            {
                word.ResetWord();
            }

            CurrentWordIndex = 0;
        }

        /// <summary>
        /// Gets the currently focused word. (Gets the word in the row which the user is currently filling)
        /// </summary>
        /// <returns>A WordData object for the word the user is currently filling.</returns>
        public WordData GetCurrentWord()
        {
            return _wordList[CurrentWordIndex];
        }


        /// <summary>
        /// Adds a letter to the row the user is currently filling.
        /// </summary>
        /// <param name="letterToAdd">The letter we want to add.</param>
        /// <returns>True if successful, False otherwise.</returns>
        public bool AddLetter(LetterData letterToAdd)
        {
            // Debug.Log($"Adding letter {letterToAdd} at CurrentWordIndex: {CurrentWordIndex}");
            return _wordList[CurrentWordIndex].AddLetter(letterToAdd);
        }

        /// <summary>
        /// Removes a letter from the row the user is currently filling.
        /// </summary>
        /// <returns>True if successful, False otherwise</returns>
        public bool RemoveLetter()
        {
            return _wordList[CurrentWordIndex].RemoveLetter();
        }


        /// <summary>
        /// Gets a list of all letters that exists in the board. 
        /// </summary>
        /// <returns>list of LetterData objects for all letters that exist in the game board.</returns>
        public List<LetterData> GetAllLetters()
        {
            //SelectMany flattens a list of list to a single list. In our case the game board contains a list of
            //words and the words contain a list of letters. Doing this is the same as looping over the words,
            //and then looping over the letters in each word and adding all the letters to a list and returning it.
            return _wordList.SelectMany(word => word.LetterList).ToList();
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

        public override string ToString()
        {
            return _wordList.Aggregate("Board Words: \n", (current, word) => current + $"{word}\n");
        }
    }
}