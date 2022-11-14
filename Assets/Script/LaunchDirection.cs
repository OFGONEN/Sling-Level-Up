/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

public class LaunchDirection : MonoBehaviour
{
#region Fields
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
    [ Button() ]
    void CreateLauncDirectionVisual( GameObject prefab, int count, float space )
    {
		UnityEditor.EditorUtility.SetDirty( this );

		var gfx = transform.GetChild( 0 );
		gfx.DestroyAllChildren();

        for( var i = 0; i < count; i++ )
        {
			var instance = UnityEditor.PrefabUtility.InstantiatePrefab( prefab ) as GameObject;
			instance.name = instance.name + "_" + ( i + 1 );
			instance.transform.SetParent( gfx );
			instance.transform.localRotation = Quaternion.identity;
			instance.transform.localPosition = Vector3.right * i * space;
		}
	}

    [ Button() ]
    void ChangeSizeLaunchDirectionVisual( float start, float end )
    {
		var gfx        = transform.GetChild( 0 );
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