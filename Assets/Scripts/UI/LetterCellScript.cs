using System;
using Backend;
using Common;
using DG.Tweening;
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
                letterText.DOText("", 0.45f);
                // letterText.text = "";
            }
            else
            {
                letterText.DOText(char.ToUpper(LetterData.CurrentLetter).ToString(), 0.45f);
                // letterText.text = char.ToUpper(LetterData.CurrentLetter).ToString();
            }
        }

        private void OnLetterStateChanged()
        {
            const float duration = 0.45f;
            switch (_letterData.State)
            {
                case LetterState.Empty:
                case LetterState.Unchecked:
                    letterBackgroundImage.DOColor(TileColors.letterColorUnchecked, duration);
                    break;
                case LetterState.LetterDoesNotExistInWord:
                    letterBackgroundImage.DOColor(TileColors.letterColorLetterNotExist, duration);
                    break;
                case LetterState.LetterExistInWordButNotInOrder:
                    letterBackgroundImage.DOColor(TileColors.letterColorExistWrongPlace, duration);
                    break;
                case LetterState.LetterExistsInWordAndInOrder:
                    letterBackgroundImage.DOColor(TileColors.letterColorExistCorrectPlace, duration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // // Debug.Log("OnLetterStateChange called in LetterCellScript");
            // letterBackgroundImage.color = _letterData.State switch
            // {
            //     LetterState.Empty                          => TileColors.letterColorUnchecked,
            //     LetterState.Unchecked                      => TileColors.letterColorUnchecked,
            //     LetterState.LetterDoesNotExistInWord       => TileColors.letterColorLetterNotExist,
            //     LetterState.LetterExistInWordButNotInOrder => TileColors.letterColorExistWrongPlace,
            //     LetterState.LetterExistsInWordAndInOrder   => TileColors.letterColorExistCorrectPlace,
            //     _                                          => throw new ArgumentOutOfRangeException()
            // };
        }
    }
}