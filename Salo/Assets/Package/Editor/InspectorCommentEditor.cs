using UnityEditor;
using UnityEngine;

namespace Salo.Infrastructure.EditorExtensions
{
    [CustomEditor(typeof(InspectorComment))]
    public class InspectorCommentEditor : Editor
    {
        private InspectorComment inspectorComment;
        private GUIStyle customStyle;
        private bool isEdit = false;


        private void OnEnable()
        {
            inspectorComment = (InspectorComment)target;
        }

        public override void OnInspectorGUI()
        {
            if (customStyle == null)
            {
                customStyle = new GUIStyle(GUI.skin.box);
                customStyle.padding = new RectOffset(4, 2, 4, 4);
                customStyle.margin = new RectOffset(6, 4, 2, 2);
            }

            EditorGUILayout.LabelField("Comment:");
            EditorGUILayout.BeginHorizontal(customStyle);
            EditorGUILayout.LabelField(GetHelpBoxIcon(), GUILayout.Width(20));
            EditorGUILayout.LabelField(inspectorComment.Comment, EditorStyles.wordWrappedLabel);
            EditorGUILayout.EndHorizontal();

            // Foldout to expose the default UI - so the text can be edited
            isEdit = EditorGUILayout.BeginFoldoutHeaderGroup(isEdit, "Edit");
            if (isEdit) base.OnInspectorGUI();
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private GUIContent GetHelpBoxIcon()
        {
            Texture2D iconTexture = EditorGUIUtility.IconContent("console.infoicon.sml").image as Texture2D;
            return new GUIContent(iconTexture);
        }

    }
}
