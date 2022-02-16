using Common;

namespace Backend
{
    public class GameLogic
    {
        public GameBoard GameBoard;

        private string _currentWordToGuess = "";

        public GameLogic()
        {
            GameBoard = new GameBoard();
        }

        public void ResetGame()
        {
            this.GameBoard.ResetBoard();
            GenerateNewWordToGuess();
        }

        private void GenerateNewWordToGuess()
        {
            //todo generate from dictionary
            // we expect all words to be lower letters.
            this._currentWordToGuess = "route";
        }

        public bool analyzeGuess()
        {
            bool     success     = false;
            WordData wordToCheck = this.GameBoard.GetCurrentWord();

            if (wordToCheck.IsWordFilled())
            {
                for (int i = 0; i < wordToCheck.LetterList.Count; i++)
                {
                    LetterData letter = wordToCheck.LetterList[i];
                    //letter exists in the word
                    if (_currentWordToGuess.Contains(letter.CurrentLetter.ToString()))
                    {
                        //letter exist in the word AND it's in the correct place
                        if (_currentWordToGuess[i] == letter.CurrentLetter)
                        {
                            letter.State = LetterState.LetterExistsInWordAndInOrder;
                        }
                        //letter exist in word but not in the correct place.
                        else
                        {
                            letter.State = LetterState.LetterExistInWordButNotInOrder;
                        }
                    }
                    //letter does not exist in the word at all.
                    else
                    {
                        letter.State = LetterState.LetterDoesNotExistInWord;
                    }
                }

                success = true;
            }

            return success;
        }
    }
}