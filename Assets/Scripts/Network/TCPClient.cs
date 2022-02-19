using System;
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

        private System.Net.Sockets.TcpClient socketConnection;
        private Thread                       _clientReceiveThread;
        private IPAddress                    _ipAddress;
        private int                          _port;

        #endregion

        //delegates

        public delegate void OnSeverReplyReceivedDelegate(string serverReply);

        public event OnSeverReplyReceivedDelegate OnSeverReplyReceived;


        public TcpClient(string ipAddress, int port)
        {
            _ipAddress = IPAddress.Parse(ipAddress);
            _port      = port;
            ConnectToTcpServer();
        }

        // // Use this for initialization 	
        // void Start()
        // {
        //     ConnectToTcpServer();
        // }

        // // Update is called once per frame
        // void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.Space))
        //     {
        //         SendMessage();
        //     }
        // }

        /// <summary> 	
        /// Setup socket connection. 	
        /// </summary> 	
        private void ConnectToTcpServer()
        {
            try
            {
                _clientReceiveThread              = new Thread(new ThreadStart(ListenForData));
                _clientReceiveThread.IsBackground = true;
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
                socketConnection = new System.Net.Sockets.TcpClient("127.0.0.1", 13000);
                var bytes = new byte[1024];
                while (true)
                {
                    // Get a stream object for reading 				
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            // Convert byte array to string message. 						
                            string serverMessage = Encoding.ASCII.GetString(incommingData);
                            Debug.Log("server message received as: " + serverMessage);
                            OnSeverReplyReceived?.Invoke(serverMessage);
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log("Socket exception: " + socketException);
            }
        }

        /// <summary> 	
        /// Send message to server using socket connection. 	
        /// </summary> 	
        public void SendMessage(string messageToSend)
        {
            if (socketConnection == null)
            {
                return;
            }

            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = socketConnection.GetStream();
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