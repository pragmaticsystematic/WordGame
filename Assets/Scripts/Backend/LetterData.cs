using System;
using Common;

namespace Backend
{
    public class LetterData
    {
        
                
        //delegates
        public delegate void OnLetterChangedDelegate();

        public event OnLetterChangedDelegate OnLetterChanged;
        
        
        public delegate void OnLetterStateChangedDelegate();

        public event OnLetterStateChangedDelegate OnLetterStateChanged;

        private char _currentLetter;

        private LetterState _letterState = LetterState.Empty;
        
        public LetterState State
        {
            get => _letterState;
            set { _letterState = value; OnLetterStateChanged?.Invoke(); }
        }

        public char CurrentLetter
        {
            get => _currentLetter;
            set
            {
                _currentLetter = value;
                OnLetterChanged?.Invoke();
            }
        }



        public LetterData()
        {
            //Initializing letter with empty char.
            CurrentLetter = char.MinValue;
        }

        public LetterData(char letter)
        {
            CurrentLetter = char.ToLower(letter);
            State = LetterState.Unchecked;

        }

        public override string ToString()
        {
            return $"Letter '{CurrentLetter}'. Letter State: {State.ToString()}";
        }
    }
}