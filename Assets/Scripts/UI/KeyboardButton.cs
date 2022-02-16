using System;
using Backend;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class KeyboardButton : MonoBehaviour
    {
        [SerializeField] private GameObject keyboardButtonPrefabInstance;
        public                   LetterData LetterData { get; set; }
        [SerializeField] private Text       keyboardButtonTextReference;
        [SerializeField] private Image      keyboardButtonBackgroundImageReference;
        [SerializeField] private Button     keyboardButtonButton;
        
        
        //delegates
        public delegate void OnButtonClickedDelegate(LetterData clickedLetterData);

        public event OnButtonClickedDelegate OnButtonClicked;
        

        //We use this instead of a constructor because Unity doesn't call constructors of MonoBehavior
        public void Init(LetterData letterData)
        {
            this.LetterData = letterData;
            keyboardButtonTextReference.text = char.ToUpper(letterData.CurrentLetter).ToString();

            //callbacks
            keyboardButtonButton.onClick.AddListener(() => {OnButtonClicked?.Invoke(this.LetterData);});
            var cb = keyboardButtonButton.colors;
            cb.normalColor = Color.magenta;
        }
        
    }
}