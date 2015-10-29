/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
* @author Dogukan Erenel (derenel@us.ibm.com)
*/

using UnityEngine;
using TouchScript.Gestures;
using IBM.Watson.Logging;
using IBM.Watson.Utilities;
using IBM.Watson.Widgets.Question;

namespace IBM.Watson.Widgets.Avatar
{
    /// <summary>
    /// 
    /// </summary>
    public class AvatarTouchManager : MonoBehaviour
    {
        private GameObject m_cubeObject;
        public GameObject cubeObject
        {
            get
            {
                if (m_cubeObject == null || m_cubeObject.transform == null)
                {
                    GameObject[] questionObjects = GameObject.FindGameObjectsWithTag("QuestionOnFocus");
                    for (int i = 0; i < questionObjects.Length; i++)
                    {
                        if (questionObjects[i] != null && questionObjects[i].transform != null)
                        {
                            m_cubeObject = questionObjects[i].GetComponentInChildren<CubeAnimationManager>().gameObject;
                            break;
                        }
                    }
                }

                return m_cubeObject;
            }
        }

        public ScreenTransformGesture OneFingerManipulationGesture;
        public ScreenTransformGesture TwoFingerMoveGesture;
        public TapGesture tapGesture;

        public LayerMask layerForQuestionWidget;
        public float PanSpeed = 100.0f;
        public float RotationSpeed = 100.0f;
        public float ZoomSpeed = 0.1f;

        public LeanTweenType easeCameraReset = LeanTweenType.easeInOutCubic;
        public float timeForCameraResetAnimation = 1.0f;
        public float speedForCameraAnimation = 1f;
        public float speedForCubeRotationAnimation = 1f;

        private Vector3 initialCameraLocation;
        private Vector3 targetCameraLocation;
        private Quaternion targetCubeRotation;

        private bool isActive = true;
        private GameObject mainCamera;

        private void OnEnable()
        {
            Log.Status("AvatarTouchManager", "OnEnable");
            mainCamera = UnityEngine.Camera.main.gameObject;
            initialCameraLocation = mainCamera.transform.localPosition;
            targetCameraLocation = initialCameraLocation;
            targetCubeRotation = Quaternion.identity;

            TwoFingerMoveGesture.Transformed += twoFingerTransformHandler;
            OneFingerManipulationGesture.Transformed += oneFingerManipulationTransformedHandler;
            tapGesture.Tapped += TapGesture_Tapped;
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, CubeAnimationStateChanged);
        }

        private void FocusOnSide(Transform hitTransform)
        {
            int touchedSide = 0;
            int.TryParse(hitTransform.name.Substring(1, 1), out touchedSide);
            CubeAnimationManager.instance.FocusOnSide((CubeAnimationManager.CubeSideType)touchedSide);
        }

        private void TapGesture_Tapped(object sender, System.EventArgs e)
        {
            if (CubeAnimationManager.instance == null)
                return;

            Log.Status("AvatarTouchManager", "TapGesture_Tapped: {0}", tapGesture.ScreenPosition);
            Ray rayForTab = mainCamera.GetComponent<UnityEngine.Camera>().ScreenPointToRay(tapGesture.ScreenPosition);

            RaycastHit hit;
            if (Physics.Raycast(rayForTab, out hit, Mathf.Infinity, layerForQuestionWidget))
            {
                //Touch on side
                switch (CubeAnimationManager.instance.currentAnimationState)
                {
                    case CubeAnimationManager.CubeAnimationState.NotPresent:
                        break;
                    case CubeAnimationManager.CubeAnimationState.ComingToScene:
                        break;
                    case CubeAnimationManager.CubeAnimationState.IdleOnScene:
                        CubeAnimationManager.instance.UnFold();
                        break;
                    case CubeAnimationManager.CubeAnimationState.UnFolding:
                        FocusOnSide(hit.transform);
                        break;
                    case CubeAnimationManager.CubeAnimationState.Unfolded:
                        FocusOnSide(hit.transform);
                        break;
                    case CubeAnimationManager.CubeAnimationState.Folding:
                        CubeAnimationManager.instance.UnFold();
                        break;
                    case CubeAnimationManager.CubeAnimationState.FocusingToSide:
                        FocusOnSide(hit.transform);
                        break;
                    case CubeAnimationManager.CubeAnimationState.FocusedToSide:
                        FocusOnSide(hit.transform);
                        break;
                    case CubeAnimationManager.CubeAnimationState.GoingFromScene:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Log.Status("AvatarTouchManager", "Touch-outside current state: " + CubeAnimationManager.instance.currentAnimationState);
                //Touch out-side
                switch (CubeAnimationManager.instance.currentAnimationState)
                {
                    case CubeAnimationManager.CubeAnimationState.NotPresent:
                        break;
                    case CubeAnimationManager.CubeAnimationState.ComingToScene:
                        break;
                    case CubeAnimationManager.CubeAnimationState.IdleOnScene:
                        break;
                    case CubeAnimationManager.CubeAnimationState.UnFolding:
                        CubeAnimationManager.instance.Fold();
                        break;
                    case CubeAnimationManager.CubeAnimationState.Unfolded:
                        CubeAnimationManager.instance.Fold();
                        break;
                    case CubeAnimationManager.CubeAnimationState.Folding:
                        break;
                    case CubeAnimationManager.CubeAnimationState.FocusingToSide:
                        CubeAnimationManager.instance.UnFocus();
                        break;
                    case CubeAnimationManager.CubeAnimationState.FocusedToSide:
                        CubeAnimationManager.instance.UnFocus();
                        break;
                    case CubeAnimationManager.CubeAnimationState.GoingFromScene:
                        break;
                    default:
                        break;
                }
            }

        }

        private void OnDisable()
        {
            Log.Status("AvatarTouchManager", "OnDisable");

            TwoFingerMoveGesture.Transformed -= twoFingerTransformHandler;
            OneFingerManipulationGesture.Transformed -= oneFingerManipulationTransformedHandler;
            tapGesture.Tapped -= TapGesture_Tapped;
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_STATE_QUESTIONCUBE_ANIMATION, CubeAnimationStateChanged);
        }

        private void oneFingerManipulationTransformedHandler(object sender, System.EventArgs e)
        {
            Log.Status("AvatarTouchManager", "oneFingerManipulationTransformedHandler: {0}", OneFingerManipulationGesture.DeltaPosition);

            Quaternion rotation = Quaternion.Euler(OneFingerManipulationGesture.DeltaPosition.y / Screen.height * RotationSpeed,
                                                    -OneFingerManipulationGesture.DeltaPosition.x / Screen.width * RotationSpeed,
                                                    0.0f);

            targetCubeRotation *= rotation;
            Log.Status("AvatarTouchManager", "Rotation: {0} , Target rotation : {1} ", rotation.eulerAngles, targetCubeRotation.eulerAngles);
            // cubeObject.transform.rotation *= rotation;
            //cam.LookAt (camTarget.transform.position);

        }

        private void twoFingerTransformHandler(object sender, System.EventArgs e)
        {
            Log.Status("AvatarTouchManager", "twoFingerTransformHandler: {0} , DeltaScale: {1}, PanSpeed: {2}, ZoomSpeed:{3}",
                TwoFingerMoveGesture.DeltaPosition,
                TwoFingerMoveGesture.DeltaScale,
                PanSpeed,
                ZoomSpeed);

            targetCameraLocation += (TwoFingerMoveGesture.DeltaPosition * PanSpeed * -1.0f);

            targetCameraLocation += mainCamera.transform.forward * (TwoFingerMoveGesture.DeltaScale - 1.0f) * ZoomSpeed;
        }

        private void CubeAnimationStateChanged(System.Object[] args)
        {
            isActive = false;
            targetCameraLocation = initialCameraLocation;
            targetCubeRotation = Quaternion.identity;
            //Reset all camera animation.
            LeanTween.moveLocal(mainCamera.gameObject, initialCameraLocation, timeForCameraResetAnimation).setEase(easeCameraReset).setOnComplete(() =>
            {
                isActive = true;
            });
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Alpha3))
                PanSpeed -= 0.1f;
            if (Input.GetKey(KeyCode.Alpha4))
                PanSpeed += 0.1f;
            if (Input.GetKey(KeyCode.Alpha5))
                ZoomSpeed -= 0.1f;
            if (Input.GetKey(KeyCode.Alpha6))
                ZoomSpeed += 0.1f;

            if (isActive && CubeAnimationManager.instance != null)
            {
                if (CubeAnimationManager.instance.currentAnimationState == CubeAnimationManager.CubeAnimationState.IdleOnScene)
                {
                    //For Rotating the cube
                    targetCubeRotation = Quaternion.Lerp(targetCubeRotation, Quaternion.identity, Time.deltaTime * speedForCubeRotationAnimation);
                    cubeObject.transform.Rotate(targetCubeRotation.eulerAngles, Space.World);
                }

                //For Zooming
                mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, targetCameraLocation, Time.deltaTime * speedForCameraAnimation);
            }


        }

    }

}