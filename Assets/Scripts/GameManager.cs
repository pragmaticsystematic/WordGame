using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend;
using Common;
using DefaultNamespace;
using DefaultNamespace.Menus;
using Network;
using Network.NetworkCodes;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public enum ConnectionStatus
{
    NotConnected,
    Pending,
    Connected
}

public class GameManager : MonoBehaviour
{
    public GameObject ScrollViewContent;
    public GameObject KeyboardButton;

    public GameObject KeyboardKeyRow;
    public GameObject KeyboardContainer;

    public GameObject KeybaordButtonLong;

    public NotificationScript notificationScript;


    [SerializeField] private GameObject gameGrid;


    private readonly string keys = "qwertyuiopasdfghjklzxcvbnm<!&";

    private readonly List<string> keyList = new List<string>() {"qwertyuiop", "asdfghjkl<", "zxcvbnm!"};

    private Dictionary<char, KeyboardButton> _keyboardButtons;

    private GameLogic _gameLogic;

    private List<LetterData> _keyboardLettersToUpdate;

    private TileColors _tileColors;

    private GameGridScript _gameGridScript;

    private MenuManager _menuManager;

    private TcpClient _tcpClient;

    private GameMode _gameMode;

    private string _playerIdentifier;

    private string _serverIp;
    private int    _serverPort;

    private ConnectionStatus _connectionStatus = ConnectionStatus.NotConnected;


    public GameManager()
    {
        this._keyboardButtons = new Dictionary<char, KeyboardButton>();
    }

    // Start is called before the first frame update
    private void Awake()
    {
        Debug.Log("Game manager start");

        _keyboardLettersToUpdate = new List<LetterData>();
        _tileColors              = GetComponent<TileColors>();
        _menuManager             = GetComponent<MenuManager>();
        // _tcpClient               = new TcpClient("127.0.0.1", 13000);
        // _gameMode                = GameMode.Online;
        //
        // var randomNumber = new Random().Next(0, 5000);
        //
        // if (_gameMode == GameMode.Offline)
        // {
        //     this._gameLogic = new GameLogicLocal();
        // }
        // else
        // {
        //     _playerIdentifier = $"Michael_{randomNumber}";
        //     this._gameLogic   = new GameLogicRemote(_tcpClient, _playerIdentifier);
        // }

        //initializes the game logic and selects a new word to guess.
        // this._gameLogic.ResetGame();

        // initializeOnScreenKeyboard();
        // InitializeBoard();

        initializeOnScreenKeyboard();


        //Event callbacks connections.
        _menuManager.OnRestartGame -= ResetGame;
        _menuManager.OnRestartGame += ResetGame;

        _menuManager.OnStartOnlineGame -= StartNewOnlineGame;
        _menuManager.OnStartOnlineGame += StartNewOnlineGame;

        _menuManager.OnStartOfflineGame -= StartOfflineGame;
        _menuManager.OnStartOfflineGame += StartOfflineGame;

        // _gameLogic.OnGameLost -= _menuManager.GameOver;
        // _gameLogic.OnGameLost += _menuManager.GameOver;
        //
        // _gameLogic.OnGameWon -= _menuManager.GameOver;
        // _gameLogic.OnGameWon += _menuManager.GameOver;

        // _tcpClient.OnSeverReplyReceived -= HandleServerReply;
        // _tcpClient.OnSeverReplyReceived += HandleServerReply;
    }

    private void StartNewOnlineGame(string ip, int port, string playerName)
    {
        _tcpClient?.CloseConnection();

        _tcpClient        = new TcpClient(ip, port);
        _playerIdentifier = playerName;
        this._gameLogic   = new GameLogicRemote(_tcpClient, _playerIdentifier);


        InitializeBoard();

        _gameLogic.OnGameLost -= _menuManager.GameOver;
        _gameLogic.OnGameLost += _menuManager.GameOver;

        _gameLogic.OnGameWon -= _menuManager.GameOver;
        _gameLogic.OnGameWon += _menuManager.GameOver;

        _tcpClient.OnSeverReplyReceived -= HandleServerReply;
        _tcpClient.OnSeverReplyReceived += HandleServerReply;

        _tcpClient.OnConnectionStatusChanged -= SetConnectionStatus;
        _tcpClient.OnConnectionStatusChanged += SetConnectionStatus;

        _connectionStatus = ConnectionStatus.Pending;
        _tcpClient.ConnectToTcpServer();

        StartCoroutine(CheckConnectionStatus(5));
    }

    private void SetConnectionStatus(ConnectionStatus connectionStatus)
    {
        this._connectionStatus = connectionStatus;
    }


    IEnumerator CheckConnectionStatus(int timeoutSeconds)
    {
        var timeoutCounter = 0;
        while (timeoutCounter <= timeoutSeconds && _connectionStatus == ConnectionStatus.Pending)
        {
            yield return new WaitForSeconds(1f);
            timeoutCounter += 1;
            Debug.Log("Waiting to determine connection status...");
        }

        if (_connectionStatus == ConnectionStatus.NotConnected)
        {
            Debug.Log("Connection status - FAILED to connect");
            _menuManager.ShowFailedToConnectToServerMenu();
        }
        else if (_connectionStatus == ConnectionStatus.Connected)
        {
            Debug.Log("Connection status - Success");
            ResetGame();
        }
    }

    private void StartOfflineGame()
    {
        _tcpClient?.CloseConnection();
        _gameLogic = new GameLogicLocal();

        InitializeBoard();

        _gameLogic.OnGameLost -= _menuManager.GameOver;
        _gameLogic.OnGameLost += _menuManager.GameOver;

        _gameLogic.OnGameWon -= _menuManager.GameOver;
        _gameLogic.OnGameWon += _menuManager.GameOver;

        ResetGame();
    }


    private void initializeOnScreenKeyboard()
    {
        foreach (var keyString in keyList)
        {
            var buttonRow               = Instantiate(KeyboardKeyRow, KeyboardContainer.transform, false) as GameObject;
            var keyboardContainerWidth  = ((RectTransform) KeyboardContainer.transform).rect.width;
            var keyboardContainerHeight = ((RectTransform) KeyboardContainer.transform).rect.width;
            var keyDesiredWidth         = (keyboardContainerWidth  - 20) / 12;
            var keyDesiredHeight        = (keyboardContainerHeight - 10) / 3;
            var keyActualWidth          = Math.Max(keyDesiredWidth, keyDesiredHeight);
            foreach (var t in keyString)
            {
                switch (t)
                {
                    case '<':
                    {
                        var keyboardButton =
                            Instantiate(KeybaordButtonLong, buttonRow.transform, false) as GameObject;
                        var rect = ((RectTransform) keyboardButton.transform).rect;
                        rect.width  = keyActualWidth;
                        rect.height = keyActualWidth;
                        var keyboardButtonScript = keyboardButton.GetComponent<KeyboardButton>();
                        var letterData           = new LetterData(t);
                        keyboardButtonScript.Init(letterData, _tileColors);
                        keyboardButtonScript.TextGuiElement.text = "<<";
                        keyboardButtonScript.ButtonGuiElement.onClick.AddListener(OnBackspacePressed);
                        break;
                    }
                    case '!':
                    {
                        var keyboardButton =
                            Instantiate(KeybaordButtonLong, buttonRow.transform, false) as GameObject;
                        var rect = ((RectTransform) keyboardButton.transform).rect;
                        rect.width  = keyActualWidth;
                        rect.height = keyActualWidth;
                        var keyboardButtonScript = keyboardButton.GetComponent<KeyboardButton>();
                        var letterData           = new LetterData(t);
                        keyboardButtonScript.Init(letterData, _tileColors);
                        keyboardButtonScript.TextGuiElement.text = "ENTER";
                        keyboardButtonScript.ButtonGuiElement.onClick.AddListener(OnEnterPressed);

                        break;
                    }
                    default:
                    {
                        var keyboardButton =
                            Instantiate(KeyboardButton, buttonRow.transform, false) as GameObject;
                        var rect = ((RectTransform) keyboardButton.transform).rect;
                        rect.width  = keyActualWidth;
                        rect.height = keyActualWidth;
                        var letterData           = new LetterData(t);
                        var keyboardButtonScript = keyboardButton.GetComponent<KeyboardButton>();
                        keyboardButtonScript.Init(letterData, _tileColors);
                        keyboardButtonScript.OnButtonClicked -= KeyboardButtonPressed;
                        keyboardButtonScript.OnButtonClicked += KeyboardButtonPressed;
                        _keyboardButtons.Add(t, keyboardButtonScript);
                        break;
                    }
                }
            }
        }
    }

    private void InitializeBoard()
    {
        var letterList = _gameLogic.GameBoard.GetAllLetters();
        _gameGridScript = gameGrid.GetComponent<GameGridScript>();
        _gameGridScript.Init(letterList, _tileColors);
    }


    private async void ResetGame()
    {
        //Reset Keyboard buttons
        foreach (var keyboardButton in _keyboardButtons.Values)
        {
            keyboardButton.LetterData.State = LetterState.Unchecked;
        }

        //Reset board
        _gameGridScript.ResetGridCells();

        //Reset Game Logic

        await _gameLogic.ResetGame();
        _keyboardLettersToUpdate.Clear();
    }


    private void KeyboardButtonPressed(LetterData pressedLetterData)
    {
        // Debug.Log($"KeyboardButtonPressed: {pressedLetterData}");
        if (pressedLetterData.CurrentLetter.Equals('<'))
        {
            var result = this._gameLogic.GameBoard.RemoveLetter();
            // Debug.Log($"Trying to remove a letter from the current word. Result is: {result}");
        }
        else if (pressedLetterData.CurrentLetter.Equals('!'))
        {
            // Debug.Log($"Analyzing the current word...");
            _keyboardLettersToUpdate.Clear();
            this._gameLogic.AnalyzeGuess(_keyboardLettersToUpdate, notificationScript);
            UpdateKeyboardLettersState();
        }
        else if (pressedLetterData.CurrentLetter.Equals(('&')))
        {
            ResetGame();
        }
        else
        {
            var result = this._gameLogic.GameBoard.AddLetter(pressedLetterData);
            // Debug.Log($"Trying to add letter {pressedLetterData.CurrentLetter} to the next index. Result is: {result}");
        }
    }

    private void OnBackspacePressed()
    {
        var result = this._gameLogic.GameBoard.RemoveLetter();
    }

    private async void OnEnterPressed()
    {
        _keyboardLettersToUpdate.Clear();
        var    wordData         = _gameLogic.GameBoard.GetCurrentWord();
        var    currentGuessWord = wordData.GetWordString();
        string jsonString       = JsonUtility.ToJson(wordData.ToSchema());
        // _tcpClient.SendMessage($"Hello from Unity! Player guess is `{currentGuessWord.ToUpper()}`");
        await this._gameLogic.AnalyzeGuess(_keyboardLettersToUpdate, notificationScript);
        UpdateKeyboardLettersState();
    }

    public void OnResetGamePressed()
    {
        ResetGame();
    }


    private void UpdateKeyboardLettersState()
    {
        foreach (var letter in _keyboardLettersToUpdate)
        {
            _keyboardButtons[letter.CurrentLetter].LetterData.State = letter.State;
        }
    }

    private void HandleServerReply(string serverReplyString)
    {
        Debug.Log($"GameManger received server reply, {serverReplyString} ");
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application is about to quit...");
        if (_gameMode == GameMode.Online)
        {
            Debug.Log("We are in an online mode... Sending disconnect notification to server");
            var requestStruct = new CommunicationStruct()
            {
                RequestId        = 999,
                PlayerIdentifier = _playerIdentifier,
                RequestCommand   = NetworkClientRequestCommandCodes.REQUEST_DISCONNECT,
                Payload          = "DONTCARE"
            };
            _tcpClient.SendMessage(requestStruct.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}