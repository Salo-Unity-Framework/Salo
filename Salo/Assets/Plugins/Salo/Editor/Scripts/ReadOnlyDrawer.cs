using UnityEditor;
using UnityEngine;

namespace Salo.Infrastructure.EditorExtensions
{
    [CustomPropertyDrawer(typeof(InspectorReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
