using System.Collections;
using System.Collections.Generic;
using Backend;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public GameObject ScrollViewContent;
    public GameObject KeyboardButton;

    [SerializeField] private GameObject gameGrid;


    private string keys = "qwertyuiopasdfghjklzxcvbnm<";

    private List<KeyboardButton> _keyboardButtons;

    private GameLogic _gameLogic;

    public GameManager()
    {
        this._keyboardButtons = new List<KeyboardButton>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game manager start");
        this._gameLogic = new GameLogic();
        initializeOnScreenKeyboard();
        InitializeBoard();
    }


    private void initializeOnScreenKeyboard()
    {
        foreach (var t in keys)
        {
            var keyboardButton =
                Instantiate(KeyboardButton, ScrollViewContent.transform, false) as GameObject;
            var letterData           = new LetterData(t);
            var keyboardButtonScript = keyboardButton.GetComponent<KeyboardButton>();
            keyboardButtonScript.Init(letterData);
            keyboardButtonScript.OnButtonClicked -= KeyboardButtonPressed;
            keyboardButtonScript.OnButtonClicked += KeyboardButtonPressed;
            _keyboardButtons.Add(keyboardButtonScript);
        }
    }

    private void InitializeBoard()
    {
        var letterList = _gameLogic.GameBoard.GetAllLetters();
        var gameGridScript = gameGrid.GetComponent<GameGridScript>();
        gameGridScript.Init();
        gameGridScript.InitializeLetters(letterList);
    }


    private void KeyboardButtonPressed(LetterData pressedLetterData)
    {
        Debug.Log($"KeyboardButtonPressed: {pressedLetterData}");
        if (pressedLetterData.CurrentLetter.Equals('<'))
        {
            var result = this._gameLogic.GameBoard.RemoveLetter();
            Debug.Log($"Trying to remove a letter from the current word. Result is: {result}");
        }
        else
        {
            var result = this._gameLogic.GameBoard.AddLetter(pressedLetterData);
            Debug.Log($"Trying to add letter {pressedLetterData.CurrentLetter} to the next index. Result is: {result}");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}