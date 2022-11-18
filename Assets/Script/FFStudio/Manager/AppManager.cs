/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FFStudio
{
	public class AppManager : MonoBehaviour
	{
#region Fields
	[ Title( "Fired Events" ) ]
		public GameEvent event_level_loaded;
		public GameEvent event_level_unload_start;
		public GameEvent event_level_unload_end;
		public SharedFloatNotifier levelProgress;
#endregion

#region Unity API
		void Start()
		{
			var eventSystem = GameObject.Find( "EventSystem" );

			if( eventSystem != null )
			{
				FFLogger.Log( "Another EventSystem is disabled", eventSystem );
				eventSystem.SetActive( false );
			}

			StartCoroutine( LoadLevel( null ) );
			Application.targetFrameRate = Screen.currentResolution.refreshRate;
		}
#endregion

#region API
		public void ResetLevel()
		{
			event_level_unload_start.Raise();
			var operation = SceneManager.UnloadSceneAsync( CurrentLevelData.Instance.levelData.scene_index );
			operation.completed += ( AsyncOperation operation ) => StartCoroutine( LoadLevel( event_level_unload_end.Raise ) );
		}

		public void LoadNewLevel()
		{
			CurrentLevelData.Instance.currentLevel_Real++;
			CurrentLevelData.Instance.currentLevel_Shown++;
			PlayerPrefs.SetInt( "Level", CurrentLevelData.Instance.currentLevel_Real );
			PlayerPrefs.SetInt( "Consecutive Level", CurrentLevelData.Instance.currentLevel_Shown );

			event_level_unload_start.Raise();
			var operation = SceneManager.UnloadSceneAsync( CurrentLevelData.Instance.levelData.scene_index );
			operation.completed += ( AsyncOperation operation ) => StartCoroutine( LoadLevel( event_level_unload_end.Raise ) );
		}
#endregion

#region Implementation
		IEnumerator LoadLevel( UnityMessage onLevelUnload )
		{
			onLevelUnload?.Invoke();

			CurrentLevelData.Instance.currentLevel_Real = PlayerPrefs.GetInt( "Level", 1 );
			CurrentLevelData.Instance.currentLevel_Shown = PlayerPrefs.GetInt( "Consecutive Level", 1 );

			CurrentLevelData.Instance.LoadCurrentLevelData();

			// SceneManager.LoadScene( CurrentLevelData.Instance.levelData.sceneIndex, LoadSceneMode.Additive );
			var operation = SceneManager.LoadSceneAsync( CurrentLevelData.Instance.levelData.scene_index, LoadSceneMode.Additive );

			levelProgress.SharedValue = 0;

			while( !operation.isDone )
			{
				yield return null;

				levelProgress.SharedValue = operation.progress;
			}

			event_level_loaded.Raise();
		}
#endregion
	}
}