using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DefaultNamespace.Menus;
using UnityEngine;
using Random = System.Random;

namespace Backend
{
    public class GameLogicLocal : GameLogic
    {
        private const string ACCEPTABLE_WORDS_PATH = "words/all_words/words_of_length_5.csv";

        private const string COMMON_WORDS_PATH = "words/common_words/common_words_of_length_5.csv";

        private string _currentWordToGuess = "";

        private readonly Random _randomGen;

        private readonly List<string> _acceptableWordsPool;

        public GameLogicLocal() : base()
        {
            _randomGen           = new Random();
            _acceptableWordsPool = FileReader.ReadCsvFile(ACCEPTABLE_WORDS_PATH);
        }

        /// <summary>
        /// Resets the game to it's initial values. This resets the game board's letters to their empty state
        /// and chooses a new word for the player to guess. Also resets the number of tries.
        /// </summary>
        public override async Task ResetGame()
        {
            await base.ResetGame();
            ChooseNewWordToGuess();
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
        public override async Task<bool> AnalyzeGuess(List<LetterData>   updatedLetters,
                                                      NotificationScript notificationScript)
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

                        updatedLetters.Add(letter); //used to color the keyboard letters
                    }

                    // Check if we won the game.
                    if (CheckWin())
                    {
                        Debug.Log(
                            $"Congratulations! You won the game! And correctly Guessed the word! '{_currentWordToGuess}'");

                        InvokeGameWon(
                            $"Congratulations! You guessed the word! '{_currentWordToGuess.ToUpper()}'");
                    }
                    else //If game isn't won. Decrease the number of tries left. If no tries left, then the game is lost.
                    {
                        NumberOfTriesLeft -= 1;
                        if (NumberOfTriesLeft == 0)
                        {
                            Debug.Log($"You failed to guess the word '{_currentWordToGuess}'. Game Over!");
                            InvokeGameWon(
                                $"Sorry, The word was '{_currentWordToGuess.ToUpper()}'. Good Luck next time!'");
                        }
                        else
                        {
                            Debug.Log($"You have {NumberOfTriesLeft} tries left");
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