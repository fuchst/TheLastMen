using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson {
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonControllerSpherical : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
			public float JetpackMaxFlightDuration = 5.0f;
			//public KeyCode JetpackKey = KeyCode.LeftControl;
			public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;

#if !MOBILE_INPUT
            private bool m_Running;
#endif

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
	            if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
					CurrentTargetSpeed = StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
					CurrentTargetSpeed = BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
					CurrentTargetSpeed = ForwardSpeed;
				}
#if !MOBILE_INPUT
	            if (Input.GetKey(RunKey))
	            {
		            CurrentTargetSpeed *= RunMultiplier;
		            m_Running = true;
	            }
	            else
	            {
		            m_Running = false;
	            }
#endif
            }

#if !MOBILE_INPUT
            public bool Running
            {
                get { return m_Running; }
            }
#endif
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl = true; // can the user control the direction that is being moved in the air
        }


        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLookSpherical mouseLook = new MouseLookSpherical();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;
		
		private bool m_Fly;
		private float m_JetpackCurFlightDuration = 0;
        public bool m_Hooked;


        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }

        public bool Running
        {
            get
            {
 #if !MOBILE_INPUT
				return movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init (transform, cam.transform);
            m_Hooked = false;
        }


        private void Update()
        {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
            {
                m_Jump = true;
            }

			if (CrossPlatformInputManager.GetButtonDown ("Jetpack") && !m_Fly && m_JetpackCurFlightDuration < movementSettings.JetpackMaxFlightDuration) {
				m_Fly = true;
			}
			else if (CrossPlatformInputManager.GetButtonUp ("Jetpack") && m_Fly) {
				m_Fly = false;
			}
           
          
        }


        private void FixedUpdate()
        {
            GroundCheck();
            Vector2 input = GetInput();

			if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded) || m_Fly || m_Hooked)
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward*input.y + cam.transform.right*input.x;
				//Vector3 desiredMoveJetpack = (desiredMove.normalized) * movementSettings.CurrentTargetSpeed;
                desiredMove = m_Fly ? desiredMove.normalized : Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                desiredMove.x = desiredMove.x*movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z*movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y*movementSettings.CurrentTargetSpeed;

				desiredMove *= m_Fly? 1 : SlopeMultiplier();

                if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed*movementSettings.CurrentTargetSpeed) ||
				     m_RigidBody.velocity.sqrMagnitude > (m_RigidBody.velocity + desiredMove/m_RigidBody.mass).sqrMagnitude)
                {
					m_RigidBody.AddForce(desiredMove, ForceMode.Impulse);
                }
            }

			if (m_Fly){
				//m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, Mathf.Min(m_RigidBody.velocity.y + 30*Time.fixedDeltaTime, 3), m_RigidBody.velocity.z);
				//m_RigidBody.velocity = m_RigidBody.velocity + Vector3.up * Mathf.Min(30*Time.fixedDeltaTime, 3 - m_RigidBody.velocity.y);
				//transformation below is incorrect!! check!!
				//m_RigidBody.velocity = m_RigidBody.velocity + Vector3.up * Mathf.Min(30*Time.fixedDeltaTime, 3 - Vector3.Project(m_RigidBody.velocity, Vector3.up).magnitude);
				//m_RigidBody.velocity = m_RigidBody.velocity + transform.up * Mathf.Min(30*Time.fixedDeltaTime, 3 - Vector3.Project(m_RigidBody.velocity, transform.up).magnitude);

				Vector3 vel = m_RigidBody.velocity;
				Vector3 upV = transform.up;
				Vector3 velUp = Vector3.Project(vel, transform.up);
				float factorUp = (velUp.x != 0 && upV.x != 0) ? (velUp.x/upV.x) : ( (velUp.y != 0 && upV.y != 0) ? (velUp.y/upV.y) : (velUp.z/upV.z) );
				vel += upV * Mathf.Min(30*Time.fixedDeltaTime, 3 - factorUp);
				m_RigidBody.velocity = vel;

				m_JetpackCurFlightDuration += Time.fixedDeltaTime;
				if(m_JetpackCurFlightDuration > movementSettings.JetpackMaxFlightDuration){
					m_Fly = false;
				}
			}

            if (m_IsGrounded)
            {
                m_RigidBody.drag = 5f;
				m_JetpackCurFlightDuration = 0f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    //m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
					m_RigidBody.velocity -= Vector3.Project(m_RigidBody.velocity, transform.up);
                    //m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
					//m_RigidBody.AddForce(m_RigidBody.position.normalized * movementSettings.JumpForce, ForceMode.Impulse);
					m_RigidBody.AddForce(transform.up * movementSettings.JumpForce, ForceMode.Impulse);
                    m_Jumping = true;
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                m_RigidBody.drag = 0.5f;
                if (m_PreviouslyGrounded && !(m_Jumping || m_Fly))
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;
        }


        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, transform.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius, -transform.up, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {
            
            Vector2 input = new Vector2
                {
                    x = CrossPlatformInputManager.GetAxis("Horizontal"),
                    y = CrossPlatformInputManager.GetAxis("Vertical")
                };
			movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

			transform.up = transform.position.normalized;

            // get the rotation before it's changed
			//float oldYRotation = transform.eulerAngles.y;
			float oldYRotation = mouseLook.yAngleCur;

            mouseLook.LookRotation (transform, cam.transform);

            if (m_IsGrounded || advancedSettings.airControl || m_Fly)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                //Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, transform.up);
				Quaternion velRotation = Quaternion.AngleAxis(mouseLook.yAngleCur - oldYRotation, transform.up);
                m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
            }
        }


        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius, -transform.up, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = transform.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
    }
}
