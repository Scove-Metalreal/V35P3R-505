using _Project.Scripts.Utilities;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(U_StickyNote))]
public class StickyNoteEditor : UnityEditor.Editor
{
    // Draw the text in the scene view
    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Active)]
    static void DrawGizmo(U_StickyNote note, GizmoType gizmoType)
    {
        if (!note.showAlways && (gizmoType & GizmoType.Selected) == 0) return;

        Vector3 position = note.transform.position;

        // Draw an Icon (optional, looks nice)
        Gizmos.color = note.noteColor;
        Gizmos.DrawSphere(position, 0.2f);

        // Define Text Style
        GUIStyle style = new GUIStyle();
        style.normal.textColor = note.noteColor;
        style.fontSize = 12;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;

        // Draw Text
        Handles.Label(position + Vector3.up * 0.5f, note.noteText, style);
    }
}