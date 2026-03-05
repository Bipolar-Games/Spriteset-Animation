using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bipolar.SpritesetAnimation
{
	[System.Serializable]
	public class SpritesetRow : IReadOnlyList<Sprite>
    {
		[SerializeField]
		private string name;
		public string Name => name;

        [SerializeField]
        private Sprite[] sprites;

        public Sprite this[int index]
        {
            get
            {
                return sprites[index];
            }
        }

        public int Count => sprites.Length;

		public IEnumerator<Sprite> GetEnumerator()
		{
			foreach (var sprite in sprites) 
				yield return sprite;
		}

		IEnumerator IEnumerable.GetEnumerator() => sprites.GetEnumerator();
	}
}
