using System;
using System.Collections.Generic;
using Backend;
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
            var letterCells = gameGrid.GetComponentsInChildren<LetterCellScript>();
            this._letterCellScripts = letterCells;
            
        }

        public void InitializeLetters(List<LetterData> lettersData)
        {
            for (int i = 0; i < lettersData.Count; i++)
            {
                _letterCellScripts[i].LetterData = lettersData[i];
            }
        }

        // public void Awake()
        // {
        //     var letterCells = gameGrid.GetComponentsInChildren<LetterCellScript>();
        //
        //     for (int i = 0; i < letters.Length; i++)
        //     {
        //         var currentLetter = letters[i];
        //         letterCells[i].LetterData = new LetterData(currentLetter);
        //         // letterCells[i].OnLetterChange();
        //     }
        // }
    }
}