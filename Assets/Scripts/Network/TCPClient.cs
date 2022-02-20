using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Network
{
    public class TcpClient
    {
        #region private members

        private System.Net.Sockets.TcpClient _socketConnection;
        private Thread                       _clientReceiveThread;
        private string                       _ipAddress;
        private int                          _port;
        private List<string>                 _messageQueue;

        #endregion

        //delegates

        public delegate void OnSeverReplyReceivedDelegate(string serverReply);

        public event OnSeverReplyReceivedDelegate OnSeverReplyReceived;

        public delegate void OnConnectionStatusChangedDelegate(ConnectionStatus connectionStatus);

        public event OnConnectionStatusChangedDelegate OnConnectionStatusChanged;


        public TcpClient(string ipAddress, int port)
        {
            _ipAddress    = ipAddress;
            _port         = port;
            _messageQueue = new List<string>();
        }

        public void CloseConnection()
        {
            _clientReceiveThread.Abort();
        }

        public bool IsConnected()
        {
            return _socketConnection != null && _socketConnection.Connected;
        }


        /// <summary> 	
        /// Setup socket connection. 	
        /// </summary> 	
        public void ConnectToTcpServer()
        {
            try
            {
                _clientReceiveThread = new Thread(new ThreadStart(ListenForData)) {IsBackground = true};
                _clientReceiveThread.Start();
            }
            catch (Exception e)
            {
                Debug.Log("On client connect exception " + e);
            }
        }

        /// <summary> 	
        /// Runs in background clientReceiveThread; Listens for incomming data. 	
        /// </summary>     
        private void ListenForData()
        {
            try
            {
                _socketConnection = new System.Net.Sockets.TcpClient(_ipAddress, _port);
                OnConnectionStatusChanged?.Invoke(ConnectionStatus.Connected);
                if (_messageQueue.Count > 0)
                {
                    Debug.Log("Sending messages that were accumulated before the connection was made...");
                    foreach (var message in _messageQueue)
                    {
                        SendMessage(message);
                    }

                    Debug.Log("Clearing message queue...");
                    _messageQueue.Clear();
                }

                var bytes = new byte[1024];
                while (true)
                {
                    // Get a stream object for reading 				
                    using (NetworkStream stream = _socketConnection.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            // Convert byte array to string message. 						
                            var serverMessage = Encoding.ASCII.GetString(incomingData);
                            Debug.Log("server message received as: " + serverMessage);
                            OnSeverReplyReceived?.Invoke(serverMessage);
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
                OnConnectionStatusChanged?.Invoke(ConnectionStatus.NotConnected);
            }
        }

        /// <summary> 	
        /// Send message to server using socket connection. 	
        /// </summary> 	
        public void SendMessage(string messageToSend)
        {
            if (_socketConnection == null)
            {
                Debug.Log("Trying to send a message through unconnected socket! Putting it in queue!");
                _messageQueue.Add(messageToSend);
                return;
            }

            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = _socketConnection.GetStream();
                if (stream.CanWrite)
                {
                    string clientMessage = messageToSend;
                    // Convert string message to byte array.                 
                    byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                    // Write byte array to socketConnection stream.                 
                    stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                    Debug.Log("Client sent his message - should be received by server");
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }
    }
}