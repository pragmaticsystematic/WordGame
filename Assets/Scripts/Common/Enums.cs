namespace Common
{
    /// <summary>
    /// Represents the state of a letter.
    /// Empty means the letter contains an empty char (char.MinValue)
    /// Unchecked means the letter wasn't checked against the target word yet.
    /// LetterDoesNotExistInWord,LetterExistInWordButNotInOrder,LetterExistsInWordAndInOrder are self explanatory.
    /// </summary>
    public enum LetterState
    {
        Empty,
        Unchecked,
        LetterDoesNotExistInWord,
        LetterExistInWordButNotInOrder,
        LetterExistsInWordAndInOrder
    }

    public enum GameMode
    {
        Offline,
        Online
    }
}