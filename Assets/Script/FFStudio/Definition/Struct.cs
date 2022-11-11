/* Created by and for usage of FF Studios (2021). */

using System;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace FFStudio
{
	[ Serializable ]
	public struct TransformData
	{
		public Vector3 position;
		public Vector3 rotation; // Euler angles.
		public Vector3 scale; // Local scale.
	}

	[ Serializable ]
	public struct EventAndResponse
	{
		public MultipleEventListenerDelegateResponse eventListener;
		public UnityEvent unityEvent;

		public void Pair()
		{
			eventListener.response = unityEvent.Invoke;
		}
	}

	[ Serializable ]
	public struct ParticleData
	{
		public ParticleSpawnEvent particle_event;
		public string alias;
		public bool parent;
		public Vector3 offset;
		public float size;
	}

	[ Serializable ]
	public struct PopUpTextData
	{
		public string text;
		public Color color;
		public bool parent;
		public Vector3 offset;
		public float size;
	}

	[ Serializable ]
	public struct RandomParticlePool
	{
		public string alias;
		public ParticleEffectPool[] particleEffectPools;

		public ParticleEffect GiveRandomEntity()
		{
			return particleEffectPools.ReturnRandom<ParticleEffectPool>().GetEntity();
		}
	}

	[ Serializable ]
	public struct AnimationParameterData
	{
		public AnimationParameterType parameterType;
		public string parameter_name;
		[ ShowIf( "parameterType", AnimationParameterType.Bool  )] public bool parameter_bool;
		[ ShowIf( "parameterType", AnimationParameterType.Int   )] public int parameter_int;
		[ ShowIf( "parameterType", AnimationParameterType.Float )] public float parameter_float;
	}

	[ Serializable ]
	public struct CameraTweenData
	{
		public Transform target_transform;

		public float tween_duration;
		public bool change_position;
		public bool change_rotation;

		[ ShowIf( "change_position" ) ] public Ease ease_position;
		[ ShowIf( "change_rotation" ) ] public Ease ease_rotation;

		public UnityEvent event_complete;
		public bool event_complete_alwaysInvoke;
	}
	
	[ Serializable ]
	public struct PlayerPrefs_Int
	{
		[ ReadOnly ]
		public string key;
		[ OnValueChanged( "Save" ) ]
		public int value;
		
		public void Refresh() => value = PlayerPrefs.GetInt( key, 0 );
		public void Save()
		{
			PlayerPrefs.SetInt( key, value );
			FFLogger.Log( $"PlayerPrefs: Saved value \"{value}\" for key \"{key}\"." );
		}
	}
	
	[ Serializable ]
	public struct PlayerPrefs_Float
	{
		[ ReadOnly ]
		public string key;
		[ OnValueChanged( "Save" ) ]
		public float value;
		
		public void Refresh() => value = PlayerPrefs.GetFloat( key, 0.0f );
		public void Save()
		{
			PlayerPrefs.SetFloat( key, value );
			FFLogger.Log( $"PlayerPrefs: Saved value \"{value}\" for key \"{key}\"." );
		}
	}
	
	[ Serializable ]
	public struct PlayerPrefs_String
	{
		[ ReadOnly ]
		public string key;
		[ OnValueChanged( "Save" ) ]
		public string value;
		
		public void Refresh() => value = PlayerPrefs.GetString( key, "" );
		public void Save()
		{
			PlayerPrefs.SetString( key, value );
			FFLogger.Log( $"PlayerPrefs: Saved value \"{value}\" for key \"{key}\"." );
		}
	}
	
	[ Serializable ]
	public struct ColorPerThreshold
	{
		public Color color;
		[ MappedFloat ] public float threshold;
	}

	[ Serializable ]
	public struct TriggerRespondData
	{
		[ Layer() ] public int collision_layer;
		public UnityEvent< Collider > trigger_event;
	}

	[ Serializable ]
	public struct CollisionRespondData
	{
		[ Layer() ] public int collision_layer;
		public UnityEvent< Collision > collision_event;
	}
}
