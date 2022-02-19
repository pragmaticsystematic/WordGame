using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Network;

namespace Backend
{
    /// <summary>
    /// This class represents a single word in the puzzle.
    /// It contains a list of letter classes that together form a word.
    /// </summary>
    public class WordData
    {
        //fields
        private const int WORD_MAX_LENGTH   = 5;
        private       int _inputLetterIndex = 0;

        //properties
        public List<LetterData> LetterList { get; }


        /// <summary>
        /// Empty Constructor. Initializes the internal letter list with empty letters.
        /// </summary>
        public WordData()
        {
            LetterList = new List<LetterData>();

            // initiate list with 'empty' letters.
            for (var i = 0; i < WORD_MAX_LENGTH; i++)
            {
                LetterList.Add(new LetterData());
            }
        }

        /// <summary>
        /// Adds the letter `letterToAdd` to the current letter list at the correct index.
        /// </summary>
        /// <param name="letterToAdd">The letter we wish to add to the list</param>
        /// <returns>True if the letter was successfully added. False otherwise.</returns>
        public bool AddLetter(LetterData letterToAdd)
        {
            var success = false;
            if (!IsWordFilled())
            {
                LetterList[_inputLetterIndex].CurrentLetter = letterToAdd.CurrentLetter;
                _inputLetterIndex++; //after last word is filled index will be WordMaxLength
                success = true;
            }

            return success;
        }

        /// <summary>
        /// Removes the last letter from the letter list.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RemoveLetter()
        {
            var success = false;
            if (!IsWordEmpty())
            {
                //We decrease 1 from the index because AddLetter adds 1 after it inputs a letter.
                //Basically after we input letter on index 4 the index will be 5.
                //If we want to delete the last letter, we need to delete at index 5 - 1 = 4
                var indexToDelete = _inputLetterIndex - 1;

                LetterList[indexToDelete].CurrentLetter = char.MinValue;
                _inputLetterIndex                       = indexToDelete;
                success                                 = true;
            }

            return success;
        }


        //Checks if all the letters for the current word are filled.
        /// <summary>
        /// Checks if the current word is filled. In other words if the length of the current word is WORD_MAX_LENGTH
        /// </summary>
        /// <returns>True if the current word's length is WORD_MAX_LENGTH. </returns>
        public bool IsWordFilled()
        {
            //We use the index instead of LetterList.Count because the letterlist is initialized with empty letters.
            //It's count will always be WORD_MAX_LENGTH. Instead we look at the index of the current letter.
            //Indices start at 0, so we'd expect the index for a word of length WORD_MAX_LENGTH will be WORD_MAX_LENGTH -1
            //but because we increment the index right after we insert a new letter our index is always one unit higher
            //so in our case a word is of length WORD_MAX_LENGTH when index == WORD_MAX_LENGTH.
            return _inputLetterIndex == WORD_MAX_LENGTH;
        }

        /// <summary>
        /// Resets the current word back it's initial state. (Sets all letters in the word list to char.MinValue)
        /// </summary>
        public void ResetWord()
        {
            foreach (var letterData in LetterList)
            {
                letterData.CurrentLetter = char.MinValue;
                letterData.State         = LetterState.Empty;
            }

            _inputLetterIndex = 0;
        }

        /// <summary>
        /// Gets a string representation of the current word.
        /// </summary>
        /// <returns>String of the current word.</returns>
        public string GetWordString()
        {
            //todo BUG! This includes empty characters at the the end of the word if it isn't completely filled.
            //This is not the intended behavior. We want to return a word that contains all the characters from the start
            //till the first char.minvalue character.
            return LetterList.Aggregate("", (current, letter) => current + $"{letter.CurrentLetter}");
        }

        public WordDataSchema ToSchema()
        {
            var letterDataSchemaList = LetterList.Select(letterData => letterData.ToSchema()).ToList();
            var schema               = new WordDataSchema {LetterData = letterDataSchemaList};
            return schema;
        }

        /// <summary>
        /// Checks if the current word is empty
        /// </summary>
        /// <returns>True if the word is empty. False otherwise.</returns>
        private bool IsWordEmpty()
        {
            return _inputLetterIndex == 0;
        }

        public override string ToString()
        {
            return LetterList.Aggregate("", (current, letter) => current + $"{letter}\n");
        }
    }
}