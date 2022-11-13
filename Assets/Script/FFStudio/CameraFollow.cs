/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
    public class CameraFollow : MonoBehaviour
    {
#region Fields
    [ Title( "Setup" ) ]
        [ SerializeField ] SharedReferenceNotifier notif_stickman_reference;
        [ LabelText( "Follow Method should use Delta Time" ), SerializeField ] bool followWithDeltaTime;
        [ LabelText( "Launch Power" ), SerializeField ] SharedFloat shared_finger_delta_magnitude;

        Transform target_transform;
        UnityMessage updateMethod;

        float target_offset_Z;
#endregion

#region Properties
#endregion

#region Unity API
        void OnDisable()
        {
			updateMethod = ExtensionMethods.EmptyMethod;
		}

        void Awake()
        {
            updateMethod = ExtensionMethods.EmptyMethod;
        }

        void Update()
        {
            updateMethod();
        }
#endregion

#region API
        public void OnLevelRevealedResponse()
        {
            target_transform = notif_stickman_reference.sharedValue as Transform;

            if( followWithDeltaTime )
				updateMethod = FollowTargetDeltaTime;
            else
				updateMethod = FollowTargetFixedDeltaTime;
		}

        public void OnLevelFinishedResponse()
        {
            updateMethod = ExtensionMethods.EmptyMethod;
        }

        public void OnStickmanLaunchStart()
        {
			updateMethod += OnStickmanLaunchUpdate;
			target_offset_Z = 0;
		}

		public void OnStickmanLaunchEnd()
		{
			updateMethod -= OnStickmanLaunchUpdate;
		}
#endregion

#region Implementation
        void OnStickmanLaunchUpdate()
        {
			target_offset_Z = Mathf.InverseLerp( shared_finger_delta_magnitude.sharedValue, GameSettings.Instance.camera_zoomOut_value_range.x, GameSettings.Instance.camera_zoomOut_value_range.y ) * GameSettings.Instance.camera_zoomOut_value_max;
		}

        void FollowTargetDeltaTime()
        {
			// Info: Simple follow logic.
			var offset             = GameSettings.Instance.camera_follow_offset + Vector3.forward * target_offset_Z;
			var targetPosition     = target_transform.position + offset;
			    transform.position = Vector3.Lerp( transform.position, targetPosition, GameSettings.Instance.camera_follow_speed * Time.deltaTime );
        }

		void FollowTargetFixedDeltaTime()
		{
			// Info: Simple follow logic.
			var offset             = GameSettings.Instance.camera_follow_offset + Vector3.forward * target_offset_Z;
			var targetPosition     = target_transform.position + offset;
			    transform.position = Vector3.Lerp( transform.position, targetPosition, GameSettings.Instance.camera_follow_speed * Time.fixedDeltaTime );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
        [ Button() ]
        void RefreshOffset()
        {
			var stickman = GameObject.FindWithTag( "Player" );

            if( stickman )
				transform.position = stickman.transform.position + GameSettings.Instance.camera_follow_offset;

		}
#endif
#endregion
    }
}