using System.Collections;
using UnityEngine;

namespace Bipolar.SpritesetAnimation
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SpritesetAnimator : MonoBehaviour
	{
		public const float IdleAnimationSpeed = 4;

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
		public int AnimationIndex
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

		public string CurrentAnimationName
		{
			get => spriteset.GetRowName(currentAnimationIndex);
			set => AnimationIndex = spriteset.GetRowIndex(value);
		}

		[SerializeField]
		[Tooltip("By default sequence has length (number of frames) equal to sprites count in row. " +
			"If this value is positive, it will be taken as animation sequence length instead. " +
			"If this value is negative, sequence length will be lowered by that amount (min 1).")]
		private int overrideSequenceLength;
		public int OverrideSequenceLength
		{
			get => overrideSequenceLength;
			set => overrideSequenceLength = value;
		}

		[SerializeField]
		[Tooltip("By default sprite at index zero is treated as starting animation frame. Use this field to modify this value.")]
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


		private SpriteRenderer spriteRenderer;
		public SpriteRenderer SpriteRenderer => spriteRenderer;

		public int CurrentFrameIndex => baseFrameIndex + frameIndexOffset;

		public int GetCurrentSequenceLength() => GetSequenceLength(currentAnimationIndex);

		private int GetSequenceLength(int animationIndex)
		{
			int sequenceLength = spriteset.GetFramesCount(animationIndex);

			if (overrideSequenceLength > 0)
				sequenceLength = Mathf.Min(overrideSequenceLength, sequenceLength);
			else if (overrideSequenceLength < 0)
				sequenceLength += overrideSequenceLength;

			sequenceLength -= frameIndexOffset;
			return Mathf.Max(1, sequenceLength);
		}

		public int RowCount => spriteset ? spriteset.RowCount : 0;

#if NAUGHTY_ATTRIBUTES
		[NaughtyAttributes.ShowNonSerializedField]
#endif
		private float animationTimer;

		private void Awake()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			animationTimer = 0;
		}

		private void Update()
		{
			if (isAnimating)
				Animate(Time.deltaTime);
		}

		private void Animate(float timeDelta)
		{
			animationTimer += timeDelta * animationSpeed;
			if (animationTimer > 1)
			{
				animationTimer -= 1;
				int sequenceLength = GetCurrentSequenceLength();
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
			int endingFrameIndex = isReversed ? 0 : sequenceLength - 1;
			int startFrameIndex = isReversed ? sequenceLength - 1 : 0;
			int indexChange = isReversed ? -1 : 1;

			for (baseFrameIndex = startFrameIndex; baseFrameIndex != endingFrameIndex; baseFrameIndex += indexChange)
			{
				RefreshSprite(animationIndex, CurrentFrameIndex);
				yield return wait;
			}
			RefreshSprite(animationIndex, endingFrameIndex);
			onFinished?.Invoke();
		}

		private void OnValidate()
		{
			ValidateAnimationIndex();
			if (spriteset)
			{
				RefreshSprite(currentAnimationIndex, CurrentFrameIndex % spriteset[AnimationIndex].Count);
			}
		}
	}

	public struct PlayAnimationOnceParams
	{
		private Param<int> animationIndex;
		public int AnimationIndex
        {
            readonly get => animationIndex.Value; 
			set => animationIndex.Value = value;
        }

		private Param<float> speed;
		public float Speed
        {
            readonly get => speed.Value; 
			set => speed.Value = value;
        }




        internal struct Param<T>
		{
			private T _value;
			public T Value
			{
				readonly get => _value;
				set
				{
					_value = value;
					IsSpecified = true;
				}
			}

			public bool IsSpecified { get; private set; }
		}
	}



}