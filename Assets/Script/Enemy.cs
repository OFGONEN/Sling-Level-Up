/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FFStudio;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;

public class Enemy : MonoBehaviour
{
#region Fields
  [ Title( "Shared Variables" ) ]
    [ SerializeField ] SharedIntNotifier notif_stickman_power;
    [ SerializeField ] Currency notif_currency;
    [ SerializeField ] GameEvent event_stickman_lost;
    [ SerializeField ] IntGameEvent event_stickman_won;
    [ SerializeField ] ParticleSpawnEvent event_particle_hit;

  [ Title( "Setup" ) ]
    [ SerializeField ] int enemy_power;
    [ SerializeField ] UnityEvent enemy_lost;
    [ SerializeField ] UnityEvent enemy_won;

  [ Title( "Components" ) ]
    [ SerializeField ] ToggleRagdoll enemy_ragdoll;
    [ SerializeField ] Animator enemy_animator;
    [ SerializeField ] TextMeshProUGUI enemy_power_ui;

// Private
	ColliderTransmitMessage onPlayerCollided;

	Cooldown      cooldown_disable = new Cooldown();
	RecycledTween recycledTween    = new RecycledTween();

	Vector3 enemy_cell_position;
#endregion

#region Properties
#endregion

#region Unity API
	void Awake()
	{
		onPlayerCollided = ExtensionMethods.EmptyMethod;

		enemy_ragdoll.SwitchRagdoll( false );
		enemy_ragdoll.ToggleCollider( false );

		enemy_cell_position = transform.localPosition;
	}
#endregion

#region API
	public void OnPlayerTrigger( Collider enemy, Collider player ) // Info: Called from Enemy's own ragdoll collider 
	{
		onPlayerCollided( enemy, player );
	}

	public void EnableTriggerOnRagdoll() // Info: Called from Cell prefab
	{
		enemy_ragdoll.ToggleCollider( true );
		enemy_ragdoll.ToggleTriggerOnCollider( true );

		onPlayerCollided = PlayerTrigger;
	}

	public void OnStickmanLaunchFlipped( bool flipped ) //Info: If true: Stickman aiming towards left
	{
		Vector3 tweenPosition;

		if( flipped )
			tweenPosition = enemy_cell_position * -1f;
		else
			tweenPosition = enemy_cell_position;

		recycledTween.Recycle( transform.DOLocalMove( tweenPosition, GameSettings.Instance.enemy_flip_duration ).SetEase( GameSettings.Instance.enemy_flip_ease ) );

	}
#endregion

#region Implementation
	void PlayerTrigger( Collider enemy, Collider player ) 
	{
		onPlayerCollided = ExtensionMethods.EmptyMethod;
		recycledTween.Kill();

		event_particle_hit.Raise( "stickman_hit", enemy.transform.position );

		if( enemy_power > notif_stickman_power.sharedValue )
			OnWin();
		else
			OnLoose( enemy, ( enemy.transform.position - player.transform.position ).normalized );
	}

	void OnLoose( Collider collider, Vector3 direction )
	{
		enemy_power_ui.gameObject.SetActive( false );

		enemy_animator.enabled = false;

		enemy_ragdoll.ToggleTriggerOnCollider( false );
		enemy_ragdoll.SwitchRagdoll( true );

		var rigidbody = collider.GetComponent< Rigidbody >();
		rigidbody.AddForce( direction * GameSettings.Instance.enemy_defeat_force.ReturnRandom(), ForceMode.Impulse );

		cooldown_disable.Start( GameSettings.Instance.enemy_defeat_duration.ReturnRandom(), false, OnDefeatComplete );

		notif_currency.SharedValue += enemy_power * ( int )GameSettings.Instance.enemy_power_conversion_rate.ReturnRandom();
		notif_stickman_power.SharedValue += enemy_power;

		enemy_lost.Invoke();
		event_stickman_won.Raise( enemy_power );
	}

	void OnWin()
	{
		enemy_ragdoll.ToggleCollider( false );
		enemy_ragdoll.ToggleTriggerOnCollider( false );

		enemy_animator.SetTrigger( "victory" );

		enemy_won.Invoke();
		event_stickman_lost.Raise();
	}

	void OnDefeatComplete()
	{
		gameObject.SetActive( false );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
	void OnValidate()
	{
		enemy_power_ui.text = enemy_power.ToString();
	}

	public void SetPower( int power )
	{
		UnityEditor.EditorUtility.SetDirty( gameObject );

		enemy_power = power;
		enemy_power_ui.text = enemy_power.ToString();
	}
#endif
#endregion
}
