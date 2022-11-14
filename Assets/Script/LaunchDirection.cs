/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class LaunchDirection : MonoBehaviour
{
#region Fields
  [ Title( "Shared Variables" ) ]
    [ SerializeField ] SharedReferenceNotifier notif_stickman_reference;
    [ SerializeField ] SharedVector2 shared_finger_delta_direction;
	[ SerializeField ] SharedFloat shared_finger_delta_magnitude;

  [ Title( "Components" ) ]
    [ SerializeField ] Transform gfx;
    [ SerializeField ] GameObject[] gfx_child_array;
    [ SerializeField ] Transform target;


    UnityMessage onUpdate;
#endregion

#region Properties
#endregion

#region Unity API
    void OnDisable()
    {
		onUpdate = ExtensionMethods.EmptyMethod;
	}

    void Awake()
    {
		onUpdate = ExtensionMethods.EmptyMethod;
	}

    void Update()
    {
		onUpdate();
	}
#endregion

#region API
    public void OnStickmanLaunchStart()
    {
		target.localPosition = Vector3.right;
		DisableLaunchVisual();
	}

	public void OnStickmanLaunchEnd()
	{
		onUpdate = ExtensionMethods.EmptyMethod;
		DisableLaunchVisual();
	}
#endregion

#region Implementation
    void OnLaunchUpdate()
    {
		target.localPosition = shared_finger_delta_direction.sharedValue;
		OnLaunchUpdate_Visual();
	}

    void OnLaunchUpdate_Visual()
    {

    }

    void DisableLaunchVisual()
    {
        for( var i = 0; i < gfx_child_array.Length; i++ )
        {
			gfx_child_array[ i ].SetActive( false );
		}
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
    [ Button() ]
    void CreateLauncDirectionVisual( GameObject prefab, int count, float space )
    {
		UnityEditor.EditorUtility.SetDirty( this );

		gfx.DestroyAllChildren();
		gfx_child_array = new GameObject[ count ];

		for( var i = 0; i < count; i++ )
        {
			var instance = UnityEditor.PrefabUtility.InstantiatePrefab( prefab ) as GameObject;
			instance.name = instance.name + "_" + ( i + 1 );
			instance.transform.SetParent( gfx );
			instance.transform.localRotation = Quaternion.identity;
			instance.transform.localPosition = Vector3.right * i * space;

			gfx_child_array[ i ] = instance;
		}
	}

    [ Button() ]
    void ChangeSizeLaunchDirectionVisual( float start, float end )
    {
		var childCount = gfx.childCount;

		for( var i = 0; i < gfx.childCount; i++ )
		{
			float ratio = ( float )i / childCount;
			gfx.GetChild( i ).localScale = Vector3.one * Mathf.Lerp( start, end, ratio );
		}
	}
#endif
#endregion
}