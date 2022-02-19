using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public abstract class GameLogic
    {
        //fields

        private const int MAX_NUMBER_OF_TRIES = 6;

        public readonly GameBoard GameBoard;

        protected int NumberOfTriesLeft;


        //delegates

        #region Delegates

        public delegate void OnGameWonDelegate(string gameOverMessage);

        public event OnGameWonDelegate OnGameWon;

        public delegate void OnGameLostDelegate(string gameOverMessage);

        public event OnGameLostDelegate OnGameLost;

        protected void InvokeGameWon(string gameOverMessage)
        {
            OnGameWon?.Invoke(gameOverMessage);
        }

        protected void InvokeGameLost(string gameOverMessage)
        {
            OnGameLost?.Invoke(gameOverMessage);
        }

        #endregion


        /// <summary>
        /// Constructor, initializes the game board with empty values. And loads the acceptable word pool from
        /// a csv file.
        /// </summary>
        protected GameLogic()
        {
            GameBoard         = new GameBoard(MAX_NUMBER_OF_TRIES);
            NumberOfTriesLeft = MAX_NUMBER_OF_TRIES;
        }


        /// <summary>
        /// Resets the game to it's initial values. This resets the game board's letters to their empty state
        /// and chooses a new word for the player to guess. Also resets the number of tries.
        /// </summary>
        public virtual async Task ResetGame()
        {
            GameBoard.ResetBoard();
            NumberOfTriesLeft = MAX_NUMBER_OF_TRIES;
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
        public abstract Task<bool> AnalyzeGuess(List<LetterData> updatedLetters, NotificationScript notificationScript);
    }
}