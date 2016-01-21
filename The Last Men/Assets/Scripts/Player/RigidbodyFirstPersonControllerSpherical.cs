using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyFirstPersonControllerSpherical : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.5f;  // Speed when sprinting

            public float JetpackMaxFlightDuration = 5.0f;      //Maximum duration of "nonstop" jetpack thrust duration (resets when landing)
            public float JetpackHorizontalBaseSpeed = 8.0f;    //Base speed for horizontal flight (scaled by flight direction input)
            public float JetpackMaxVerticalSpeed = 8.0f;       //Maximum speed for vertical flight
            public float JetpackVerticalAcceleration = 30.0f;  //Vertical acceleration for the jetpack
            public bool JetpackFlyInLookingDir = false;

            public bool changeFOV = true;
            public float FOV_regular = 60.0f;
            public float FOV_flying = 85.0f;
            public float maxFOVchangeRate = 7.5f;
            public float minHeightAboveGroundForFOVChange = 2.5f;
            public float heightAboveGroundForMaxFOV = 25.0f;
            public LayerMask heightCheckLayers;

            public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector]
            public float CurrentTargetSpeed = 8f;

#if !MOBILE_INPUT
            //[HideInInspector] public bool m_RunningLock = false;    //No running if hooked
            private bool m_Running;
#endif
            //[HideInInspector] public bool m_JetpackLock = false;
			[HideInInspector] public bool m_Hooked = false;

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
                if (input != Vector2.zero)
                {
                    if (input.y > 0)
                    {
                        //forwards
                        CurrentTargetSpeed = ForwardSpeed;
                    }
                    else if (input.y < 0)
                    {
                        //backwards
                        CurrentTargetSpeed = BackwardSpeed;
                    }
                    else if (input.x > 0 || input.x < 0)
                    {
                        //strafe
                        CurrentTargetSpeed = StrafeSpeed;
                    }
                }
#if !MOBILE_INPUT
                if (!m_Hooked && Input.GetKey(RunKey))
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
            public bool airControl; // can the user control the direction that is being moved in the air
        }
        
        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLookSpherical mouseLook = new MouseLookSpherical();
        public AdvancedSettings advancedSettings = new AdvancedSettings();

        public bool swingFloR = false;
        float ropeSwingStrength = 5f;

        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        //private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;

        private bool m_Fly;
        private float m_JetpackCurFlightDuration = 0;
        //private bool m_swingimpulse;
        private float m_swingImpulseTimer = 0;
        private Vector3 prevVel = new Vector3(0,0,0);
        private Vector3 curVel = new Vector3(0, 0, 0);

        private new AudioPlayer audio;

        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public Vector3 GroundNormal
        {
            get { return m_GroundContactNormal; }
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
            mouseLook.Init(transform, cam.transform);
            //m_swingimpulse = true;
            audio = GetComponent<AudioPlayer>();
        }
        
        private void Update()
        {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
            {
                m_Jump = true;
            }

            if (!movementSettings.m_Hooked && CrossPlatformInputManager.GetButtonDown("Jetpack") && !m_Fly && s_GameManager.Instance.energyPlayer_Cur > 0) // && m_JetpackCurFlightDuration < movementSettings.JetpackMaxFlightDuration
            {
                m_Fly = true;
            }
            else if (m_Fly && CrossPlatformInputManager.GetButtonUp("Jetpack"))
            {
                m_Fly = false;
            }
        }

        private float Walk (Vector2 input) {
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
            //Vector3 desiredMoveJetpack = (desiredMove.normalized) * movementSettings.CurrentTargetSpeed;
            desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;
            desiredMove *= movementSettings.CurrentTargetSpeed * SlopeMultiplier();
            
            float horVel = Vector3.ProjectOnPlane(m_RigidBody.velocity, transform.up).magnitude;
            if (horVel < movementSettings.CurrentTargetSpeed)
            {
                m_RigidBody.AddForce(5 * Time.fixedDeltaTime * desiredMove, ForceMode.VelocityChange);
            }
            if(horVel > 0.1f)
            {
                //audio.UpdateWalkingState(true, desiredMove.magnitude);
                //audio.UpdateWalkingState(true, Vector3.ProjectOnPlane(m_RigidBody.velocity, transform.up).magnitude);
                //audio.UpdateWalkingState(true, desiredMove.magnitude);
                audio.UpdateWalkingState(true, horVel);
            }
            else
            {
                audio.UpdateWalkingState(false);
            }

            return 0;
        }

        private float SkyDive (Vector2 input) {
            //TODO: insert code for slightly influencing movement direction when falling/ungrounded
            Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
            desiredMove *= movementSettings.CurrentTargetSpeed * SlopeMultiplier();
            float horVel = Vector3.ProjectOnPlane(m_RigidBody.velocity, transform.up).magnitude;
            if (horVel < movementSettings.CurrentTargetSpeed/2)
            {
                m_RigidBody.AddForce(2 * Time.fixedDeltaTime * desiredMove, ForceMode.VelocityChange);
            }
            return 0;
        }

        private float Fly (Vector2 input) {
            if (s_GameManager.Instance.energyPlayer_Cur <= 0) {
                m_Fly = false;
                return 0;
            }

            float curRunMultiplier = movementSettings.Running ? movementSettings.RunMultiplier : 1;
            Vector3 desiredMove, force = Vector3.zero;
            if (movementSettings.JetpackFlyInLookingDir) {
                desiredMove = cam.transform.forward;
            } else {
                //desiredMove = (cam.transform.forward*input.y + cam.transform.right*input.x).normalized;
                //desiredMove = (transform.forward*input.y + transform.right*input.x).normalized;
                Vector2 modifiedInput = input.normalized;
                modifiedInput.x *= 0.5f;
                modifiedInput.y *= modifiedInput.y > 0 ? 1.5f : 0.25f;
                desiredMove = transform.forward * input.y + transform.right * input.x;
            }
            float curJetpackSpeed = movementSettings.JetpackHorizontalBaseSpeed * curRunMultiplier;
            desiredMove *= curJetpackSpeed;

            Vector3 horVel = Vector3.ProjectOnPlane(m_RigidBody.velocity, transform.up);
            if (horVel.sqrMagnitude < (curJetpackSpeed * curJetpackSpeed) ||
                horVel.sqrMagnitude > (horVel + 5 * Time.fixedDeltaTime * desiredMove).sqrMagnitude)
            {
                force += 5 * Time.fixedDeltaTime * desiredMove;
                //m_RigidBody.AddForce(5*Time.fixedDeltaTime * desiredMove, ForceMode.VelocityChange);
                //energyCost += desiredMove.magnitude;
            }

            //m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, Mathf.Min(m_RigidBody.velocity.y + 30*Time.fixedDeltaTime, 3), m_RigidBody.velocity.z);
            //m_RigidBody.velocity = m_RigidBody.velocity + Vector3.up * Mathf.Min(30*Time.fixedDeltaTime, 3 - m_RigidBody.velocity.y);

            Vector3 upV = transform.up;
            Vector3 velUp = Vector3.Project(m_RigidBody.velocity + force, transform.up);
            //compute the "upwards" (away from world center) speed - compare current upwards direction vector with the projected velocity component-wise
            //for computing a meaningful factor, you need to pick a component (if possible) that is not 0 for both vectors - (velUp.x != 0 && upV.x != 0) or shorter (velUp.x * upV.x != 0)
            float factorUp = (velUp.x * upV.x != 0) ? (velUp.x / upV.x) : ((velUp.y * upV.y != 0) ? (velUp.y / upV.y) : ((velUp.z * upV.z != 0) ? (velUp.z / upV.z) : 0));
            float maxUpSpeed = movementSettings.JetpackMaxVerticalSpeed * curRunMultiplier;
            float upDelta = Mathf.Min(+movementSettings.JetpackVerticalAcceleration * Time.fixedDeltaTime, maxUpSpeed - factorUp);
            //m_RigidBody.velocity += upV * upDelta;
            //energyCost += Mathf.Max (upDelta, 0);
            m_RigidBody.AddForce(upV * upDelta + force, ForceMode.VelocityChange);

            //energyCost += (force + (upDelta > 0 ? upV * upDelta : Vector3.zero)).magnitude;

            m_JetpackCurFlightDuration += Time.fixedDeltaTime;
            /*if (m_JetpackCurFlightDuration > movementSettings.JetpackMaxFlightDuration) {
                m_Fly = false;
            }*/

            return Time.fixedDeltaTime * (force + (upDelta > 0 ? upV * upDelta : Vector3.zero)).magnitude;
        }

        private float Swing (Vector2 input) {
			prevVel = curVel;
			curVel = m_RigidBody.velocity;
			
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
            //Vector3 desiredMoveJetpack = (desiredMove.normalized) * movementSettings.CurrentTargetSpeed;
            desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;
            desiredMove *= movementSettings.CurrentTargetSpeed * SlopeMultiplier();

            //compute angle between vector from hook to person and hook to center of world.
            //SpringJoint joint = GetComponent<SpringJoint>();
            ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
            Vector3 hookPos = joint.connectedBody.transform.position;
            Vector3 hookToPlayer = hookPos - transform.position;
            
            if (swingFloR) {
                #region Flo_R
                Vector3 referenceRight = Vector3.Cross(Vector3.up, hookPos);
                float angle = Vector3.Angle(hookToPlayer, hookPos);
                float sign = Mathf.Sign(Vector3.Dot(hookToPlayer, referenceRight));
                float finalAngle = sign * angle;

                //Give impulse only if we want to swing
                if (finalAngle < 5 && finalAngle > 0)
                {
                    m_RigidBody.AddForce(2.0f * ropeSwingStrength * Time.fixedDeltaTime * desiredMove, ForceMode.VelocityChange);
                    m_swingImpulseTimer += Time.fixedDeltaTime;
                }

                //allow force into opposite direction
                float angle2 = Vector3.Angle(prevVel, desiredMove);
                //Debug.Log("angle: " + angle);
                if (angle2 < 30)
                {
                    m_RigidBody.AddForce(0.2f * ropeSwingStrength * Time.fixedDeltaTime * desiredMove, ForceMode.VelocityChange);
                    m_swingImpulseTimer += Time.fixedDeltaTime;
                }
                #endregion
            }

            /*
            if (Vector3.Angle(transform.forward, htp) < 95) {
                m_RigidBody.AddForce(10 * Time.fixedDeltaTime * desiredMove, ForceMode.VelocityChange);
                //m_swingimpulse = false;
                // Debug.Log("Swingimpuls given");
                m_swingImpulseTimer += Time.fixedDeltaTime;
            }
            */

            else {
			    #region Flo_W
                Vector3 desiredMoveY, desiredMoveX;
                desiredMoveY = Vector3.ProjectOnPlane(cam.transform.forward, hookToPlayer).normalized * input.y;
                desiredMoveY *= movementSettings.ForwardSpeed;
                desiredMoveX = Vector3.ProjectOnPlane(cam.transform.right, hookToPlayer).normalized * input.x;
                desiredMoveX *= movementSettings.StrafeSpeed;

                //only add swinging forces if we are lower than the hook, 
                if (Vector3.Angle(transform.up, hookToPlayer) < 85) {
                    
                    if (Vector3.Angle(Mathf.Sign(input.y) * transform.forward, hookToPlayer) < 95) {
                        m_RigidBody.AddForce(ropeSwingStrength * Time.fixedDeltaTime * desiredMoveY, ForceMode.VelocityChange);
                    }
                
                    if (Vector3.Angle(Mathf.Sign(input.x) * transform.right, hookToPlayer) < 95) {
                        m_RigidBody.AddForce(ropeSwingStrength * Time.fixedDeltaTime * desiredMoveX, ForceMode.VelocityChange);
                    }
                }
			    #endregion
            }

            return 0;
        }

        public void Jump (bool keepVerticalVelocity = false) {
            m_RigidBody.drag = 0.5f;
            if (!keepVerticalVelocity) {
                m_RigidBody.velocity -= Vector3.Project(m_RigidBody.velocity, transform.up);
            }
            m_RigidBody.AddForce(transform.up * 5 * Time.fixedDeltaTime * movementSettings.JumpForce, ForceMode.VelocityChange);
            m_Jumping = true;
        }

        protected void UpdateFOV () {
            float distToGround;
            RaycastHit hitInfo;

            if (m_IsGrounded) {
                distToGround = 0;
            }
            else if(Physics.Raycast(transform.position - transform.up, -transform.up, out hitInfo, movementSettings.heightAboveGroundForMaxFOV, movementSettings.heightCheckLayers, QueryTriggerInteraction.Ignore)) {
                distToGround = hitInfo.distance;
            }
            else {
                distToGround = movementSettings.heightAboveGroundForMaxFOV;
            }
            
            if(distToGround < movementSettings.minHeightAboveGroundForFOVChange) {
                distToGround = 0;
            }

            float FOVBase = Mathf.Lerp(movementSettings.FOV_regular, movementSettings.FOV_flying, distToGround/movementSettings.heightAboveGroundForMaxFOV);
            float FOVModifierLookDir = Vector3.Angle(transform.up, cam.transform.forward) - 90;
            FOVModifierLookDir = 0.05f * (Mathf.Abs(FOVModifierLookDir) < 25 ? 0 : (FOVModifierLookDir < 0 ? -2.5f * (FOVModifierLookDir + 25) : 1.5f * (FOVModifierLookDir - 25)));
            float FOVModifierRun = Running ? 5 : 0;
            float targetFOV = Mathf.Clamp(FOVBase + FOVModifierLookDir + FOVModifierRun, movementSettings.FOV_regular, movementSettings.FOV_flying);
            
            cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, targetFOV, movementSettings.maxFOVchangeRate * Time.fixedDeltaTime);
            
        }

        private void FixedUpdate () {
            GroundCheck();
            Vector2 input = GetInput();
            float energyCost = 0;
            bool inputExists = (Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon);

            if (movementSettings.changeFOV) {
                UpdateFOV();
            }

            if (m_Fly) {
                energyCost += Fly(input);
            }

            if (movementSettings.m_Hooked && !m_IsGrounded && inputExists) {
                energyCost += Swing(input);
            }

            if (!m_Fly && m_IsGrounded && inputExists) {
                energyCost += Walk(input);
            }
            else {
                audio.UpdateWalkingState(false);
            }

            if(!m_IsGrounded && !m_Fly && !movementSettings.m_Hooked && advancedSettings.airControl && inputExists) {
                energyCost += SkyDive(input);
            }

            if (m_IsGrounded) {
                m_RigidBody.drag = 5f;
                m_JetpackCurFlightDuration = 0f;

                if (m_Jump) {
                    Jump();
                }

                if (!m_Fly && !m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f) {
                    m_RigidBody.Sleep();
                }
            }
            else {
                m_RigidBody.drag = 0.5f;
                if (m_PreviouslyGrounded && !(m_Jumping || m_Fly)) {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;

            if (energyCost > 0) {
                //Debug.Log("consumed " + energyCost + " energy in this fixedUpdate");
                s_GameManager.Instance.ConsumeEnergy(energyCost);
            }
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
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) +
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

            //transform.up = transform.position.normalized;

            // get the rotation before it's changed
            //float oldYRotation = transform.eulerAngles.y;
            float oldYRotation = mouseLook.yAngleCur;

            mouseLook.LookRotation(transform, cam.transform);

            if (m_IsGrounded || m_Fly)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                //Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, transform.up);
                Quaternion velRotation = Quaternion.AngleAxis(mouseLook.yAngleCur - oldYRotation, transform.up);
                m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
            }
        }


        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius, -transform.up, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = transform.up;
            }
            //Player landed
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;

                /*Bastion bastion = hitInfo.collider.GetComponentInParent<Bastion>();
                if (bastion != null)
                {
                    bastion.PlayerLanded();
                }*/
            }
        }
        public void SetHooked(bool hooked)
        {
            movementSettings.m_Hooked = hooked;
            m_Fly = !hooked && m_Fly;
/*#if !MOBILE_INPUT
            movementSettings.m_RunningLock = hooked;
#endif
            movementSettings.m_JetpackLock = hooked;*/
        }

        /*public static float AngleSigned (Vector3 v1, Vector3 v2, Vector3 n) {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }*/
    }
}