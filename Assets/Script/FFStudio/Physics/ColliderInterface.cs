/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class ColliderInterface : MonoBehaviour
	{
#region Fields
		[ SerializeField ] CollisionRespondData[] collision_respond_data_array;
		[ SerializeField ] TriggerRespondData[] trigger_respond_data_array;

		Dictionary< int, int > collision_respond_data_dictionary;
		Dictionary< int, int > trigger_respond_data_dictionary;
#endregion

#region Properties
#endregion

#region Unity API
		private void Awake()
		{
			collision_respond_data_dictionary = new Dictionary< int, int >( collision_respond_data_array.Length );
			trigger_respond_data_dictionary   = new Dictionary< int, int >( trigger_respond_data_array.Length );

			for( var i = 0; i < collision_respond_data_array.Length; i++ )
				collision_respond_data_dictionary.Add( collision_respond_data_array[ i ].collision_layer, i );

			for( var i = 0; i < trigger_respond_data_array.Length; i++ )
				trigger_respond_data_dictionary.Add( trigger_respond_data_array[ i ].collision_layer, i );

		}
#endregion

#region API
		public void OnCollision( Collision collision )
		{
			int index;

			if( collision_respond_data_dictionary.TryGetValue( collision.gameObject.layer, out index ) )
				collision_respond_data_array[ index ].collision_event.Invoke( collision );
		}

		public void OnTrigger( Collider collider )
		{
			int index;

			if( trigger_respond_data_dictionary.TryGetValue( collider.gameObject.layer, out index ) )
				trigger_respond_data_array[ index ].trigger_event.Invoke( collider );
		}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}