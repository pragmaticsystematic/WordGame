using System;
using Backend;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    /// <summary>
    /// Represents the GUI element for an on-screen keyboard button.
    /// </summary>
    public class KeyboardButton : MonoBehaviour
    {
        [SerializeField] private Button     buttonGuiElement;
        public                   LetterData LetterData { get; set; }
        private                  Text       _textGuiElement;

        public Button ButtonGuiElement
        {
            get => buttonGuiElement;
            set => buttonGuiElement = value;
        }

        public Text TextGuiElement
        {
            get => _textGuiElement;
            set => _textGuiElement = value;
        }

        public Image BackgroundImageGuiElement
        {
            get => _backgroundImageGuiElement;
            set => _backgroundImageGuiElement = value;
        }

        private Image       _backgroundImageGuiElement;
        private LetterState _previousLetterState;
        private TileColors  _tileColors;


        //delegates
        public delegate void OnButtonClickedDelegate(LetterData clickedLetterData);

        public event OnButtonClickedDelegate OnButtonClicked;


        //We use this instead of a constructor because Unity doesn't call constructors of MonoBehavior
        public void Init(LetterData letterData, TileColors tileColors)
        {
            LetterData                 = letterData;
            _tileColors                = tileColors;
            _textGuiElement            = buttonGuiElement.GetComponentInChildren<Text>();
            _backgroundImageGuiElement = buttonGuiElement.GetComponent<Image>();

            _textGuiElement.text = char.ToUpper(letterData.CurrentLetter).ToString();
            _previousLetterState = letterData.State;


            //callbacks
            buttonGuiElement.onClick.AddListener(() => { OnButtonClicked?.Invoke(this.LetterData); });

            LetterData.OnLetterStateChanged -= OnLetterStatusChanged;
            LetterData.OnLetterStateChanged += OnLetterStatusChanged;
        }

        private void OnLetterStatusChanged()
        {
            // Debug.Log($"Keyboard button {LetterData.CurrentLetter} letter state changed to {LetterData.State}");
            switch (LetterData.State)
            {
                case LetterState.Empty:
                case LetterState.Unchecked:
                    _backgroundImageGuiElement.color = _tileColors.letterColorUnchecked;
                    break;
                case LetterState.LetterDoesNotExistInWord:
                    _backgroundImageGuiElement.color = _tileColors.letterColorLetterNotExist;
                    break;
                case LetterState.LetterExistInWordButNotInOrder:
                    if (_previousLetterState != LetterState.LetterExistsInWordAndInOrder)
                    {
                        _backgroundImageGuiElement.color = _tileColors.letterColorExistWrongPlace;
                    }

                    break;
                case LetterState.LetterExistsInWordAndInOrder:
                    _backgroundImageGuiElement.color = _tileColors.letterColorExistCorrectPlace;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}