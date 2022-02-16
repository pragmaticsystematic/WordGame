using System;
using System.Collections.Generic;

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
            for (int i = 0; i < WordMaxLength; i++)
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
            if (!isWordEmpty())
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

        private bool isWordEmpty()
        {
            return this._inputLetterIndex == 0;
        }

        public override string ToString()
        {
            string output = "";
            foreach (var letter in LetterList)
            {
                output += $"{letter}\n";
            }

            return output;
        }
    }
}