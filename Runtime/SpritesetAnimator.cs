using System.Collections;
using UnityEngine;

namespace Bipolar.SpritesetAnimation
{
	public class SpritesetAnimator : MonoBehaviour
	{
		public const float IdleAnimationSpeed = 4;

		[Header("Settings")]
		[SerializeField]
		[Tooltip("Sprite Renderer whose sprite will be animated.")]
		private SpriteRenderer spriteRenderer;

		[SerializeField]
		[Tooltip("Spriteset for animation.")]
		private Spriteset spriteset;
		public Spriteset Spriteset
		{
			get => spriteset;
			set
			{
				spriteset = value;
			}
		}

		[Header("Properties")]
		[SerializeField]
		private bool isAnimating = true;
		public bool IsAnimating
		{
			get => isAnimating;
			set
			{
				isAnimating = value;
			}
		}

		[SerializeField, Min(0)]
		[Tooltip("Speed of animation [in Frames per Second].")]
		private float animationSpeed = IdleAnimationSpeed;
		public float AnimationSpeed
		{
			get => animationSpeed;
			set => animationSpeed = Mathf.Max(value, 0);
		}

		[SerializeField]
		[Tooltip("If true: frames will change in reversed order.")]
		private bool isReversed;
		public bool IsReversed
		{
			get => isReversed;
			set => isReversed = value;
		}

		[SerializeField]
		[Tooltip("Current row in spriteset from which frames are taken.")]
		private int currentAnimationIndex;
		public int CurrentAnimationIndex
		{
			get => currentAnimationIndex;
			set
			{
				if (currentAnimationIndex == value)
					return;

				currentAnimationIndex = value;
				ValidateAnimationIndex();
				animationTimer = 1;
			}
		}

		[SerializeField]
		[Tooltip("By default sequence has length (number of frames) equal to spriteset columns count. If this value is positive, it will be taken as animation sequence lenght instead.")]
		private int overrideSequenceLength;
		public int OverrideSequenceLength
		{
			get => overrideSequenceLength;
			set => overrideSequenceLength = value;
		}

		[SerializeField]
		[Tooltip("By default sprite in column zero is treated as starting animation frame. Use this field to modify this value.")]
		private int frameIndexOffset;
		public int FrameIndexOffset
		{
			get => frameIndexOffset;
			set => frameIndexOffset = value;
		}

		[SerializeField]
		[Tooltip("This value is incremented (or decremented) while animating frames.")]
		private int baseFrameIndex;
		public int BaseFrameIndex
		{
			get => baseFrameIndex;
			set
			{
				baseFrameIndex = value;
				RefreshSprite();
			}
		}

		public int CurrentFrameIndex => baseFrameIndex + frameIndexOffset;

		public int CurrentSequenceLength => GetSequenceLength(currentAnimationIndex);

		private int GetSequenceLength(int animationIndex)
		{
			int columnsCount = spriteset.GetFramesCount(animationIndex);
			if (overrideSequenceLength <= 0)
				return columnsCount;

			return Mathf.Min(overrideSequenceLength, columnsCount);
		}

		public int RowCount => spriteset ? spriteset.RowCount : 0;

#if NAUGHTY_ATTRIBUTES
		[NaughtyAttributes.ShowNonSerializedField]
#endif
		private float animationTimer;

		protected void Reset()
		{
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		}

		private void Start()
		{
			currentAnimationIndex = 0;
			animationTimer = 0;
		}

		private void Update()
		{
			if (isAnimating == false)
				return;

			animationTimer += Time.deltaTime * animationSpeed;
			if (animationTimer > 1)
			{
				animationTimer -= 1;
				int sequenceLength = CurrentSequenceLength;
				int indexChange = isReversed ? -1 : 1;
				baseFrameIndex = (baseFrameIndex + indexChange + sequenceLength) % sequenceLength;
				RefreshSprite();
			}
		}

		public void RefreshSprite()
		{
			RefreshSprite(currentAnimationIndex, CurrentFrameIndex);
		}

		public void RefreshSprite(int animationIndex, int frameIndex)
		{
			if (spriteRenderer && spriteset)
				spriteRenderer.sprite = spriteset[animationIndex][frameIndex];
		}

		private void ValidateAnimationIndex()
		{
			currentAnimationIndex = Mathf.Clamp(currentAnimationIndex, 0, RowCount - 1);
		}

		public void PlayAnimationOnce(System.Action onFinished = null)
		{
			PlayAnimationOnce(currentAnimationIndex, animationSpeed, onFinished);
		}

		public void PlayAnimationOnce(int animationIndex, System.Action onFinished = null)
		{
			PlayAnimationOnce(animationIndex, animationSpeed, onFinished);
		}
		
		public void PlayAnimationOnce(System.Action onFinished, float speed)
		{
			PlayAnimationOnce(currentAnimationIndex, speed, onFinished);
		}

		public void PlayAnimationOnce(int animationIndex, float speed, System.Action onFinished = null)
		{
			isAnimating = false;
			StopPlayingOnce();
			StartCoroutine(PlayAnimationOnceCo(animationIndex, onFinished, speed));
		}

		public void StopPlayingOnce()
		{
			StopAllCoroutines();
		}

		private IEnumerator PlayAnimationOnceCo(int animationIndex, System.Action onFinished, float speed)
		{
			var wait = new WaitForSeconds(1f / speed);
			int sequenceLength = GetSequenceLength(animationIndex);
			baseFrameIndex = isReversed ? sequenceLength - 1 : 0;
			int endingFrameIndex = isReversed ? 0 : sequenceLength - 1;
			RefreshSprite(animationIndex, CurrentFrameIndex);
			while (true)
			{
				yield return wait;
				int indexChange = isReversed ? -1 : 1;
				baseFrameIndex += indexChange;
				RefreshSprite(animationIndex, CurrentFrameIndex);
				if (baseFrameIndex == endingFrameIndex)
					break;
			}
			onFinished?.Invoke();
		}

		private void OnValidate()
		{
			ValidateAnimationIndex();
			RefreshSprite();
		}
	}
}
