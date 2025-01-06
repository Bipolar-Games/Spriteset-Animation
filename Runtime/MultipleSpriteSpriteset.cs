using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bipolar.SpritesetAnimation
{
	[CreateAssetMenu(menuName = CreateAssetPath.Root + "Multiple Sprite Spriteset")]
    public class MultipleSpriteSpriteset : Spriteset
    {
        [SerializeField, FormerlySerializedAs("sprites")]
        private SpritesetRow[] animations;
		private Dictionary<string, int> rowIndicesByName;

        public override IReadOnlyList<Sprite> this[int index] => animations[index];

		public IReadOnlyList<Sprite> this[string name]
		{
			get
			{
				int index = GetRowIndex(name);
				return index >= 0 ? animations[index] : null;
			}
		}

		private void CreateDictionary()
		{
			rowIndicesByName = new Dictionary<string, int>();
			for (int i = 0; i < animations.Length; i++)
				rowIndicesByName.Add(animations[i].Name, i);
		}

		public int GetRowIndex(string name)
		{
			if (rowIndicesByName == null)
				CreateDictionary();

			return rowIndicesByName.TryGetValue(name, out int index) ? index : -1;
		}

		public override int RowCount => animations.Length;

		public override int GetFramesCount(int rowIndex) => animations[rowIndex].Count;
	}
}
