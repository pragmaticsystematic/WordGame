using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace Backend
{
    public class WordData
    {
        public readonly int WordMaxLength = 5;
        public List<LetterData> LetterList { get; }
        private int _inputLetterIndex = 0;

        public WordData()
        {
            LetterList = new List<LetterData>();

            // initiate list with 'empty' letters.
            for (var i = 0; i < WordMaxLength; i++)
            {
                LetterList.Add(new LetterData());
            }
        }

        //appends a letter at the end of _letterList.
        public bool AddLetter(LetterData letterToAdd)
        {
            bool success = false;
            if (!IsWordFilled())
            {
                LetterList[_inputLetterIndex].CurrentLetter = letterToAdd.CurrentLetter;
                _inputLetterIndex++; //after last word is filled index will be WordMaxLength
                success = true;
            }

            return success;
        }
        
        //removes the last inputed letter.
        public bool RemoveLetter()
        {
            bool success = false;
            if (!IsWordEmpty())
            {
                
                LetterList[_inputLetterIndex - 1].CurrentLetter = char.MinValue;
                _inputLetterIndex--;
                success = true;
            }

            return success;
        }

        //Returns the letter at a specific index.
        public LetterData GetLetterAtIndex(int index)
        {
            return LetterList[index];
        }

        //Checks if all the letters for the current word are filled.
        public bool IsWordFilled()
        {
            return this._inputLetterIndex == WordMaxLength;
        }

        private bool IsWordEmpty()
        {
            return this._inputLetterIndex == 0;
        }

        public override string ToString()
        {
            return LetterList.Aggregate("", (current, letter) => current + $"{letter}\n");
        }

        public string GetWordString()
        {
            return LetterList.Aggregate("", (current, letter) => current + $"{letter.CurrentLetter}");
        }

        //Resets the current word letters to char.min (Empty char)
        public void ResetWord()
        {
            foreach (var letterData in LetterList)
            {
                letterData.CurrentLetter = char.MinValue;
                letterData.State = LetterState.Empty;
            }

            _inputLetterIndex = 0;
        }
    }
}