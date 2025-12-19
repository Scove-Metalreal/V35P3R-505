using UnityEngine;

namespace _Project.Scripts.Utilities
{
    public class U_StickyNote : MonoBehaviour
    {
        [TextArea] public string noteText = "Enter note here...";
        public Color noteColor = Color.yellow;
        public bool showAlways = true; // Show even when not selected
    }
}