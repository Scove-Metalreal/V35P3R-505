using UnityEngine;
using UnityEditor;

namespace Editor
{
    public static class TransformCopier
    {
        private static Vector3? _positionClipboard;
        private static Quaternion? _rotationClipboard;
        private static Vector3? _scaleClipboard;

        // --- POSITION ---
        [MenuItem("CONTEXT/Transform/Copy Position Only")]
        public static void CopyPosition(MenuCommand command)
        {
            Transform t = (Transform)command.context;
            _positionClipboard = t.position;
            Debug.Log($"Copied Position: {t.position}");
        }

        [MenuItem("CONTEXT/Transform/Paste Position Only")]
        public static void PastePosition(MenuCommand command)
        {
            if (_positionClipboard.HasValue)
            {
                Transform t = (Transform)command.context;
                Undo.RecordObject(t, "Paste Position");
                t.position = _positionClipboard.Value;
            }
        }

        // --- ROTATION ---
        [MenuItem("CONTEXT/Transform/Copy Rotation Only")]
        public static void CopyRotation(MenuCommand command)
        {
            Transform t = (Transform)command.context;
            _rotationClipboard = t.rotation;
            Debug.Log($"Copied Rotation: {t.rotation.eulerAngles}");
        }

        [MenuItem("CONTEXT/Transform/Paste Rotation Only")]
        public static void PasteRotation(MenuCommand command)
        {
            if (_rotationClipboard.HasValue)
            {
                Transform t = (Transform)command.context;
                Undo.RecordObject(t, "Paste Rotation");
                t.rotation = _rotationClipboard.Value;
            }
        }

        // --- RESET NHANH ---
        [MenuItem("CONTEXT/Transform/Reset Position To Zero")]
        public static void ResetPosition(MenuCommand command)
        {
            Transform t = (Transform)command.context;
            Undo.RecordObject(t, "Reset Position");
            t.localPosition = Vector3.zero;
        }
    }
}