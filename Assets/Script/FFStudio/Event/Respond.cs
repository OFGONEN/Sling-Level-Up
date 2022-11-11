/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class Respond : MonoBehaviour
	{
#region Fields
        [ SerializeReference ] public EventAndResponseDataBase[] eventPairs = new EventAndResponseDataBase[ 0 ];
#endregion

#region Properties
#endregion

#region Unity API
		void OnEnable()
		{
			for( var i = 0; i < eventPairs.Length; i++ )
				eventPairs[ i ].eventListener.OnEnable();
		}

		void OnDisable()
		{
			for( var i = 0; i < eventPairs.Length; i++ )
				eventPairs[ i ].eventListener.OnDisable();
		}

		void Awake()
		{
			for( var i = 0; i < eventPairs.Length; i++ )
				eventPairs[ i ].Pair();
		}
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}