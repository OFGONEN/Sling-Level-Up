/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using DG.Tweening;
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
        [ SerializeField ] GameEvent event_level_started;

    [ Title( "Sequence References" ) ]
        [ SerializeField ] SharedReferenceNotifier notif_camera_reference_sequence_start;
        [ SerializeField ] SharedReferenceNotifier notif_camera_reference_sequence_end;

    [ Title( "Components" ) ]
        [ SerializeField ] Camera _camera;

        Transform target_transform;
        UnityMessage updateMethod;
		RecycledTween recycledTween = new RecycledTween();

		float camera_zoom_out_value;

        Vector3 camera_sequence_position_start;
        Vector3 camera_sequence_position_end;
#endregion

#region Properties
#endregion

#region Unity API
        void OnDisable()
        {
			recycledTween.Kill();
			updateMethod = ExtensionMethods.EmptyMethod;
		}

        void Awake()
        {
            updateMethod = ExtensionMethods.EmptyMethod;
        }

		private void Start()
		{
			if( CurrentLevelData.Instance.levelData.scene_sequence )
            {
				camera_sequence_position_start = ( notif_camera_reference_sequence_start.sharedValue as Transform ).position;
				camera_sequence_position_end   = ( notif_camera_reference_sequence_end.sharedValue as Transform ).position;

				transform.position = camera_sequence_position_start;
			}
		}

        void Update()
        {
            updateMethod();
        }
#endregion

#region API
        public void OnLevelRevealedResponse()
        {
            if( CurrentLevelData.Instance.levelData.scene_sequence )
				DoSequence();
			else
				OnCameraSequenceComplete();
		}

        public void OnLevelFinishedResponse()
        {
            // updateMethod = ExtensionMethods.EmptyMethod;
        }

        public void OnStickmanLaunchStart()
        {
			updateMethod += OnStickmanLaunchUpdate;
			camera_zoom_out_value = 0;
		}

		public void OnStickmanLaunchEnd()
		{
			ZoomIn();

			updateMethod -= OnStickmanLaunchUpdate;
		}
#endregion

#region Implementation
		void ZoomIn()
		{
			camera_zoom_out_value = 0;
			
			recycledTween.Recycle( DOTween.To( GetZoomValue,
				SetZoomOutValue, GameSettings.Instance.camera_zoom_value, GameSettings.Instance.camera_zoomIn_duration )
				.SetEase( GameSettings.Instance.camera_zoomIn_ease ) );
		}
        void DoSequence()
        {
			transform.DOMove( camera_sequence_position_end,
				CurrentLevelData.Instance.levelData.scene_sequence_duration )
				.SetEase( CurrentLevelData.Instance.levelData.scene_sequence_ease )
				.OnComplete( OnCameraSequenceComplete );
		}

        void OnCameraSequenceComplete()
        {
			ZoomIn();
			event_level_started.Raise();
			StartFollowingTarget();
		}

		void StartFollowingTarget()
		{
			target_transform = notif_stickman_reference.sharedValue as Transform;

            if( followWithDeltaTime )
				updateMethod = FollowTargetDeltaTime;
            else
				updateMethod = FollowTargetFixedDeltaTime;
        }

		void OnStickmanLaunchUpdate()
        {
			var targetOffset = GameSettings.Instance.camera_zoomOut_value_range.ReturnProgress( shared_finger_delta_magnitude.sharedValue );

			camera_zoom_out_value = Mathf.Lerp( camera_zoom_out_value, targetOffset, GameSettings.Instance.camera_zoomOut_value_speed * Time.deltaTime );

			_camera.orthographicSize = GameSettings.Instance.camera_zoom_value + camera_zoom_out_value;
		}

        void FollowTargetDeltaTime()
        {
			// Info: Simple follow logic.
			var targetPosition     = target_transform.position + GameSettings.Instance.camera_follow_offset;
			    transform.position = Vector3.Lerp( transform.position, targetPosition, GameSettings.Instance.camera_follow_speed * Time.deltaTime );
        }

		void FollowTargetFixedDeltaTime()
		{
			// Info: Simple follow logic.
			var targetPosition     = target_transform.position + GameSettings.Instance.camera_follow_offset;
			    transform.position = Vector3.Lerp( transform.position, targetPosition, GameSettings.Instance.camera_follow_speed * Time.fixedDeltaTime );
		}

        float GetZoomValue()
        {
			return _camera.orthographicSize;
		}

        void SetZoomOutValue( float value )
        {
			_camera.orthographicSize = value;
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

			_camera.orthographicSize = GameSettings.Instance.camera_zoom_value;
		}

        [ Button() ]
		void ResetToSequenceStartPosition()
		{
			var cameraSequenceStart = GameObject.FindWithTag( "Camera_Sequence_Start" );

			if( cameraSequenceStart )
				transform.position = cameraSequenceStart.transform.position;
		}
#endif
#endregion
    }
}