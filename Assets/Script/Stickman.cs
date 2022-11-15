/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Stickman : MonoBehaviour
{
#region Fields
  [ Title( "Setup" ) ]
    [ SerializeField ] SharedIntNotifier notif_stickman_power;
    [ SerializeField ] SharedFloat shared_finger_delta_magnitude;
    [ SerializeField ] SharedReferenceNotifier notif_stickman_target_reference;
    [ SerializeField ] StickmanPose[] stickman_pose_array;

  [ Title( "Fired Events" ) ]
	[ SerializeField ] GameEvent event_stickman_launch_start;
	[ SerializeField ] GameEvent event_stickman_launch_flipped;
	[ SerializeField ] GameEvent event_stickman_launch_end;

  [ Title( "Components" ) ]
	[ SerializeField ] Animator stickman_animator;
	[ SerializeField ] ToggleRagdoll stickman_ragdoll;
	[ SerializeField ] TextMeshProUGUI stickman_power_ui;
	[ SerializeField ] ParticleSystem particle_launch_update;
	[ SerializeField ] ParticleSystem particle_cell_entered;
	[ SerializeField ] ParticleSystem particle_cell_spawned;
	[ SerializeField ] Transform[] stickman_transform_array;
// Private
	Transform stickman_target_transform;
	Vector3 cell_position_previous;
	Vector3 cell_position_current;
	Enemy enemy_current;
	bool enemy_current_isOnRight;

    UnityMessage onUpdate;
    UnityMessage onFingerDown;
    UnityMessage onFingerUp;

	RecycledTween recycledTween  = new RecycledTween();
	Cooldown      cooldown_spawn = new Cooldown();
#endregion

#region Properties
#endregion

#region Unity API
    void OnDisable()
    {
		EmptyDelegates();
	}

    void Awake()
    {
		EmptyDelegates();

		stickman_ragdoll.SwitchRagdoll( false );
		stickman_ragdoll.ToggleCollider( false );
		stickman_ragdoll.ToggleTriggerOnCollider( false );

		cell_position_previous = transform.position - Vector3.up * GameSettings.Instance.stickman_cell_offset;
	}

	private void Update()
	{
		onUpdate();
	}
#endregion

#region API
    public void LevelStarted()
    {
		onFingerDown = Rise;

		stickman_target_transform = notif_stickman_target_reference.sharedValue as Transform;
	}

    public void OnFingerDown()
    {
		onFingerDown();
	}

    public void OnFingerUp()
    {
		onFingerUp();
	}

	public void OnStickmanEnteredCell( object value )
	{
		var enemy              = value as Enemy;  // Info: Enemy's +Y position will give us the Cell's +Y position as well
		    enemy_current = enemy;

		var enemyPosition = enemy.transform.position;

		particle_cell_entered.Play();
		ChangeIntoAttackPose();

		recycledTween.Recycle( transform.DOMove( enemyPosition + GameSettings.Instance.stickman_cell_enemy_attack_offset * Vector3.up,
			GameSettings.Instance.stickman_cell_enemy_attack_duration )
			.SetEase( GameSettings.Instance.stickman_cell_enemy_attack_ease ) );

		enemy_current_isOnRight = transform.position.x <= enemyPosition.x;

		if( enemy_current_isOnRight )
			cell_position_current = enemyPosition;
		else
			cell_position_current = enemyPosition.MultiplyX( -1f );
	}

	public void OnStickmanWon()
	{
		PushStickmanAwayFromEnemy();
		cooldown_spawn.Start( GameSettings.Instance.stickman_spawn_delay_cell, false, SpawnInCurrentCell );
	}

	public void OnStickmanLost()
	{
		PushStickmanAwayFromEnemy();
		cooldown_spawn.Start( GameSettings.Instance.stickman_spawn_delay_cell, false, SpawnInPreviousCell );
	}

	public void OnStickmanGround()
	{
		cooldown_spawn.Start( GameSettings.Instance.stickman_spawn_delay_ground, false, SpawnInPreviousCell );
	}
	}
#endregion

#region Implementation
	void SpawnInCurrentCell()
	{
		stickman_ragdoll.SwitchRagdoll( false );
		stickman_ragdoll.ToggleCollider( false );

		stickman_power_ui.gameObject.SetActive( true );

		cell_position_previous = cell_position_current;
		transform.position     = cell_position_current + Vector3.up * GameSettings.Instance.stickman_cell_offset;

		particle_cell_spawned.Play();
		stickman_animator.SetTrigger( "idle" );

		onFingerDown = Rise;
	}

	void SpawnInPreviousCell()
	{
		stickman_ragdoll.SwitchRagdoll( false );
		stickman_ragdoll.ToggleCollider( false );

		stickman_power_ui.gameObject.SetActive( true );

		transform.position = cell_position_previous + Vector3.up * GameSettings.Instance.stickman_cell_offset;

		particle_cell_spawned.Play();
		stickman_animator.SetTrigger( "idle" );

		onFingerDown = Rise;
	}
	void PushStickmanAwayFromEnemy()
	{
		stickman_ragdoll.SwitchRagdoll( true );
		stickman_ragdoll.ToggleCollider( true );
		stickman_ragdoll.ToggleTriggerOnCollider( false );

		var forceCofactor = enemy_current_isOnRight ? 1 : -1f;
		stickman_ragdoll.ApplyForce( GameSettings.Instance.stickman_cell_enemy_pushed_force.ReturnRandom() * Vector3.right * forceCofactor, ForceMode.Impulse );
	}

	void ChangeIntoAttackPose()
	{
		stickman_ragdoll.SwitchRagdoll( false );
		stickman_ragdoll.ToggleCollider( true );
		stickman_ragdoll.ToggleTriggerOnCollider( true );

		transform.position = stickman_ragdoll.MainRigidbody.position;
		ChangeStickmanPose( stickman_pose_array.ReturnRandom() );
	}

    void Rise()
    {
		onFingerDown.EmptyDelegate();

		stickman_ragdoll.ToggleCollider( true );
		stickman_ragdoll.BecomeMovableRagdoll();

		recycledTween.Recycle( transform.DOMove( GameSettings.Instance.stickman_rise_height * Vector3.up,
			GameSettings.Instance.stickman_rise_duration )
			.SetEase( GameSettings.Instance.stickman_rise_ease )
			.SetRelative()
			.OnComplete( OnRiseComplete ) );
	}

    void OnRiseComplete()
    {
		onFingerUp = Launch;
		event_stickman_launch_start.Raise(); // Launch direction target is on default position now.

		particle_launch_update.Play();

		onUpdate = OnLaunchUpdate;
	}

    void Launch()
    {
		EmptyDelegates();
		event_stickman_launch_end.Raise();

		particle_launch_update.Stop( true, ParticleSystemStopBehavior.StopEmitting );

		stickman_ragdoll.ApplyForce( transform.forward * GameSettings.Instance.stickman_launch_power.ReturnProgress( shared_finger_delta_magnitude.sharedValue ), ForceMode.Impulse );
	}

	void OnLaunchUpdate()
	{
		transform.LookAtOverTimeAxis( stickman_target_transform.position, Vector3.right, GameSettings.Instance.stickman_launch_rotation_speed );
	}

	void ChangeStickmanPose( StickmanPose pose )
	{
		for( var i = 0; i < stickman_transform_array.Length; i++ )
		{
			var stickmanTransform = stickman_transform_array[ i ];
			var poseData = pose.GetDataOnIndex( i );

			stickmanTransform.localPosition    = poseData.position;
			stickmanTransform.localEulerAngles = poseData.rotation;
			stickmanTransform.localScale       = poseData.scale;
		}
	}

    void EmptyDelegates()
    {
		onUpdate.EmptyDelegate();
		onFingerDown.EmptyDelegate();
		onFingerUp.EmptyDelegate();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
	[ Button() ]
	void CacheStickmanTransforms( Transform transform )
	{
		UnityEditor.EditorUtility.SetDirty( gameObject );

		stickman_transform_array = transform.GetComponentsInChildren< Transform >();
	}
#endif
#endregion
}