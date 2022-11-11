/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using DG.Tweening;

namespace FFStudio
{
	public class Cooldown
	{
#region Fields
		RecycledTween recycledTween_cooldown = new RecycledTween();

		bool isOngoing = false;

		TweenCallback onComplete;

		bool popupTextOnComplete = false;
		string description;
#endregion

#region Properties
		public bool IsOver => !isOngoing;
		public float ElapsedPercentage => recycledTween_cooldown.Tween.ElapsedPercentage();
#endregion

#region API
		public void Start( float duration, bool loop = false, TweenCallback onCompleteDelegate = null )
		{
			isOngoing = true;
			onComplete = onCompleteDelegate;
			recycledTween_cooldown.Recycle( DOVirtual.DelayedCall( duration, OnComplete ).SetLoops( loop ? -1 : 1 ) );
		}

		public void Start( float duration, string description, bool popupTextOnComplete, bool loop = false, TweenCallback onCompleteDelegate = null )
		{
			this.description = description;
			this.popupTextOnComplete = popupTextOnComplete;

			FFStudio.FFLogger.Log( "Cooldown duration for " + description + " started." );
			FFLogger.PopUpText( Vector3.zero, "Cooldown duration for " + description + " started." );

			Start( duration, loop, onCompleteDelegate );
		}

		public void Kill()
		{
			recycledTween_cooldown.Kill();
			isOngoing = false;
		}
#endregion

#region Implementation
		void OnComplete()
		{
			isOngoing = false;
			onComplete?.Invoke();

			if( popupTextOnComplete )
			{
				FFStudio.FFLogger.Log( "Cooldown duration for " + description + " is over." );
				FFLogger.PopUpText( Vector3.zero, "Cooldown duration for " + description + " is over." );
			}
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}