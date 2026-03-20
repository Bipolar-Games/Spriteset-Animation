#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Bipolar.SpritesetAnimation
{
    internal class SpritesetGeneration
	{
        private const string GenerateMenuItemName = "Assets/" + Paths.Root + "Generate Spriteset";

        [MenuItem(GenerateMenuItemName, isValidateFunction: true)]
        private static bool CanGenerate()
        {
            return Selection.objects.All(obj => obj is Texture);
        }

        [MenuItem(GenerateMenuItemName)]
        private static void Generate()
        {
            foreach (var selection in Selection.objects)
                if (selection is Texture texture)
                    CreateSpritesetFromTexture(texture);

            AssetDatabase.SaveAssets();
        }

        private static void CreateSpritesetFromTexture(Texture texture)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            var allSprites = AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<Sprite>().ToArray();

            float currentY = float.PositiveInfinity;
            var rows = new List<SpritesetRow>();
            var rowSprites = new List<Sprite>();
            for (int i = 0; i <= allSprites.Length; i++)
            {
                bool afterLastSprite = i == allSprites.Length;
                if (afterLastSprite || allSprites[i].rect.y < currentY)
                {
                    if (rowSprites.Count > 0)
                    {
                        var row = new SpritesetRow
                        {
                            name = texture.name + " Row " + rows.Count,
                            sprites = rowSprites.ToArray()
                        };
                        rows.Add(row);
                        rowSprites.Clear();
                    }
                    if (afterLastSprite == false)
                        currentY = allSprites[i].rect.y;
                }

                if (afterLastSprite == false)
                    rowSprites.Add(allSprites[i]);
            }

            var folder = System.IO.Path.GetDirectoryName(path);
            var spriteset = ScriptableObject.CreateInstance<Spriteset>();
            spriteset.animations = rows.ToArray();
            spriteset.name = texture.name + " Spriteset";
            AssetDatabase.CreateAsset(spriteset, folder + "/" + spriteset.name + ".asset");
        }
    }
}
#endif
