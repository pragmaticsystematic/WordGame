using System;
using System.Collections.Generic;
using Backend;
using Common;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// This class controls the Game Grid GUI element. Its function is to initialize the cells of the game grid
    /// with references to the back end letter data. And to pass the color information to the individual grid cells.
    /// It also can reset the GUI cells to their original empty position.
    /// </summary>
    public class GameGridScript : MonoBehaviour
    {
        // References defined in the Unity Editor
        [Tooltip("References to the GUI element that houses the game grid")] [SerializeField]
        private GameObject gameGrid;

        //fields
        private LetterCellScript[] _letterCellScripts;

        /// <summary>
        /// Initialize the Game Grid GUI element with the provided LetterData and tile colors 
        /// </summary>
        /// <param name="lettersData">List of LetterData objects that will be provided to the individual GUI cells.</param>
        /// <param name="tileColors">Object that houses the colors that will be used to color the grid tiles</param>
        public void Init(List<LetterData> lettersData, TileColors tileColors)
        {
            //Gets all LetterCellScripts in the game grid.
            var letterCells = gameGrid.GetComponentsInChildren<LetterCellScript>();
            _letterCellScripts = letterCells;

            InitializeLetters(lettersData, tileColors);
        }

        /// <summary>
        /// Resets the GUI grid cell to their empty position. Setting the cell LetterData letter to empty char and
        /// the letter state to an EMPTY state. 
        /// </summary>
        public void ResetGridCells()
        {
            //todo add a Reset method to LetterData so this could be done by calling letterCellScript.LetterData.Reset();
            foreach (var letterCellScript in _letterCellScripts)
            {
                letterCellScript.LetterData.CurrentLetter = char.MaxValue;
                letterCellScript.LetterData.State         = LetterState.Empty;
            }
        }

        /// <summary>
        /// Sets the each GUI grid cell with the backend letterData instance. Also passes the tile colors to each cell.
        /// </summary>
        /// <param name="lettersData">A list of LetterData</param>
        /// <param name="tileColors">The colors that will be used to color the tiles</param>
        private void InitializeLetters(IReadOnlyList<LetterData> lettersData, TileColors tileColors)
        {
            for (var i = 0; i < lettersData.Count; i++)
            {
                _letterCellScripts[i].LetterData = lettersData[i];
                _letterCellScripts[i].TileColors = tileColors;
            }
        }
    }
}