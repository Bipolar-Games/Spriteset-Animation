using UnityEditor;
using UnityEngine;

namespace Bipolar.SpritesetAnimation.Editor
{
	//[CustomPropertyDrawer(typeof(SpritesetRow))]
    public class SpritesetRowDrawer : PropertyDrawer
    {
        private const string innerArrayPropertyName = "sprites";
        private const string namePropertyName = "name";
		private const int arraySizeRectWidth = 48;
		private static readonly GUIContent empty = new GUIContent(null, null, null);

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
            if (property.isExpanded == false)
                return EditorGUIUtility.singleLineHeight;
            
            var arrayProperty = property.FindPropertyRelative(innerArrayPropertyName);
            return EditorGUI.GetPropertyHeight(arrayProperty, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
            EditorGUI.BeginProperty(position, label, property);

            var arrayProperty = property.FindPropertyRelative(innerArrayPropertyName);
            
            var labelRect = position;
            labelRect.xMin += 4;
            labelRect.xMax -= arraySizeRectWidth + 1;
            labelRect.height = EditorGUIUtility.singleLineHeight;
			property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(labelRect, property.isExpanded, label);

            var count = arrayProperty.arraySize;
            var countRect = position;
            countRect.xMin = countRect.xMax - arraySizeRectWidth;
            countRect.height = EditorGUIUtility.singleLineHeight;
			arrayProperty.arraySize = EditorGUI.IntField(countRect, count);

			EditorGUI.indentLevel++;
            //EditorGUI.PropertyField(position, arrayProperty, label);
            EditorGUI.indentLevel--;

            var nameProperty = property.FindPropertyRelative(namePropertyName);
            Rect nameRect = position;
            nameRect.height = EditorGUIUtility.singleLineHeight;
            nameRect.xMin += 32;
            nameRect.xMax -= 32;
            //EditorGUI.PropertyField(nameRect, nameProperty, empty);

            EditorGUI.EndFoldoutHeaderGroup();
            EditorGUI.EndProperty();
        }
	}
}
