using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using DefaultNamespace.Menus;
using Network;
using Network.NetworkCodes;
using UnityEngine;

namespace Backend
{
    public class GameLogicRemote : GameLogic
    {
        private readonly TcpClient                            _tcpClient;
        private readonly string                               _playerIdentifier;
        private readonly Dictionary<int, CommunicationStruct> _requestDictionary;
        private          int                                  _requestId;
        private          string                               _wordToGuess;

        public GameLogicRemote(TcpClient tcpClient, string playerIdentifier) : base()
        {
            _tcpClient         = tcpClient;
            _playerIdentifier  = playerIdentifier;
            _requestDictionary = new Dictionary<int, CommunicationStruct>();


            _tcpClient.OnSeverReplyReceived -= ServerReply;
            _tcpClient.OnSeverReplyReceived += ServerReply;
        }

        public override async Task<bool> AnalyzeGuess(List<LetterData>   updatedLetters,
                                                      NotificationScript notificationScript)
        {
            var wordToGuessString = GameBoard.GetCurrentWord().GetWordString();
            var success           = false;
            var wordToGuessData   = GameBoard.GetCurrentWord();

            if (wordToGuessData.IsWordFilled())
            {
                var isWordLegit = await IsLegitimateWord(wordToGuessString);
                if (isWordLegit)
                {
                    _requestId += 1;
                    var requestStruct = new CommunicationStruct()
                    {
                        RequestCommand   = NetworkClientRequestCommandCodes.EVALUATE_GUESS,
                        PlayerIdentifier = _playerIdentifier,
                        Payload          = wordToGuessString
                    };

                    var reply = await SendMessageToServerAndWaitUntilServerReplyArrives(requestStruct, 10);
                    if (reply.RequestCommand == NetworkServerReplyCommandCodes.REQUEST_SUCCESS)
                    {
                        var evaluationResults = reply.Payload;
                        for (var i = 0; i < evaluationResults.Length; i++)
                        {
                            switch (evaluationResults[i])
                            {
                                case LetterEvaluationCodes.LETTER_DOESNT_EXIST_IN_WORD:
                                    wordToGuessData.LetterList[i].State = LetterState.LetterDoesNotExistInWord;
                                    break;
                                case LetterEvaluationCodes.LETTER_EXISTS_IN_WORD_BUT_NOT_IN_ORDER:
                                    wordToGuessData.LetterList[i].State = LetterState.LetterExistInWordButNotInOrder;
                                    break;
                                case LetterEvaluationCodes.LETTER_EXISTS_IN_WORD_AND_IN_CORRECT_PLACE:
                                    wordToGuessData.LetterList[i].State = LetterState.LetterExistsInWordAndInOrder;
                                    break;
                                case LetterEvaluationCodes.SOMETHING_WENT_WRONG_YOU_SHOULD_PANIK:
                                    Debug.Log("ERROR IN SERVER WORD EVALUATION!");
                                    break;
                                default:
                                    Debug.Log(
                                        $"Unfamiliar evaluation case! {evaluationResults[i]} - https://c.tenor.com/QcuvpD6BFacAAAAC/meme-man-panik.gif");
                                    break;
                            }

                            updatedLetters.Add(wordToGuessData.LetterList[i]); //used to color the keyboard letters.
                        }

                        if (CheckWinTemp(wordToGuessData))
                        {
                            InvokeGameWon(
                                $"Congratulations! You guessed the word '{wordToGuessData.GetWordString().ToUpper()} !'");
                        }
                        else
                        {
                            NumberOfTriesLeft -= 1;
                            if (NumberOfTriesLeft == 0)
                            {
                                InvokeGameWon(
                                    $"Sorry, You failed to guess the word '{_wordToGuess}'. Don't worry! You'll get it next time.'");
                            }
                            else
                            {
                                Debug.Log($"You have {NumberOfTriesLeft} tries left");
                            }

                            //Move to the next row in the game grid.
                            GameBoard.CurrentWordIndex += 1;
                            success                    =  true;
                        }
                    }
                }
                else // Word does not exist in our dictionary.
                {
                    Debug.Log($"Word: {wordToGuessData} Does not exist in the dictionary!");
                    notificationScript.DisplayNotification(
                        $"The word {wordToGuessData.GetWordString()} Doesn't exist in the dictionary!");
                }
            }
            else // Word length is not correct.
            {
                notificationScript.DisplayNotification($"The word is not long enough!");
            }


            return success;
        }

        private bool CheckWinTemp(WordData guessWordData)
        {
            var letters =
                guessWordData.LetterList.TakeWhile(letterData =>
                                                       letterData.State == LetterState.LetterExistsInWordAndInOrder)
                             .ToList();
            return letters.Count == 5;
        }

        public override async Task ResetGame()
        {
            await base.ResetGame();
            await TellServerToChooseNewWordForPlayer();
        }


        private void ServerReply(string serverReply)
        {
            Debug.Log($"GameLogicRemote Received reply from server: {serverReply}");
            var communicationStruct = new CommunicationStruct();

            //todo throw exception if this doesn't work!
            communicationStruct.FromString(serverReply);

            _requestDictionary[communicationStruct.RequestId] = communicationStruct;
        }

        private async Task TellServerToChooseNewWordForPlayer()
        {
            var communicationStruct = new CommunicationStruct()
            {
                RequestCommand   = NetworkClientRequestCommandCodes.GUESS_NEW_WORD,
                PlayerIdentifier = _playerIdentifier,
                Payload          = "DONTCARE"
            };

            var reply = await SendMessageToServerAndWaitUntilServerReplyArrives(communicationStruct, 10);

            if (reply.RequestCommand == NetworkServerReplyCommandCodes.REQUEST_SUCCESS)
            {
                Debug.Log("Server successfully picked a new word for the player!");
                _wordToGuess = reply.Payload;
            }
        }

        private async Task<bool> IsLegitimateWord(string wordToCheck)
        {
            var requestStruct = new CommunicationStruct()
            {
                RequestCommand   = NetworkClientRequestCommandCodes.CHECK_WORD_EXISTS,
                PlayerIdentifier = _playerIdentifier,
                Payload          = wordToCheck
            };
            var result = false;

            var replyStruct = await SendMessageToServerAndWaitUntilServerReplyArrives(requestStruct, 10);
            if (replyStruct.RequestCommand == NetworkServerReplyCommandCodes.REQUEST_SUCCESS)
            {
                result = bool.Parse(replyStruct.Payload);
            }

            return result;
        }

        //todo: implement timeout for this function
        private async Task<CommunicationStruct> SendMessageToServerAndWaitUntilServerReplyArrives(
            CommunicationStruct messageToSend, int timeoutSeconds)
        {
            var timeoutCounter = 0;
            var isTimeout      = false;

            //Each time we send a request we increment the request id counter.
            //so that each request will be uniquely identifiable. 
            messageToSend.RequestId = _requestId += 1;
            _requestDictionary.Add(_requestId, null);

            _tcpClient.SendMessage(messageToSend.ToString());
            CommunicationStruct replyStruct     = null;
            var                 shouldBreakLoop = false;
            while (!shouldBreakLoop && !isTimeout)
            {
                replyStruct = _requestDictionary[messageToSend.RequestId];
                if (replyStruct != null)
                {
                    Debug.Log($"Got Server Reply for Request id: {replyStruct.RequestId}");

                    shouldBreakLoop = true;
                }

                Debug.Log("Waiting for server Reply!");
                await Task.Delay(100);
                timeoutCounter += 1;

                if (timeoutCounter * 10 >= timeoutSeconds * 1000)
                {
                    Debug.Log("Reached timeout while waiting for reply... exiting");
                    isTimeout = true;
                }
            }


            // var replyCopy = new CommunicationStruct(replyStruct);

            // removed the received request from the requests dictionary.
            if (!isTimeout)
            {
                _requestDictionary.Remove(replyStruct.RequestId);
            }

            return replyStruct;
        }
    }
}