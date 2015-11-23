using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

//namespace UnityStandardAssets.Characters.FirstPerson
//{
    [Serializable]
    public class MouseLookSpherical
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
		private Transform m_helper;
		private float yAngle;

		public float yAngleCur {
			get { return yAngle; }
		}


        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
			m_helper = new GameObject("helper").transform;
			m_helper.SetParent(character.transform);
			m_helper.rotation = Quaternion.identity;
			m_helper.up = character.position.normalized;
			yAngle = 0;
        }


        public void LookRotation(Transform character, Transform camera)
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            //m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
			//m_CharacterTargetRot *= Quaternion.AngleAxis(yRot, character.position.normalized);
			//m_CharacterTargetRot = Quaternion.LookRotation(Vector3.up, character.position.normalized);
			//m_helper.rotation = Quaternion.identity;

			//m_helper.Rotate (Vector3.up, yRot, Space.Self);
			//m_helper.rotation *= Quaternion.AngleAxis (yRot, m_helper.up);

			yAngle += yRot;
			m_helper.rotation = Quaternion.identity;
			m_helper.up = character.position;
			//m_helper.rotation *= Quaternion.AngleAxis (yAngle, m_helper.up);
			m_helper.Rotate (Vector3.up, yAngle, Space.Self);

			//m_helper.Rotate (Vector3.up, 180*CrossPlatformInputManager.GetAxis("Mouse X"), Space.Self);
			m_CharacterTargetRot = m_helper.rotation;
            
			m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);
			//m_CameraTargetRot *= Quaternion.AngleAxis(-xRot, character.right);

            if(clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

            if(smooth)
            {
                character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
        }


        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
//}
