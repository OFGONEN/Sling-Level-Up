/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FFStudio;
using Sirenix.OdinInspector;

public class ColliderTransmiter : MonoBehaviour
{
#region Fields
    [ LabelText( "Cache The Collider" ), SerializeField ] bool collider_cache;
    [ HideIf( "collider_cache" ), SerializeField ] Collider _collider;
    [ SerializeField ] UnityEvent< Collider, Collider > unityEvent;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
        _collider = GetComponent< Collider >();

#if UNITY_EDITOR
        if( !_collider )
            FFLogger.LogError( "Collider DID NOT FOUND", gameObject );
#endif
    }
#endregion

#region API
    public void Transmit( Collider other )
    {
		unityEvent.Invoke( _collider, other );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
