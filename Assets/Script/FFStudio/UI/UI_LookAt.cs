/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using Sirenix.OdinInspector;

namespace FFStudio
{
	public class UI_LookAt : MonoBehaviour
	{
#region Fields
	[ Title( "Setup" ) ]
        public SharedReferenceNotifier lookAt_Reference;
        public Vector3 lookAt_Axis;
        public bool can_override;
		public Vector3 lookAt_Axis_override;

		Transform lookAt_Transform;
		UnityMessage updateMethod;
#endregion

#region Properties
#endregion

#region Unity API
        void Awake()
        {
			updateMethod = ExtensionMethods.EmptyMethod;
		}

		void OnEnable()
		{
			lookAt_Transform = lookAt_Reference.SharedValue as Transform;
			updateMethod = LookAtTarget;
		}

		private void Start()
		{
			OnEnable();
		}

		void OnDisable()
		{
			updateMethod = ExtensionMethods.EmptyMethod;
		}

        void Update()
        {
			updateMethod();
		}
#endregion

#region API
		public void ManualDisable()
		{
			OnDisable();
		}
#endregion

#region Implementation
        void LookAtTarget()
        {
			transform.LookAtAxis( lookAt_Transform.position, lookAt_Axis, -1f );

			transform.eulerAngles = Vector3.Scale( transform.eulerAngles, lookAt_Axis_override );
		}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}