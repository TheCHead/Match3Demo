using UnityEngine;

namespace Scripts.DataModels
{
    [CreateAssetMenu(fileName = "GemType", menuName = "Match3/GemType")]
    public class GemTypeSO : ScriptableObject
    {
        public GemType gemType;
        public Sprite sprite;
    }

    public enum GemType { Unknown, Red, Green, Blue, Yellow }
}

