/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "stickman_pose_", menuName = "FF/Game/Stickman Pose" ) ]
public class StickmanPose : ScriptableObject
{
#region Fields
    [ SerializeField ] TransformData[] pose_data_array;
    [ SerializeField ] string pose_key;
#endregion

#region Properties
    public string PoseKey => pose_key;
#endregion


#region API
    public TransformData GetDataOnIndex( int index )
    {
		return pose_data_array[ index ];
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    [ Button() ]
	void ExtractPoseData( Transform parent )
	{
		UnityEditor.EditorUtility.SetDirty( this );

		var transformArray = parent.GetComponentsInChildren< Transform >();

		pose_data_array = new TransformData[ transformArray.Length ];

		for( var i = 0; i < transformArray.Length; i++ )
        {
			pose_data_array[ i ] = new TransformData( transformArray[ i ] );
		}
	}
#endif
#endregion
}