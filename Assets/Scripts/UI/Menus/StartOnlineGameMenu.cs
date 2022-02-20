using System;
using System.Text.RegularExpressions;
using DefaultNamespace.Menus.MenuElements;
using UnityEngine;

namespace DefaultNamespace.Menus
{
    public class StartOnlineGameMenu : MonoBehaviour
    {
        [Tooltip("Input field that's responsible for getting the IP and port from the user.")] [SerializeField]
        private MenuInputField ipAndPortInput;

        [Tooltip("Input field that's responsible for getting the player name")] [SerializeField]
        private MenuInputField playerNameInput;

        [Tooltip("Notification GUI object")] [SerializeField]
        private NotificationScript notificationScript;

        private readonly Regex _ipRegex   = new Regex("(\\d+.\\d+.\\d+.\\d+):(\\d+)");
        private readonly Regex _nameRegex = new Regex("^[a-zA-Z0-9_]+$");

        private string _parsedIp;
        private int    _parsedPort;
        private string _playerName;


        public delegate void OnAttemptStartOnlineGameDelegate(string ip, int port, string playerName);

        public event OnAttemptStartOnlineGameDelegate OnAttemptStartOnlineGame;

        private void Start()
        {
            ipAndPortInput.SetPlaceHolderText("127.0.0.1:13000");
            playerNameInput.SetPlaceHolderText("JONSNOW");
        }


        private bool ValidateInput()
        {
            var isInputValid = true;
            var matchIp      = _ipRegex.Match(ipAndPortInput.GetInputText());
            var matchName    = _nameRegex.Match(playerNameInput.GetInputText());

            if (!matchIp.Success)
            {
                notificationScript.DisplayNotification("IP doesn't look right! Please enter a valid IP and port!");
                isInputValid = false;
            }


            if (!matchName.Success)
            {
                notificationScript.DisplayNotification("Please only use a single word for your name.");
                isInputValid = false;
            }

            if (isInputValid)
            {
                this._parsedIp   = matchIp.Groups[1].Value;
                this._parsedPort = int.Parse(matchIp.Groups[2].Value);
                this._playerName = playerNameInput.GetInputText();
            }

            return isInputValid;
        }


        public void OnConnectClicked()
        {
            if (ValidateInput())
            {
                ipAndPortInput.SetPlaceHolderText(ipAndPortInput.GetInputText());
                playerNameInput.SetPlaceHolderText(playerNameInput.GetInputText());
                OnAttemptStartOnlineGame?.Invoke(_parsedIp, _parsedPort, _playerName);
            }
        }
    }
}