﻿using UnityEngine;

namespace FFStudio
{
	[CreateAssetMenu( fileName = "SharedVector2CleanUp", menuName = "FF/Data/Shared/Vector2CleanUp" )]
	public class SharedVector2CleanUp : ScriptableObject
	{
		public Vector2 sharedValue;
		public Vector2 defaultValue;
		public EventListenerDelegateResponse cleanUpListener;

		private void OnEnable()
		{
			cleanUpListener.OnEnable();
			cleanUpListener.response = CleanUp;
		}
		private void OnDisable()
		{
			cleanUpListener.OnDisable();
		}
		void CleanUp()
		{
			sharedValue = defaultValue;
		}
	}
}