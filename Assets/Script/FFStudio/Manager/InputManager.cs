﻿/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Lean.Touch;
using Sirenix.OdinInspector;

namespace FFStudio
{
    public class InputManager : MonoBehaviour
    {
#region Fields (Inspector Interface)
	[ Title( "Fired Events" ) ]
		public SwipeInputEvent event_input_swipe;
		public ScreenPressEvent event_input_screenPress;
		public IntGameEvent event_input_tap;

	[ Title( "Shared Variables" ) ]
		public SharedReferenceNotifier notifier_reference_camera_main;
		public SharedVector2 shared_finger_delta;

		int swipeThreshold;
		Vector2 finger_down_position;

		Transform transform_camera_main;
		Camera camera_main;
		LeanTouch leanTouch;
#endregion

#region Unity API
		void OnEnable()
		{
			notifier_reference_camera_main.Subscribe( OnCameraReferenceChange );
		}

		void OnDisable()
		{
			notifier_reference_camera_main.Unsubscribe( OnCameraReferenceChange );
		}

		void Awake()
		{
			swipeThreshold = Screen.width * GameSettings.Instance.swipeThreshold / 100;

			leanTouch         = GetComponent< LeanTouch >();
			leanTouch.enabled = false;
		}
#endregion
		
#region API
		public void Swiped( Vector2 delta )
		{
			event_input_swipe.ReceiveInput( delta );
		}
		
		public void Tapped( int count )
		{
			event_input_tap.eventValue = count;

			event_input_tap.Raise();
		}

		public void OnFingerDown( LeanFinger finger )
		{
			finger_down_position = finger.ScreenPosition;
		}

		public void OnFingerUpdate( LeanFinger finger )
		{
			var fingerPosition                  = finger.ScreenPosition;
			    shared_finger_delta.sharedValue = fingerPosition - finger_down_position;
		}
#endregion

#region Implementation
		void OnCameraReferenceChange()
		{
			var value = notifier_reference_camera_main.SharedValue;

			if( value == null )
			{
				transform_camera_main = null;
				leanTouch.enabled = false;
			}
			else 
			{
				transform_camera_main = value as Transform;
				camera_main           = transform_camera_main.GetComponent< Camera >();
				leanTouch.enabled    = true;
			}
		}
#endregion
    }
}