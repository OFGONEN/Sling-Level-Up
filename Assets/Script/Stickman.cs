/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Stickman : MonoBehaviour
{
#region Fields
  [ Title( "Setup" ) ]
    [ SerializeField ] SharedIntNotifier notif_stickman_power;
    [ SerializeField ] SharedVector3Notifier notif_stickman_finishLine_spawn_position;
    [ SerializeField ] SharedFloat shared_finger_delta_magnitude;
    [ SerializeField ] SharedReferenceNotifier notif_stickman_target_reference;
    [ SerializeField ] StickmanPose[] stickman_pose_array;

  [ Title( "Fired Events" ) ]
	[ SerializeField ] GameEvent event_stickman_spawned;
	[ SerializeField ] GameEvent event_stickman_launch_start;
	[ SerializeField ] GameEvent event_stickman_launch_end;
	[ SerializeField ] GameEvent event_stickman_victory;
	[ SerializeField ] GameEvent event_level_completed;

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
	Vector3 cell_rotation;
	Vector3 cell_position_previous;
	Vector3 cell_position_current;
	Enemy enemy_current;
	bool enemy_current_isOnRight;

    UnityMessage onUpdate;
    UnityMessage onFingerDown;
    UnityMessage onFingerUp;
	UnityMessage onStickmanCollidedGround;

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
		cell_rotation          = transform.eulerAngles;
	}

	private void Start()
	{
		notif_stickman_power.SetValue_NotifyAlways( CurrentLevelData.Instance.levelData.stickman_power_start );

		stickman_power_ui.gameObject.SetActive( true );
		UpdateStickmanPowerUI();
	}

	private void Update()
	{
		onUpdate();
	}
#endregion

#region API
    public void OnLevelStarted()
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
		var enemyLocalPosition = enemy.transform.localPosition;

		particle_cell_entered.Play();
		ChangeIntoAttackPose();

		EmptyDelegates();
		cooldown_spawn.Kill();

		recycledTween.Recycle( transform.DOMove( enemyPosition + GameSettings.Instance.stickman_cell_enemy_attack_offset * Vector3.up,
			GameSettings.Instance.stickman_cell_enemy_attack_duration )
			.SetEase( GameSettings.Instance.stickman_cell_enemy_attack_ease ) );

		enemy_current_isOnRight = transform.position.x <= enemyPosition.x;
		cell_position_current = enemyPosition + enemyLocalPosition;

		// if( enemy_current_isOnRight )
		// 	cell_position_current = enemyPosition + enemyLocalPosition;
		// else
		// 	cell_position_current = enemyPosition + enemyLocalPosition;
	}

	public void OnStickmanWon()
	{
		UpdateStickmanPowerUI();
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
		onStickmanCollidedGround();
	}

	public void OnFinishLine()
	{
		EmptyDelegates();

		event_stickman_victory.Raise();
		cooldown_spawn.Start( GameSettings.Instance.stickman_spawn_delay_finishLine, false, SpawnOnTarget );
	}

	public void OnStickmanFlipped( bool value )
	{
		// if( value )
			// transform.localEulerAngles = transform.localEulerAngles.SetZ( 180 );
		// else
			// transform.localEulerAngles = transform.localEulerAngles.SetZ( 0 );
	}
#endregion

#region Implementation
	void StickmanCollideWithGround()
	{
		cooldown_spawn.Start( GameSettings.Instance.stickman_spawn_delay_ground, false, SpawnInPreviousCell );
		onStickmanCollidedGround = ExtensionMethods.EmptyMethod;
	}

	void UpdateStickmanPowerUI()
	{
		stickman_power_ui.text = notif_stickman_power.sharedValue.ToString();
	}

	void SpawnInCurrentCell()
	{
		stickman_ragdoll.SwitchRagdoll( false );
		stickman_ragdoll.ToggleCollider( false );

		stickman_animator.enabled = true;

		stickman_power_ui.gameObject.SetActive( true );

		cell_position_previous = cell_position_current;
		transform.position     = cell_position_current + Vector3.up * GameSettings.Instance.stickman_cell_offset;
		transform.eulerAngles  = cell_rotation;

		particle_cell_spawned.Play();
		stickman_animator.SetTrigger( "idle" );

		event_stickman_spawned.Raise();

		onFingerDown = Rise;
	}

	void SpawnInPreviousCell()
	{
		stickman_ragdoll.SwitchRagdoll( false );
		stickman_ragdoll.ToggleCollider( false );

		stickman_animator.enabled = true;

		stickman_power_ui.gameObject.SetActive( true );

		transform.position    = cell_position_previous + Vector3.up * GameSettings.Instance.stickman_cell_offset;
		transform.eulerAngles = cell_rotation;

		particle_cell_spawned.Play();
		stickman_animator.SetTrigger( "idle" );

		event_stickman_spawned.Raise();

		onFingerDown = Rise;
	}

	void SpawnOnTarget()
	{
		stickman_ragdoll.SwitchRagdoll( false );
		stickman_ragdoll.ToggleCollider( false );

		stickman_animator.enabled = true;

		stickman_power_ui.gameObject.SetActive( true );

		transform.position    = notif_stickman_finishLine_spawn_position.sharedValue + Vector3.up * GameSettings.Instance.stickman_cell_offset;
		transform.eulerAngles = cell_rotation;

		event_stickman_spawned.Raise();

		particle_cell_spawned.Play();
		stickman_animator.SetTrigger( "victory" );

		event_level_completed.Raise();
	}

	void PushStickmanAwayFromEnemy()
	{
		stickman_ragdoll.SwitchRagdoll( true );
		stickman_ragdoll.ToggleCollider( true );
		stickman_ragdoll.ToggleTriggerOnCollider( false );

		var forceCofactor = enemy_current_isOnRight ? -1f : 1f;
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
		EmptyDelegates();

		stickman_ragdoll.ToggleCollider( true );
		stickman_ragdoll.ToggleTriggerOnCollider( false );
		stickman_ragdoll.BecomeMovableRagdoll();

		particle_launch_update.Play();

		stickman_animator.enabled = false;

		recycledTween.Recycle( transform.DOMove( GameSettings.Instance.stickman_rise_height * Vector3.up,
			GameSettings.Instance.stickman_rise_duration )
			.SetEase( GameSettings.Instance.stickman_rise_ease )
			.SetRelative(), OnRiseComplete );
	}

    void OnRiseComplete()
    {
		onFingerUp = Launch;
		onUpdate   = OnLaunchUpdate;

		event_stickman_launch_start.Raise(); // Launch direction target is on default position now.
	}

    void Launch()
    {
		EmptyDelegates();

		onStickmanCollidedGround = StickmanCollideWithGround;

		event_stickman_launch_end.Raise();

		stickman_power_ui.gameObject.SetActive( false );

		particle_launch_update.Stop( true, ParticleSystemStopBehavior.StopEmitting );

		stickman_ragdoll.MakeMainRbDynamic();
		stickman_ragdoll.ApplyForce( transform.forward * GameSettings.Instance.stickman_launch_power.ReturnProgress( shared_finger_delta_magnitude.sharedValue ), ForceMode.Impulse );
	}

	void OnLaunchUpdate()
	{
		transform.LookAtOverTime( stickman_target_transform.position, GameSettings.Instance.stickman_launch_rotation_speed );
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
		onUpdate                 = ExtensionMethods.EmptyMethod;
		onFingerDown             = ExtensionMethods.EmptyMethod;
		onFingerUp               = ExtensionMethods.EmptyMethod;
		onStickmanCollidedGround = ExtensionMethods.EmptyMethod;
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