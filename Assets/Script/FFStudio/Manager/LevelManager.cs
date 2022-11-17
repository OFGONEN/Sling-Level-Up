/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using UnityEngine.SceneManagement;

namespace FFStudio
{
    public class LevelManager : MonoBehaviour
    {
#region Fields
        [ Header( "Fired Events" ) ]
        public GameEvent levelFailedEvent;
        public GameEvent levelCompleted;

        [ Header( "Level Releated" ) ]
        public SharedProgressNotifier notifier_progress;
        public SharedReferenceNotifier notif_stickman_follow_reference;
        public SharedVector3Notifier notif_finishLine_position ;
    
// Private
        UnityMessage onUpdate;

        Transform stickman_transform;
        float stickman_finishLine_distance;
#endregion

#region UnityAPI
        private void Awake()
        {
			onUpdate = ExtensionMethods.EmptyMethod;
		}

        private void Update()
        {
			onUpdate();
		}
#endregion

#region API
        // Info: Called from Editor.
        public void LevelLoadedResponse()
        {
			notifier_progress.SharedValue = 0;

			var levelData = CurrentLevelData.Instance.levelData;
            // Set Active Scene.
			if( levelData.scene_overrideAsActiveScene )
				SceneManager.SetActiveScene( SceneManager.GetSceneAt( 1 ) );
            else
				SceneManager.SetActiveScene( SceneManager.GetSceneAt( 0 ) );
		}

        // Info: Called from Editor.
        public void LevelRevealedResponse()
        {

        }

        // Info: Called from Editor.
        public void LevelStartedResponse()
        {
			stickman_transform           = notif_stickman_follow_reference.sharedValue as Transform;
			stickman_finishLine_distance = notif_finishLine_position.sharedValue.x - stickman_transform.position.x;

			onUpdate = OnLevelProgressUpdate;
		}

        public void OnStickmanReachedFinishLine()
        {
			onUpdate = ExtensionMethods.EmptyMethod;
			notifier_progress.SharedValue = 1f;
		}

        public void OnLevelFailed()
        {
			onUpdate = ExtensionMethods.EmptyMethod;
		}
#endregion

#region Implementation
        void OnLevelProgressUpdate()
        {
			notifier_progress.SharedValue = 1f - ( notif_finishLine_position.sharedValue.x - stickman_transform.position.x ) / stickman_finishLine_distance;
		}
#endregion
    }
}