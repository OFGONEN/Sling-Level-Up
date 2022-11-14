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

  [ Title( "Fired Events" ) ]
	[ SerializeField ] GameEvent event_stickman_launch_start;
	[ SerializeField ] GameEvent event_stickman_launch_flipped;
	[ SerializeField ] GameEvent event_stickman_launch_end;

  [ Title( "Components" ) ]
	[ SerializeField ] ToggleRagdoll stickman_ragdoll;
	[ SerializeField ] ParticleSystem particle_launch_update;
// Private
	Transform stickman_target_transform;

    UnityMessage onUpdate;
    UnityMessage onFingerDown;
    UnityMessage onFingerUp;

	RecycledTween recycledTween = new RecycledTween();
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

	public void OnStickmanEnteredCell()
	{
		
	}
#endregion

#region Implementation
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

    void EmptyDelegates()
    {
		onUpdate.EmptyDelegate();
		onFingerDown.EmptyDelegate();
		onFingerUp.EmptyDelegate();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}