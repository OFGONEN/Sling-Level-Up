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
        public GameEvent event_stickman_enemy_cleared;

        [ Header( "Level Releated" ) ]
        public SharedProgressNotifier notifier_progress;
        public SharedReferenceNotifier notif_stickman_follow_reference;
        public SharedVector3Notifier notif_finishLine_position ;
    
// Private
        UnityMessage onUpdate;

        Transform stickman_transform;
        float stickman_finishLine_distance;
        float stickman_enemy_count_spawned;
        float stickman_enemy_count_lost;
#endregion

#region UnityAPI
        private void Awake()
        {
			onUpdate = ExtensionMethods.EmptyMethod;
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
			// stickman_enemy_count_spawned = 0;
			// stickman_enemy_count_lost = 0;
		}

        // Info: Called from Editor.
        public void LevelStartedResponse()
        {
		}

		public void OnStickmanEnemySpawned()
		{
			stickman_enemy_count_spawned += 1;
            FFLogger.Log( "Spawned: " + stickman_enemy_count_spawned );
		}

        public void OnStickmanEnemyLost()
        {
			stickman_enemy_count_lost += 1;
			notifier_progress.SharedValue = stickman_enemy_count_lost / ( float )stickman_enemy_count_spawned;

            if( stickman_enemy_count_lost >= stickman_enemy_count_spawned )
				event_stickman_enemy_cleared.Raise();
		}

        public void OnLevelFinished()
        {
			stickman_enemy_count_lost    = 0;
			stickman_enemy_count_spawned = 0;
		}
#endregion

#region Implementation
#endregion
    }
}