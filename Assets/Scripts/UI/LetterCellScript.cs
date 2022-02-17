using System;
using Backend;
using Common;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LetterCellScript : MonoBehaviour
    {
        [SerializeField] private GameObject letterCellPrefab;
        [SerializeField] private Text       letterText;
        [SerializeField] private Image      letterBackgroundImage;


        private LetterData _letterData;

        public LetterData LetterData
        {
            get => _letterData;
            set
            {
                _letterData = value;
                SetupLetterDelegate();
                OnLetterChange();
            }
        }

        public TileColors TileColors { get; set; }


        // public void Init(LetterData letterData)
        // {
        //     this.LetterData = letterData;
        //     OnLetterChange();
        //     letterBackgroundImage.color = Color.blue;
        //
        //
        //     //todo set background change on letterdata state change
        // }

        private void SetupLetterDelegate()
        {
            this.LetterData.OnLetterChanged -= OnLetterChange;
            this.LetterData.OnLetterChanged += OnLetterChange;

            this.LetterData.OnLetterStateChanged -= OnLetterStateChanged;
            this.LetterData.OnLetterStateChanged += OnLetterStateChanged;
        }

        public void OnLetterChange()
        {
            if (LetterData.CurrentLetter.Equals(char.MinValue))
            {
                letterText.text = "";
            }
            else
            {
                letterText.text = char.ToUpper(LetterData.CurrentLetter).ToString();
            }
        }

        private void OnLetterStateChanged()
        {
            // Debug.Log("OnLetterStateChange called in LetterCellScript");
            letterBackgroundImage.color = _letterData.State switch
            {
                LetterState.Empty                          => TileColors.letterColorUnchecked,
                LetterState.Unchecked                      => TileColors.letterColorUnchecked,
                LetterState.LetterDoesNotExistInWord       => TileColors.letterColorLetterNotExist,
                LetterState.LetterExistInWordButNotInOrder => TileColors.letterColorExistWrongPlace,
                LetterState.LetterExistsInWordAndInOrder   => TileColors.letterColorExistCorrectPlace,
                _                                          => throw new ArgumentOutOfRangeException()
            };
        }
    }
}