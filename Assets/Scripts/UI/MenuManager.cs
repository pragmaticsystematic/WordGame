using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class MenuManager : MonoBehaviour
    {
        public GameObject mainMenu;
        public GameObject gameScreen;
        public GameObject gameOverScreen;
        public GameObject notificationMessage;

        [SerializeField] private Text gameOverTextElement;

        private void Start()
        {
            gameScreen.SetActive(false);
            gameOverScreen.SetActive(false);
            mainMenu.SetActive(true);
        }


        //Delegates

        public delegate void OnRestartGameDelegate();

        public event OnRestartGameDelegate OnRestartGame;


        public void StartGame()
        {
            mainMenu.SetActive(false);
            gameScreen.SetActive(true);
            OnRestartGame?.Invoke();
        }

        public void RestartGame()
        {
            gameOverScreen.SetActive(false);
            gameScreen.SetActive(true);
            OnRestartGame?.Invoke();
        }

        public void GameOver(string gameOverMessage)
        {
            gameScreen.SetActive(false);
            gameOverTextElement.text = gameOverMessage;
            gameOverScreen.SetActive(true);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}