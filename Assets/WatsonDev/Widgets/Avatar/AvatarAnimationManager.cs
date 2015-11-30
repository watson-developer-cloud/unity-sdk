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
*/


using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using IBM.Watson.Utilities;

namespace IBM.Watson.Widgets.Avatar
{
    /// <summary>
    /// Avatar animation manager.
    /// All animations related with Avatar is listed here. 
    /// </summary>
    public class AvatarAnimationManager : WatsonBaseAnimationManager
    {

        [SerializeField, FormerlySerializedAs("rotationVector")]
        private Vector3 m_RotationVector = new Vector3(0.0f, 1.0f, 0.0f);
        [SerializeField, FormerlySerializedAs("rotationSpeed")]
        private float m_RotationSpeed = 5.0f;

        [SerializeField]
        private Vector3 m_AvatarMoveDown;
        [SerializeField]
        private LeanTweenType m_EaseOnAvatarMovement = LeanTweenType.easeInOutCubic;

        #region Private Members
        private float m_AnimationSpeedModifier = 1.0f;

        [SerializeField]
        private float m_AnimationTime = 1.0f;
        private float m_AnimationInitialTime;

        private int m_AnimationRotation = -1;
        private int m_AnimationMoveDefault = -1;
        private int m_AnimationMoveDown = -1;
        #endregion

        #region OnEnable / OnDisable For Event Registration

        void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_AVATAR_MOVE_DEFAULT, AnimateMoveDefault);
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_AVATAR_MOVE_DOWN, AnimateMoveDown);
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_AVATAR_STOP_MOVE, StopAnimationMove);
        }

        void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_AVATAR_MOVE_DEFAULT, AnimateMoveDefault);
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_AVATAR_MOVE_DOWN, AnimateMoveDown);
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_AVATAR_STOP_MOVE, StopAnimationMove);
        }

        #endregion

        #region Awake / Start

        void Awake()
        {
            m_AnimationInitialTime = m_AnimationTime;
        }
        // Use this for initialization
        void Start()
        {
            //transform.Rotate(m_RotationVector * Time.deltaTime * m_Speed);

            AnimateRotation();
            //AnimateMoveDown ();
        }

        #endregion



        #region Overriden function on WatsonAnimationManager

        protected override void OnAnimationStop()
        {
            StopAllAnimations();
        }

        protected override void OnAnimationPause()
        {
            if (LeanTween.descr(m_AnimationRotation) != null)
            {
                LeanTween.descr(m_AnimationRotation).pause();
            }
            if (LeanTween.descr(m_AnimationMoveDefault) != null)
            {
                LeanTween.descr(m_AnimationMoveDefault).pause();
            }
            if (LeanTween.descr(m_AnimationMoveDown) != null)
            {
                LeanTween.descr(m_AnimationMoveDown).pause();
            }
        }

        protected override void OnAnimationResume()
        {
            if (LeanTween.descr(m_AnimationRotation) != null)
            {
                LeanTween.descr(m_AnimationRotation).resume();
            }
            if (LeanTween.descr(m_AnimationMoveDefault) != null)
            {
                LeanTween.descr(m_AnimationMoveDefault).resume();
            }
            if (LeanTween.descr(m_AnimationMoveDown) != null)
            {
                LeanTween.descr(m_AnimationMoveDown).resume();
            }
        }

        protected override void OnAnimationSpeedChange(float speedModifier)
        {
            m_AnimationSpeedModifier = speedModifier;
            m_AnimationTime = m_AnimationInitialTime * (1.0f / speedModifier);

            if (LeanTween.descr(m_AnimationMoveDefault) != null)
            {
                LeanTween.descr(m_AnimationMoveDefault).setTime(m_AnimationTime);
            }

            if (LeanTween.descr(m_AnimationMoveDown) != null)
            {
                LeanTween.descr(m_AnimationMoveDown).setTime(m_AnimationTime);
            }
        }

        #endregion

        #region All Animation - Stop / Speed Up - Down

        void StopAllAnimations()
        {
            StopAnimateRotation();
            StopAnimateMoveDefault();
            StopAnimateMoveDown();
        }

        void SetupAnimationSpeedModifier(float speedMofier)
        {
            m_AnimationSpeedModifier = speedMofier;
        }

        #endregion

        #region Avatar Rotation Animation

        void AnimateRotation()
        {
            if (LeanTween.descr(m_AnimationRotation) == null)
            {
                m_AnimationRotation = LeanTween.value(gameObject, 0.0f, 10.0f, 10.0f).setLoopType(LeanTweenType.linear).setOnUpdate((float f) =>
                {
                    transform.Rotate(m_RotationVector * Time.deltaTime * m_RotationSpeed * m_AnimationSpeedModifier);
                }).id;
            }
            else
            {
                //do nothing		
            }
        }

        void StopAnimateRotation()
        {
            if (LeanTween.descr(m_AnimationRotation) != null)
            {
                LeanTween.cancel(m_AnimationRotation);
                m_AnimationRotation = 0;
            }
        }


        #endregion

        #region Avatar Movements Up / Down / Left / Right - Event

        private void StopAnimationMove(System.Object[] args = null)
        {
            StopAnimateMoveDefault();
            StopAnimateMoveDown();
        }


        public void AnimateMoveDefault(System.Object[] args = null)
        {
            StopAnimationMove();

            float animationTime = m_AnimationTime;
            if (args != null && args.Length == 1 && float.TryParse(args[0].ToString(), out animationTime))
            {
                //animation time come as argument

            }

            m_AnimationMoveDefault = LeanTween.moveLocalY(gameObject, 0.0f, animationTime).setEase(m_EaseOnAvatarMovement).id;

        }

        public void StopAnimateMoveDefault(System.Object[] args = null)
        {
            if (LeanTween.descr(m_AnimationMoveDefault) != null)
            {
                LeanTween.cancel(m_AnimationMoveDefault);
                m_AnimationMoveDefault = -1;
            }
        }

        public void AnimateMoveDown(System.Object[] args = null)
        {
            StopAnimationMove();

            float animationTime = m_AnimationTime;
            if (args != null && args.Length == 1 && float.TryParse(args[0].ToString(), out animationTime))
            {
                //animation time come as argument

            }

            m_AnimationMoveDown = LeanTween.moveLocalY(gameObject, m_AvatarMoveDown.y, animationTime).setEase(m_EaseOnAvatarMovement).id;
        }

        public void StopAnimateMoveDown(System.Object[] args = null)
        {
            if (LeanTween.descr(m_AnimationMoveDown) != null)
            {
                LeanTween.cancel(m_AnimationMoveDown);
                m_AnimationMoveDown = -1;
            }
        }

        #endregion
    }

}
