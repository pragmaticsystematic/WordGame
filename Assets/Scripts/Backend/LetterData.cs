using System;
using Common;
using Network;

namespace Backend
{
    /// <summary>
    /// This class represents a single letter in the puzzle. It fires a OnLetterChanged event when
    /// a letter is changed to another letter and it fires a OnLetterStateChanged event when a letter
    /// state is changed. (Letter states reflect the letter's state in the current puzzle. If it exist in the word or not etc.)
    /// </summary>
    public class LetterData
    {
        //fields
        private char        _currentLetter;
        private LetterState _letterState = LetterState.Empty;

        //properties

        /// <summary>
        /// Property that sets/get the letter state. When a new value is set, an OnLetterStateChanged event is fired.
        /// </summary>
        public LetterState State
        {
            get => _letterState;
            set
            {
                _letterState = value;
                OnLetterStateChanged?.Invoke();
            }
        }

        /// <summary>
        /// Property that sets/gets the letter. When a new value is set, OnLetterChanged event is fired.
        /// </summary>
        public char CurrentLetter
        {
            get => _currentLetter;
            set
            {
                _currentLetter = value;
                OnLetterChanged?.Invoke();
            }
        }

        //delegates and events. 
        public delegate void OnLetterChangedDelegate();

        public event OnLetterChangedDelegate OnLetterChanged;

        public delegate void OnLetterStateChangedDelegate();

        public event OnLetterStateChangedDelegate OnLetterStateChanged;


        /// <summary>
        /// Empty constructor. Initializes the class with an empty letter 'char.MinValue'.
        /// </summary>
        public LetterData()
        {
            CurrentLetter = char.MinValue;
        }

        /// <summary>
        /// Constructor. Initialize the class with a given letter.
        /// </summary>
        /// <param name="letter">The letter to initialize the class with.</param>
        public LetterData(char letter)
        {
            CurrentLetter = char.ToLower(letter);
            State         = LetterState.Unchecked;
        }

        public LetterDataSchema ToSchema()
        {
            var schema = new LetterDataSchema {Letter = _currentLetter, LetterState = _letterState};
            return schema;
        }

        /// <summary>
        /// Overrides Default ToString(), Prints the current letter and it's state.
        /// </summary>
        /// <returns>String that describes the class contents.</returns>
        public override string ToString()
        {
            return $"Letter '{CurrentLetter}'. Letter State: {State.ToString()}";
        }
    }
}