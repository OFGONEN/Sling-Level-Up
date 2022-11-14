/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
#region Fields (Settings)
    // Info: You can use Title() attribute ONCE for every game-specific group of settings.
    [ Title( "Stickman" ) ]
		[ LabelText( "Stickman Rise Height" ) ] public float stickman_rise_height;
		[ LabelText( "Stickman Rise Duration" ) ] public float stickman_rise_duration;
		[ LabelText( "Stickman Rise Ease" ) ] public Ease stickman_rise_ease;
		[ LabelText( "Stickman Launch Rotation Speed" ) ] public float stickman_launch_rotation_speed;
		[ LabelText( "Stickman Launch Rotation Speed" ), MinMaxSlider( 0, 1000 ) ] public Vector2 stickman_launch_power;


    [ Title( "Enemy" ) ]
		[ LabelText( "Force to give Enemy when its defated" ) ] public Vector2 enemy_defeat_force;
		[ LabelText( "Enemy defeated ragdoll duration" ) ] public Vector2 enemy_defeat_duration;
		[ LabelText( "Enemy Flip Duration" ) ] public float enemy_flip_duration;
		[ LabelText( "Enemy Flip Ease" ) ] public Ease enemy_flip_ease;
    
    [ Title( "Camera" ) ]
        [ LabelText( "Follow Speed" ), SuffixLabel( "units/seconds" ), Min( 0 ) ] public float camera_follow_speed;
        [ LabelText( "Follow Offset" ) ] public Vector3 camera_follow_offset;
        [ LabelText( "Max Zoom Out Value" ), Min( 0 ) ] public float camera_zoomOut_value_max;
        [ LabelText( "Max Zoom Out Range" ), MinMaxSlider( 0, 1 ) ] public Vector2 camera_zoomOut_value_range;
    
    [ Title( "Project Setup", "These settings should not be edited by Level Designer(s).", TitleAlignments.Centered ) ]
        public int maxLevelCount;
        [ LabelText( "Lateral Screen Percentage for Max Input Delta" ), SuffixLabel( "%" ), Range( 0, 1 ) ] public float game_input_maxDelta_percentage;
        [ LabelText( "Game's Forward" ) ] public Vector3 game_forward = Vector3.right;
        
        // Info: 3 groups below (coming from template project) are foldout by design: They should remain hidden.
		[ FoldoutGroup( "Remote Config" ) ] public bool useRemoteConfig_GameSettings;
        [ FoldoutGroup( "Remote Config" ) ] public bool useRemoteConfig_Components;

        [ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the movement for ui element"          ) ] public float ui_Entity_Move_TweenDuration;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the fading for ui element"            ) ] public float ui_Entity_Fade_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the scaling for ui element"           ) ] public float ui_Entity_Scale_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Duration of the movement for floating ui element" ) ] public float ui_Entity_FloatingMove_TweenDuration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Joy Stick"                                        ) ] public float ui_Entity_JoyStick_Gap;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Pop Up Text relative float height"                ) ] public float ui_PopUp_height;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "Pop Up Text float duration"                       ) ] public float ui_PopUp_duration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Random Spawn Area in Screen" ), SuffixLabel( "percentage" ) ] public float ui_particle_spawn_width; 
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Spawn Duration" ), SuffixLabel( "seconds" ) ] public float ui_particle_spawn_duration; 
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Spawn Ease" ) ] public Ease ui_particle_spawn_ease;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Wait Time Before Target" ) ] public float ui_particle_target_waitTime;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Target Travel Time" ) ] public float ui_particle_target_duration;
		[ FoldoutGroup( "UI Settings" ), Tooltip( "UI Particle Target Travel Ease" ) ] public Ease ui_particle_target_ease;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Percentage of the screen to register a swipe"     ) ] public int swipeThreshold;
        [ FoldoutGroup( "UI Settings" ), Tooltip( "Safe Area Base Top Offset" ) ] public int ui_safeArea_offset_top = 88;

    [ Title( "UI Particle" ) ]
		[ LabelText( "Random Spawn Area in Screen Witdh Percentage" ) ] public float uiParticle_spawn_width_percentage = 10;
		[ LabelText( "Spawn Movement Duration" ) ] public float uiParticle_spawn_duration = 0.1f;
		[ LabelText( "Spanwn Movement Ease" ) ] public DG.Tweening.Ease uiParticle_spawn_ease = DG.Tweening.Ease.Linear;
		[ LabelText( "Target Travel Wait Time" ) ] public float uiParticle_target_waitDuration = 0.16f;
		[ LabelText( "Target Travel Duration" ) ] public float uiParticle_target_duration = 0.4f;
		[ LabelText( "Target Travel Duration (REWARD)" ) ] public float uiParticle_target_duration_reward = 0.85f;
		[ LabelText( "Target Travel Ease" ) ] public DG.Tweening.Ease uiParticle_target_ease = DG.Tweening.Ease.Linear;


        [ FoldoutGroup( "Debug" ) ] public float debug_ui_text_float_height;
        [ FoldoutGroup( "Debug" ) ] public float debug_ui_text_float_duration;
#endregion

#region Fields (Singleton Related)
        static GameSettings instance;

        delegate GameSettings ReturnGameSettings();
        static ReturnGameSettings returnInstance = LoadInstance;

		public static GameSettings Instance => returnInstance();
#endregion

#region Implementation
        static GameSettings LoadInstance()
		{
			if( instance == null )
				instance = Resources.Load< GameSettings >( "game_settings" );

			returnInstance = ReturnInstance;

			return instance;
		}

		static GameSettings ReturnInstance()
        {
            return instance;
        }
#endregion
    }
}
