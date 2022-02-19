using UnityEngine;

namespace Common
{
    /// <summary>
    /// Class that houses the colors used in the game for coloring the letters and keyboard keys backgrounds.
    /// </summary>
    public class TileColors : MonoBehaviour
    {
        [SerializeField] public Color letterColorUnchecked;
        [SerializeField] public Color letterColorLetterNotExist;
        [SerializeField] public Color letterColorExistWrongPlace;
        [SerializeField] public Color letterColorExistCorrectPlace;
    }
}