using UnityEngine;

namespace Common
{
    public class TileColors : MonoBehaviour
    {
        [SerializeField] public Color letterColorUnchecked;
        [SerializeField] public Color letterColorLetterNotExist;
        [SerializeField] public Color letterColorExistWrongPlace;
        [SerializeField] public Color letterColorExistCorrectPlace;
    }
}