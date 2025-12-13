using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using _Project.Scripts.Utilities;

namespace Editor
{
    // --- VẼ [GReadOnly] ---
    [CustomPropertyDrawer(typeof(Attributes.GReadOnlyAttribute))]
    public class GReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false; // Khóa tương tác
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;  // Mở lại
        }
    }

    // --- VẼ [GHeader] ---
    [CustomPropertyDrawer(typeof(Attributes.GHeaderAttribute))]
    public class GHeaderDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            Attributes.GHeaderAttribute attr = (Attributes.GHeaderAttribute)attribute;
            
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            Color color;
            if (ColorUtility.TryParseHtmlString(attr.ColorHex, out color))
                style.normal.textColor = color;
            
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 13;

            // Vẽ nền tối nhẹ cho đẹp
            Rect bgRect = position;
            bgRect.y += 2;
            bgRect.height -= 2;
            EditorGUI.DrawRect(bgRect, new Color(0.1f, 0.1f, 0.1f, 0.5f));

            EditorGUI.LabelField(position, attr.Text, style);
        }

        public override float GetHeight() => 30f;
    }

    // --- VẼ [GLine] ---
    [CustomPropertyDrawer(typeof(Attributes.GLineAttribute))]
    public class GLineDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            Attributes.GLineAttribute lineAttr = (Attributes.GLineAttribute)attribute;

            // Draw the line in the middle of the allocated space.
            Rect lineRect = new Rect(position.x, position.y + (position.height - lineAttr.Height) / 2, position.width, lineAttr.Height);
            EditorGUI.DrawRect(lineRect, new Color(0.5f, 0.5f, 0.5f, 1f));
        }

        public override float GetHeight()
        {
            Attributes.GLineAttribute lineAttr = (Attributes.GLineAttribute)attribute;
            // Return height of the line plus some padding.
            return lineAttr.Height + 8f;
        }
    }

    // --- VẼ [GButton] - CUSTOM EDITOR CHO MONOBEHAVIOUR ---
    // Cái này hơi đặc biệt, nó sẽ can thiệp vào mọi MonoBehaviour để tìm hàm có [GButton]
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class GravityMonobehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Vẽ Inspector mặc định trước
            DrawDefaultInspector();

            // Tìm các hàm có [GButton]
            var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                var attr = Attribute.GetCustomAttribute(method, typeof(Attributes.GButtonAttribute)) as Attributes.GButtonAttribute;
                if (attr != null)
                {
                    string label = string.IsNullOrEmpty(attr.ButtonLabel) ? method.Name : attr.ButtonLabel;
                    
                    GUILayout.Space(10);
                    GUI.backgroundColor = new Color(0f, 1f, 0.5f); // Màu xanh Gravity
                    if (GUILayout.Button(label, GUILayout.Height(30)))
                    {
                        method.Invoke(target, null);
                    }
                    GUI.backgroundColor = Color.white;
                }
            }
        }
    }
}