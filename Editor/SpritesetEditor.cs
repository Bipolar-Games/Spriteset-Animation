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

        public override bool HasPreviewGUI() => false;

        private int previewStartTime;
        private int previewFrameIndex;
        private Texture2D previewTexture;

        public override void OnInteractivePreviewGUI(Rect previewRect, GUIStyle background)
        {
            if (previewStartTime == 0)
                previewStartTime = (int)EditorApplication.timeSinceStartup;

            var spriteRowsArrayProperty = serializedObject.FindProperty(AnimationsPropertyName);
            int spriteRowsCount = spriteRowsArrayProperty.arraySize;
            int totalFrameCount = 0;
            for (int i = 0; i < spriteRowsCount; i++)
            {
                var row = spriteRowsArrayProperty.GetArrayElementAtIndex(i);
                var sprites = row.FindPropertyRelative("sprites");
                totalFrameCount += sprites.arraySize;
            }

            if (totalFrameCount <= 0)
                return;

            int frameIndex = ((int)EditorApplication.timeSinceStartup - previewStartTime) % totalFrameCount;
            if (previewFrameIndex != frameIndex)
            {
                previewFrameIndex = frameIndex;
                int startFrame = 0;
                for (int i = 0; i < spriteRowsCount; i++)
                {
                    var row = spriteRowsArrayProperty.GetArrayElementAtIndex(i);
                    var sprites = row.FindPropertyRelative("sprites");
                    if (previewFrameIndex < startFrame + sprites.arraySize)
                    {
                        int spriteIndex = previewFrameIndex - startFrame;
                        if (sprites.GetArrayElementAtIndex(spriteIndex)?.objectReferenceValue is Sprite sprite)
                        {
                            previewTexture = AssetPreview.GetAssetPreview(sprite.texture);
                        }
                        
                        break;
                    }
                    startFrame += sprites.arraySize;
                }

            }

            if (previewTexture)
                EditorGUI.DrawTextureTransparent(previewRect, previewTexture);
        }
    }
}
