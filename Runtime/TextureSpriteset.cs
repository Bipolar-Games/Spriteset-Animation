using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.SpritesetAnimation
{
    [System.Serializable]
    public struct SpriteSettings
    {
        [Min(0.001f)]
        public float pixelsPerUnit;

        public SpriteMeshType meshType;

        [Range(0, 32)]
        public uint extrudeEdges;

        public bool generatePhysicsShape;
    }

    [CreateAssetMenu(menuName = CreateAssetPath.Root + "Texture Spriteset")]
	public class TextureSpriteset : Spriteset
    {
		private static readonly Vector2 Center = new Vector2 (0.5f, 0.5f);

        [SerializeField, Min(1)]
        private int rowCount = 1;

        [SerializeField]
        private Texture2D texture;
        private Sprite[] sprites;

        [SerializeField]
        private SpriteSettings spriteSettings = new SpriteSettings
        {
            pixelsPerUnit = 100,
            meshType = SpriteMeshType.Tight,
            extrudeEdges = 1,
            generatePhysicsShape = true,
        };

        public override IReadOnlyList<Sprite> this[int index]
        {
            get
            {
                if (sprites == null)
                    CreateSpritesArray();
                return sprites;
            }
        }

        public override int RowCount => rowCount;
		public int Count => 12;
        
        private void OnEnable()
        {
            CreateSpritesArray();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

        [ContextMenu("Create")]
        private void CreateSpritesArray()
        {
            sprites = new Sprite[Count];

            float spriteWidth = texture.width / ColumnCount;
            float spriteHeight = texture.height / rowCount;
            
            for (int j = 0; j < rowCount; j++)
            {
                for (int i = 0; i < ColumnCount; i++)
                {
                    int index = (rowCount - 1 - j) * ColumnCount + i;
                    var rect = new Rect(i * spriteWidth, j * spriteHeight, spriteWidth, spriteHeight);
                    sprites[index] = Sprite.Create(texture, rect, Center, spriteSettings.pixelsPerUnit, 
                        spriteSettings.extrudeEdges, spriteSettings.meshType, Vector4.zero, spriteSettings.generatePhysicsShape);
                }
            }
        }

		public override int GetFramesCount(int rowIndex)
		{
			throw new System.NotImplementedException();
		}

		private void OnValidate()
        {
            CreateSpritesArray();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange change)
        {
            if (change == UnityEditor.PlayModeStateChange.EnteredPlayMode || change == UnityEditor.PlayModeStateChange.EnteredEditMode)
                CreateSpritesArray();
        }
#endif

	}
}
