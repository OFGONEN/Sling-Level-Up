/* Created by and for usage of FF Studios (2021). */

using DG.Tweening;

namespace FFStudio
{
	public class RecycledTween
	{
#region Fields
		UnityMessage onComplete;
		Tween tween;
#endregion

#region Properties
		public Tween Tween => tween;
#endregion

#region API
		public void Recycle( Tween tween_unsafe, UnityMessage onComplete )
		{
			tween = tween.KillProper();
			tween = tween_unsafe;

			this.onComplete = onComplete;

			tween.OnComplete( OnComplete_Safe );

#if UNITY_EDITOR
			tween.SetId( "_ff_RecycledTween" );
#endif
		}

		public void Recycle( Tween tween_unsafe )
		{
			tween = tween.KillProper();
			tween = tween_unsafe;
			
			this.onComplete = null;

			tween.OnComplete( OnComplete_Safe );

#if UNITY_EDITOR
			tween.SetId( "_ff_RecycledTween" );
#endif
		}
		
		public void OnComplete( UnityMessage onComplete )
		{
			this.onComplete = onComplete;
		}
		
		public bool IsPlaying()
		{
			if( tween != null )
				return tween.IsPlaying();
			else
				return false;
		}

		public void Kill()
		{
			tween = tween.KillProper();
		}

		public void KillAndRewind()
		{
			tween?.Rewind();
			tween = tween.KillProper();
		}

		public void KillAndComplete()
		{
			tween?.Complete();
			tween = tween.KillProper();
		}
#endregion

#region Implementation
		void OnComplete_Safe()
		{
			tween = null;
			onComplete?.Invoke();
		}
#endregion
	}
}