using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using DefaultNamespace.Menus;
using UnityEngine;
using Random = System.Random;

namespace Backend
{
    /// <summary>
    /// Class responsible for the game logic. It has a game board that represents the game grid.
    /// And it houses the game rules and determines when the game is won or lost. 
    /// </summary>
    public class GameLogic
    {
        //fields

        public GameBoard GameBoard;

        private string _currentWordToGuess = "";

        private readonly int _maxNumberOfTries = 6;

        private int _numberOfTriesLeft;

        private readonly Random _randomGen;

        private List<string> _acceptableWordsPool;

        private const string ACCEPTABLE_WORDS_PATH = "words/all_words/words_of_length_5.csv";

        private const string COMMON_WORDS_PATH = "words/common_words/common_words_of_length_5.csv";


        //properties


        //delegates

        #region Delegates

        public delegate void OnGameWonDelegate(string gameOverMessage);

        public event OnGameWonDelegate OnGameWon;

        public delegate void OnGameLostDelegate(string gameOverMessage);

        public event OnGameLostDelegate OnGameLost;

        #endregion


        /// <summary>
        /// Constructor, initializes the game board with empty values. And loads the acceptable word pool from
        /// a csv file.
        /// </summary>
        public GameLogic()
        {
            GameBoard            = new GameBoard(_maxNumberOfTries);
            _numberOfTriesLeft   = _maxNumberOfTries;
            _randomGen           = new Random();
            _acceptableWordsPool = FileReader.ReadCsvFile(ACCEPTABLE_WORDS_PATH);
        }


        /// <summary>
        /// Resets the game to it's initial values. This resets the game board's letters to their empty state
        /// and chooses a new word for the player to guess. Also resets the number of tries.
        /// </summary>
        public void ResetGame()
        {
            GameBoard.ResetBoard();
            ChooseNewWordToGuess();
            _numberOfTriesLeft = _maxNumberOfTries;
        }


        /// <summary>
        /// Analyzes the current guess. And determines if the game is lost or won.
        /// </summary>
        /// <param name="updatedLetters">Returns by reference a list of letters that were updated in the current guess.
        /// For example if the user guessed the word 'route' the letters 'r','o','u','t','e' are
        /// updated. And this list will contain them. Function expects this list to be empty when it's supplied as
        /// an argument.
        /// </param>
        /// <param name="notificationScript">Currently used to update the GUI notification with a message.
        /// (ex: "Word doesn't exist"). </param>
        /// <returns></returns>
        public bool AnalyzeGuess(List<LetterData> updatedLetters, NotificationScript notificationScript)
        {
            var success     = false;
            var wordToCheck = this.GameBoard.GetCurrentWord();
            //todo: move notification to GameManager by using delegates and events from here. When we detect 
            //something that should happen we should send an even with this message.

            //If the word length is correct.
            if (wordToCheck.IsWordFilled())
            {
                //If the word exist in our dictionary
                if (IsLegitimateWord(wordToCheck))
                {
                    for (var i = 0; i < wordToCheck.LetterList.Count; i++)
                    {
                        var letter = wordToCheck.LetterList[i];
                        //letter exists in the word
                        if (_currentWordToGuess.Contains(letter.CurrentLetter.ToString()))
                        {
                            //letter exist in the word AND it's in the correct place
                            if (_currentWordToGuess[i] == letter.CurrentLetter)
                            {
                                letter.State = LetterState.LetterExistsInWordAndInOrder;
                            }
                            else //letter exist in word but not in the correct place.
                            {
                                letter.State = LetterState.LetterExistInWordButNotInOrder;
                            }
                        }
                        //letter does not exist in the word at all.
                        else
                        {
                            letter.State = LetterState.LetterDoesNotExistInWord;
                        }

                        updatedLetters.Add(letter);
                    }

                    // Check if we won the game.
                    if (CheckWin())
                    {
                        Debug.Log(
                            $"Congratulations! You won the game! And correctly Guessed the word! '{_currentWordToGuess}'");

                        OnGameWon?.Invoke(
                            $"Congratulations! You guessed the word! '{_currentWordToGuess.ToUpper()}'");
                    }
                    else //If game isn't won. Decrease the number of tries left. If no tries left, then the game is lost.
                    {
                        _numberOfTriesLeft -= 1;
                        if (_numberOfTriesLeft == 0)
                        {
                            Debug.Log($"You failed to guess the word '{_currentWordToGuess}'. Game Over!");
                            this.OnGameLost?.Invoke(
                                $"Sorry, The word was '{_currentWordToGuess.ToUpper()}'. Good Luck next time!'");
                        }
                        else
                        {
                            Debug.Log($"You have {_numberOfTriesLeft} tries left");
                        }
                    }

                    //Move to the next row in the game grid.
                    GameBoard.CurrentWordIndex += 1;
                    success                    =  true;
                }
                else // Word does not exist in our dictionary.
                {
                    Debug.Log($"Word: {wordToCheck} Does not exist in the dictionary!");
                    notificationScript.DisplayNotification(
                        $"The word {wordToCheck.GetWordString()} Doesn't exist in the dictionary!");
                }
            }
            else // Word length is not correct.
            {
                notificationScript.DisplayNotification($"The word is not long enough!");
            }

            return success;
        }

        /// <summary>
        /// Chooses a random word from a CSV file and sets it as the new word to guess.
        /// </summary>
        private void ChooseNewWordToGuess()
        {
            var words = FileReader.ReadCsvFile(COMMON_WORDS_PATH);

            // we expect all words to be lower case, but better safe than sorry.
            _currentWordToGuess = words[_randomGen.Next(0, words.Count())].ToLower();
        }

        /// <summary>
        /// Checks if the word 'wordToCheck' exist in the acceptable words dictionary
        /// </summary>
        /// <param name="wordToCheck">The word to check</param>
        /// <returns>True if the word exists, False otherwise.</returns>
        private bool IsLegitimateWord(WordData wordToCheck)
        {
            var stringToCheck = wordToCheck.GetWordString();
            return _acceptableWordsPool.Contains(stringToCheck);
        }


        /// <summary>
        /// Checks if the game is won.
        /// </summary>
        /// <returns>True if victory is achieved, False otherwise.</returns>
        private bool CheckWin()
        {
            /*
             * Checks if the game is won by going through all the letters of a word and checking their state
             * if a letters state is 'LetterExistsInWordAndInOrder' then it's added to a list.
             * Then we compare the length of the list to the length of the word we want to check.
             * If our list has the same amount of letters as in the word we want to check, it means that all the letters
             * are in the correct place.
             * 
             */
            var wordToCheck = this.GameBoard.GetCurrentWord();

            var letters = wordToCheck
                          .LetterList.TakeWhile(letterData =>
                                                    letterData.State == LetterState.LetterExistsInWordAndInOrder)
                          .ToList();

            return letters.Count == wordToCheck.LetterList.Count;
        }
    }
}