using System;
using UnityEditor;
using UnityEngine;

namespace Bipolar.SpritesetAnimation.Editor
{
    [CustomEditor(typeof(Spriteset))]
    public class SpritesetEditor : UnityEditor.Editor
    {
        private const string AnimationsPropertyName = "animations";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var spritesProperty = serializedObject.FindProperty(AnimationsPropertyName);

            int spritesCount = spritesProperty.arraySize;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            bool hasChanged = false; // DrawSpritesGrid(columnCount, spritesCount, spritesProperty.GetArrayElementAtIndex);
            if (hasChanged)
                serializedObject.ApplyModifiedProperties();
            
        }

        public static bool DrawSpritesGrid(int columnCount, int spritesCount, Func<int, SerializedProperty> getSpriteFunc)
        {
            bool hasChanged = false;
            for (int rowIndex = 0, spriteIndex = 0; spriteIndex < spritesCount; rowIndex++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int columnIndex = 0; columnIndex < columnCount && spriteIndex < spritesCount; columnIndex++, spriteIndex++)
                {
                    var spriteProperty = getSpriteFunc(spriteIndex);
                    var sprite = EditorGUILayout.ObjectField(GUIContent.none,
                        spriteProperty.objectReferenceValue, typeof(Sprite), allowSceneObjects: false, GUILayout.Width(66));
                    if (sprite != spriteProperty.objectReferenceValue)
                    {
                        spriteProperty.objectReferenceValue = sprite;
                        hasChanged = true;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            }

            return hasChanged;
        }

        public override bool HasPreviewGUI() => true;

        private int previewStartTime;
        private int previewFrameIndex;
        private Texture2D previewTexture;
        public override void OnInteractivePreviewGUI(Rect previewRect, GUIStyle background)
        {
            if (previewStartTime == 0)
                previewStartTime = (int)EditorApplication.timeSinceStartup;

            var spritesProperty = serializedObject.FindProperty(AnimationsPropertyName);
            int spritesCount = spritesProperty.arraySize;
            if (spritesCount == 0)
				return;

            int frameIndex = ((int)EditorApplication.timeSinceStartup - previewStartTime) % spritesCount;
            if (previewFrameIndex != frameIndex)
            {
                previewFrameIndex = frameIndex;
                var sprite = (Sprite)spritesProperty.GetArrayElementAtIndex(previewFrameIndex).objectReferenceValue;
                previewTexture = AssetPreview.GetAssetPreview(sprite);
            }

            if (previewTexture)
                EditorGUI.DrawTextureTransparent(previewRect, previewTexture);
        }
    }
}
