using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.SpritesetAnimation
{
	[CreateAssetMenu(menuName = Paths.Root + "Spriteset")]
    public class Spriteset : ScriptableObject
    {
        [SerializeField]
        internal SpritesetRow[] animations;
		private Dictionary<string, int> rowIndicesByName;

        public IReadOnlyList<Sprite> this[int index] => animations[index];

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

		public string GetRowName(int index) => animations[index].Name;

		public int RowCount => animations.Length;

		public int GetFramesCount(int rowIndex) => animations[rowIndex].Count;
	}
}
