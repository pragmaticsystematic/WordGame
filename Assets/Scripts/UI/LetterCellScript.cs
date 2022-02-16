using Backend;
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
        public LetterData LetterData { get=> _letterData;
            set
            {
                _letterData = value;
                SetupLetterDelegate();
                OnLetterChange();
            }
        }


        public void Init(LetterData letterData)
        {
            this.LetterData = letterData;
            OnLetterChange();
            letterBackgroundImage.color = Color.blue;


            //todo set background change on letterdata state change
        }

        private void SetupLetterDelegate()
        {
            this.LetterData.OnLetterChanged -= OnLetterChange;
            this.LetterData.OnLetterChanged += OnLetterChange;
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
    }
}