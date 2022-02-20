using System;
using DefaultNamespace.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public enum MenuState
    {
        MainMenu,
        RequestOnlineGameMenu,
        GameScreen,
        GameOverMenu,
        FailedToConnectToServer
    }

    public class MenuManager : MonoBehaviour
    {
        [Tooltip("The main Menu of the game")] public GameObject mainMenu;
        [Tooltip("The UI of the game itself")] public GameObject gameScreen;

        [Tooltip("Menu that's show when the game is over")]
        public GameObject gameOverScreen;

        public GameObject notificationMessage;

        [Tooltip("Menu that's responsible for initiating an online game")]
        public GameObject startOnlineGameMenu;

        [Tooltip("Menu that's shown when failing to connect to server")]
        public GameObject failedToConnectToServerMenu;

        private StartOnlineGameMenu _startOnlineGameMenuScript;

        [Tooltip("The GUI element for the game over text that's shown at the GAME OVER screen")] [SerializeField]
        private Text gameOverTextElement;

        private MenuState _menuState;

        private void Start()
        {
            _startOnlineGameMenuScript = startOnlineGameMenu.GetComponent<StartOnlineGameMenu>();
            _menuState                 = MenuState.MainMenu;
            HandleStateChange();

            _startOnlineGameMenuScript.OnAttemptStartOnlineGame -= StartOnlineGame;
            _startOnlineGameMenuScript.OnAttemptStartOnlineGame += StartOnlineGame;
        }


        private void HandleStateChange()
        {
            switch (_menuState)
            {
                case MenuState.MainMenu:
                    gameScreen.SetActive(false);
                    gameOverScreen.SetActive(false);
                    startOnlineGameMenu.SetActive(false);
                    mainMenu.SetActive(true);
                    failedToConnectToServerMenu.SetActive(false);
                    break;
                case MenuState.RequestOnlineGameMenu:
                    gameScreen.SetActive(false);
                    gameOverScreen.SetActive(false);
                    startOnlineGameMenu.SetActive(true);
                    mainMenu.SetActive(false);
                    failedToConnectToServerMenu.SetActive(false);
                    break;
                case MenuState.GameScreen:
                    gameScreen.SetActive(true);
                    gameOverScreen.SetActive(false);
                    startOnlineGameMenu.SetActive(false);
                    mainMenu.SetActive(false);
                    failedToConnectToServerMenu.SetActive(false);
                    break;
                case MenuState.GameOverMenu:
                    gameScreen.SetActive(false);
                    gameOverScreen.SetActive(true);
                    startOnlineGameMenu.SetActive(false);
                    mainMenu.SetActive(false);
                    failedToConnectToServerMenu.SetActive(false);
                    break;
                case MenuState.FailedToConnectToServer:
                    gameScreen.SetActive(false);
                    gameOverScreen.SetActive(false);
                    startOnlineGameMenu.SetActive(false);
                    mainMenu.SetActive(false);
                    failedToConnectToServerMenu.SetActive(true);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        //Delegates

        public delegate void OnRestartGameDelegate();

        public event OnRestartGameDelegate OnRestartGame;

        public delegate void OnStartOnlineGameDelegate(string ip, int port, string playerName);

        public event OnStartOnlineGameDelegate OnStartOnlineGame;

        public delegate void OnStartOfflineGameDelegate();

        public event OnStartOfflineGameDelegate OnStartOfflineGame;


        public void StartGame()
        {
            _menuState = MenuState.GameScreen;
            HandleStateChange();
            OnStartOfflineGame?.Invoke();
        }

        public void ShowStartOnlineGameMenu()
        {
            _menuState = MenuState.RequestOnlineGameMenu;
            HandleStateChange();
        }

        public void BackToTitle()
        {
            _menuState = MenuState.MainMenu;
            HandleStateChange();
        }

        public void ShowFailedToConnectToServerMenu()
        {
            _menuState = MenuState.FailedToConnectToServer;
            HandleStateChange();
        }

        public void StartOnlineGame(string ip, int port, string playerName)
        {
            _menuState = MenuState.GameScreen;
            HandleStateChange();
            OnStartOnlineGame?.Invoke(ip, port, playerName);
        }

        public void RestartGame()
        {
            _menuState = MenuState.GameScreen;
            HandleStateChange();
            OnRestartGame?.Invoke();
        }

        public void GameOver(string gameOverMessage)
        {
            _menuState = MenuState.GameOverMenu;
            HandleStateChange();
            gameOverTextElement.text = gameOverMessage;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}