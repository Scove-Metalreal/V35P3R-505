using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TransformClipboard
    {
        private static Vector3? storedPosition;
        private static Quaternion? storedRotation;
        private static Vector3? storedScale;

        // --- COPY ---
        [MenuItem("CONTEXT/Transform/Copy Position Only")]
        static void CopyPos(MenuCommand command) => storedPosition = ((Transform)command.context).position;

        [MenuItem("CONTEXT/Transform/Copy Rotation Only")]
        static void CopyRot(MenuCommand command) => storedRotation = ((Transform)command.context).rotation;

        [MenuItem("CONTEXT/Transform/Copy Scale Only")]
        static void CopyScale(MenuCommand command) => storedScale = ((Transform)command.context).localScale;


        // --- PASTE ---
        [MenuItem("CONTEXT/Transform/Paste Position Only")]
        static void PastePos(MenuCommand command)
        {
            if (storedPosition.HasValue)
            {
                Transform t = (Transform)command.context;
                Undo.RecordObject(t, "Paste Position");
                t.position = storedPosition.Value;
            }
        }

        [MenuItem("CONTEXT/Transform/Paste Rotation Only")]
        static void PasteRot(MenuCommand command)
        {
            if (storedRotation.HasValue)
            {
                Transform t = (Transform)command.context;
                Undo.RecordObject(t, "Paste Rotation");
                t.rotation = storedRotation.Value;
            }
        }

        [MenuItem("CONTEXT/Transform/Paste Scale Only")]
        static void PasteScale(MenuCommand command)
        {
            if (storedScale.HasValue)
            {
                Transform t = (Transform)command.context;
                Undo.RecordObject(t, "Paste Scale");
                t.localScale = storedScale.Value;
            }
        }

        [MenuItem("CONTEXT/Transform/Reset Position To Zero")]
        static void ResetPosition(MenuCommand command)
        {
            Transform t = (Transform)command.context;
            Undo.RecordObject(t, "Reset Position");
            t.localPosition = Vector3.zero;
        }
    }
}