﻿using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLookFalcon
    {
        //Falcon Settings
        private HapticProbeFPS controller;
        private float XFalconSensitivity = 6f;
        private float YFalconSensitivity = 5f;

        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;
        private Vector2 lastAxis;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;

        public void Init(Transform character, Transform camera, HapticProbeFPS controller)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
            //inizializzazione controller falcon
            this.controller = controller;
        }

        public float getAxisXFalcon()
        {
            float axisX = 0;
            if (controller.isActive())
            {
                axisX = -(lastAxis.x - controller.getFalconPosition().x) * XFalconSensitivity;
                lastAxis.x = controller.getFalconPosition().x;
            }
            return axisX;
        }

        public float getAxisYFalcon()
        {
            float axisY = 0;
            if (controller.isActive())
            {
                axisY = -(lastAxis.y - controller.getFalconPosition().y) * YFalconSensitivity;
                lastAxis.y = controller.getFalconPosition().y;
            }
            return axisY;
        }

        public void LookRotation(Transform character, Transform camera)
        {

            float yRot;
            float xRot;

           //prendo input movimento visuale dal falcon
            if (controller.isActive())
            {
                yRot = getAxisXFalcon() * XSensitivity;
                xRot = getAxisYFalcon() * YSensitivity;
            }
            //prendo input movimento visuale dal mouse
            else
            {
                yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
                xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
            }
          
            aimHelper(ref yRot, ref xRot);

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }

            UpdateCursorLock();

            
        }

        private void aimHelper(ref float inputLookX,ref float inputLookY)
        {
            
            if (BasicAimHelper.I)
            {
                BasicAimHelper.I.AllowForAutoAim(ref inputLookX, ref inputLookY);
            }

        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if (!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
