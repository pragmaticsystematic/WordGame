using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace DefaultNamespace.Menus.MenuElements
{
    public class MenuInputField : MonoBehaviour
    {
        [Tooltip("Button that is used for the input label.")] [SerializeField]
        private Button labelButton;

        [Tooltip("The input field")] [SerializeField]
        private InputField inputField;

        private Text _labelButtonTextElement;
        private Text _placeHolderTextElement;

        [Tooltip("Input label Text")] public string labelText;

        [Tooltip("Placeholder text for the input")]
        public string placeholderText;


        private void Awake()
        {
            _labelButtonTextElement      = labelButton.GetComponentInChildren<Text>();
            _placeHolderTextElement      = inputField.placeholder.GetComponent<Text>();
            _placeHolderTextElement.text = placeholderText;
            _labelButtonTextElement.text = labelText;

            inputField.onValueChanged.AddListener(CapitalizeText);
        }


        /// <summary>
        /// Gets the text input of the GUI element.
        /// </summary>
        /// <returns>The string the user had inputted in the GUI element</returns>
        public string GetInputText()
        {
            if (inputField.text == "")
            {
                return _placeHolderTextElement.text;
            }
            else
            {
                return inputField.text;
            }
        }

        /// <summary>
        /// Gets the placeholder text in the gui element
        /// </summary>
        /// <returns>The string that acts as the placeholder</returns>
        public string GetPlaceHolderText()
        {
            return _placeHolderTextElement.text;
        }


        /// <summary>
        /// Sets a new string to act as a placeholder of this input
        /// </summary>
        /// <param name="newPlaceholderText">The new placeholder text</param>
        public void SetPlaceHolderText(string newPlaceholderText)
        {
            _placeHolderTextElement.text = newPlaceholderText;
        }

        //We want all letters of this input to be capitalized.
        private void CapitalizeText(string inputString)
        {
            inputField.text = inputField.text.ToUpper();
        }
    }
}