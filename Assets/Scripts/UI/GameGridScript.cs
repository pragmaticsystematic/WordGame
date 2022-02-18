using System;
using System.Collections.Generic;
using Backend;
using Common;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameGridScript : MonoBehaviour
    {
        [SerializeField] private GameObject gameGrid;

        private string letters = "abcdefghijklmnorsp";

        private LetterCellScript[] _letterCellScripts;

        public void Init()
        {
            //Gets all LetterCellScripts in the game grid.
            var letterCells = gameGrid.GetComponentsInChildren<LetterCellScript>();
            this._letterCellScripts = letterCells;
            
        }

        public void InitializeLetters(List<LetterData> lettersData, TileColors tileColors)
        {
            for (int i = 0; i < lettersData.Count; i++)
            {
                _letterCellScripts[i].LetterData = lettersData[i];
                _letterCellScripts[i].TileColors = tileColors;
            }
        }


        public void ResetGridCells()
        {
            foreach (var letterCellScript in _letterCellScripts)
            {
                letterCellScript.LetterData.CurrentLetter = char.MaxValue;
                letterCellScript.LetterData.State = LetterState.Empty;
            }
        }

    }
}