using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using Random = System.Random;

namespace Backend
{
    public class GameLogic
    {
        public GameBoard GameBoard;

        private string _currentWordToGuess = "";

        private readonly int _maxNumberOfTries = 6;

        private int _numberOfTriesLeft;

        private Random _randomGen;

        private List<string> wordPool;


        public GameLogic()
        {
            GameBoard               = new GameBoard(_maxNumberOfTries);
            this._numberOfTriesLeft = _maxNumberOfTries;
            this._randomGen         = new Random();
        }

        //delegates
        public delegate void OnGameWonDelegate();

        public event OnGameWonDelegate OnGameWon;

        public delegate void OnGameLostDelegate();

        public event OnGameLostDelegate OnGameLost;


        public void ResetGame()
        {
            this.GameBoard.ResetBoard();
            GenerateNewWordToGuess();
            _numberOfTriesLeft = _maxNumberOfTries;
            this.wordPool = FileReader.ReadCsvFile("Assets/Resources/all_words/words_of_length_5.csv");
        }

        private void GenerateNewWordToGuess()
        {
            var words = FileReader.ReadCsvFile("Assets/Resources/common_words/common_words_of_length_5.csv");
            this._currentWordToGuess = words[_randomGen.Next(0, words.Count())];
            //todo generate from dictionary
            // we expect all words to be lower letters.
            // this._currentWordToGuess = "route";
        }

        private bool IsLegitimateWord(WordData wordToCheck)
        {
            var stringToCheck = wordToCheck.GetWordString();
            return this.wordPool.Contains(stringToCheck);
        }

        public bool AnalyzeGuess(List<LetterData> updatedLetters)
        {
            var success     = false;
            var wordToCheck = this.GameBoard.GetCurrentWord();

            if (wordToCheck.IsWordFilled())
            {
                if (IsLegitimateWord(wordToCheck))
                {
                    // Debug.Log($"Word: {wordToCheck} is a legitimate word...");
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
                                // Debug.Log($"Letter {letter.CurrentLetter} is in the word {_currentWordToGuess} AND it's in the correct place!");
                            }
                            //letter exist in word but not in the correct place.
                            else
                            {
                                letter.State = LetterState.LetterExistInWordButNotInOrder;
                                
                                // Debug.Log($"Letter {letter.CurrentLetter} is in the word {_currentWordToGuess} BUT it's not in the correct place!");
                            }
                        }
                        //letter does not exist in the word at all.
                        else
                        {
                            letter.State = LetterState.LetterDoesNotExistInWord;
                            // Debug.Log($"Letter {letter.CurrentLetter} DOES NOT EXIST in the word {_currentWordToGuess}");
                        }
                        updatedLetters.Add(letter);
                    }

                    if (CheckWin())
                    {
                        Debug.Log(
                            $"Congratulations! You won the game! And correctly Guessed the word! '{_currentWordToGuess}'");
                        this.OnGameWon?.Invoke();
                    }
                    else
                    {
                        this._numberOfTriesLeft -= 1;
                        if (_numberOfTriesLeft == 0)
                        {
                            Debug.Log($"You failed to guess the word '{_currentWordToGuess}'. Game Over!");
                            this.OnGameLost?.Invoke();
                        }
                        else
                        {
                            Debug.Log($"You have {_numberOfTriesLeft} tries left");
                        }
                    }


                    this.GameBoard.CurrentWordIndex += 1;
                    success                         =  true;
                }
                else
                {
                    Debug.Log($"Word: {wordToCheck} Does not exist in the dictionary!");
                }
            }

            return success;
        }

        public bool CheckWin()
        {
            var wordToCheck = this.GameBoard.GetCurrentWord();

            var letters = wordToCheck
                          .LetterList.TakeWhile(letterData =>
                                                    letterData.State == LetterState.LetterExistsInWordAndInOrder)
                          .ToList();

            return letters.Count == wordToCheck.LetterList.Count;
        }
    }
}