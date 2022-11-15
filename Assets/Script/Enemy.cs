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
    [ SerializeField ] GameEvent event_stickman_lost;
    [ SerializeField ] IntGameEvent event_stickman_won;

  [ Title( "Setup" ) ]
    [ SerializeField ] int enemy_power;
    [ SerializeField ] UnityEvent enemy_lost;
    [ SerializeField ] UnityEvent enemy_won;

  [ Title( "Components" ) ]
    [ SerializeField ] ToggleRagdoll enemy_ragdoll;
    [ SerializeField ] Animator enemy_animator;
    [ SerializeField ] TextMeshProUGUI enemy_power_ui;

// Private
	Cooldown      cooldown_disable = new Cooldown();
	RecycledTween recycledTween    = new RecycledTween();

	Vector3 enemy_cell_position;
#endregion

#region Properties
#endregion

#region Unity API
	void Awake()
	{
		enemy_ragdoll.SwitchRagdoll( false );
		enemy_ragdoll.ToggleCollider( false );

		enemy_cell_position = transform.localPosition;
	}
#endregion

#region API
	public void OnPlayerTrigger( Collider enemy, Collider player ) // Info: Called from Enemy's own ragdoll collider 
	{
		if( enemy_power >= notif_stickman_power.sharedValue )
			OnWin();
		else
			OnLoose( enemy, ( enemy.transform.position - player.transform.position ).normalized );
	}

	public void EnableTriggerOnRagdoll() // Info: Called from Cell prefab
	{
		enemy_ragdoll.ToggleCollider( true );
		enemy_ragdoll.ToggleTriggerOnCollider( true );
	}

	public void OnStickmanLaunchFlipped()
	{
		recycledTween.Recycle( transform.DOLocalMove( -enemy_cell_position, GameSettings.Instance.enemy_flip_duration ).SetEase( GameSettings.Instance.enemy_flip_ease ) );
	}
#endregion

#region Implementation
	void OnLoose( Collider collider, Vector3 direction )
	{
		enemy_power_ui.gameObject.SetActive( false );

		enemy_animator.enabled = false;

		enemy_ragdoll.ToggleTriggerOnCollider( false );
		enemy_ragdoll.SwitchRagdoll( true );

		var rigidbody = collider.GetComponent< Rigidbody >();
		rigidbody.AddForce( direction * GameSettings.Instance.enemy_defeat_force.ReturnRandom(), ForceMode.Impulse );

		cooldown_disable.Start( GameSettings.Instance.enemy_defeat_duration.ReturnRandom(), false, OnDefeatComplete );

		notif_stickman_power.SharedValue += enemy_power;
		event_stickman_won.Raise( enemy_power );
	}

	void OnWin()
	{
		enemy_ragdoll.ToggleCollider( false );
		enemy_ragdoll.ToggleTriggerOnCollider( false );

		enemy_animator.SetTrigger( "victory" );

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
		if( enemy_power_ui != null )
			enemy_power_ui.text = enemy_power.ToString();
	}
#endif
#endregion
}
