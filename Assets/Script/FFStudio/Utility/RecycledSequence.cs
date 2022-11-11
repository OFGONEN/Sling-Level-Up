/* Created by and for usage of FF Studios (2021). */

using DG.Tweening;

namespace FFStudio
{
	public class RecycledSequence
	{
		UnityMessage onComplete;
		Sequence sequence;

		public Sequence Sequence => sequence;

		public Sequence Recycle( UnityMessage onComplete )
		{
			sequence = sequence.KillProper();

			this.onComplete = onComplete;

			sequence = DOTween.Sequence();
			sequence.OnComplete( OnComplete_Safe );

#if UNITY_EDITOR
			sequence.SetId( "_ff_RecycledSequence" );
#endif

			return sequence;
		}

		public Sequence Recycle()
		{
			sequence = sequence.KillProper();

			sequence   = DOTween.Sequence();
			onComplete = null;
			sequence.OnComplete( OnComplete_Safe );

#if UNITY_EDITOR
			sequence.SetId( "_ff_RecycledSequence" );
#endif
			
			return sequence;
		}

		public void Kill()
		{
			sequence = sequence.KillProper();
		}

		public void KillAndRewind()
		{
			sequence?.Rewind();
			sequence = sequence.KillProper();
		}

		public void KillAndComplete()
		{
			sequence?.Complete();
			sequence = sequence.KillProper();
		}

		void OnComplete_Safe()
		{
			sequence = null;
			onComplete?.Invoke();
		}
	}
}