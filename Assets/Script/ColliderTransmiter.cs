/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FFStudio;

public class ColliderTransmiter : MonoBehaviour
{
#region Fields
    [ SerializeField ] Collider _collider;
    [ SerializeField ] UnityEvent< Collider, Collider > unityEvent;
#endregion

#region Properties
#endregion

#region Unity API
    public void Transmit( Collider other )
    {
		unityEvent.Invoke( _collider, other );
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
