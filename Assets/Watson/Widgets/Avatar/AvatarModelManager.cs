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
using IBM.Watson.Logging;
using IBM.Watson.Utilities;

namespace IBM.Watson.Widgets.Avatar
{
    /// <summary>
    /// Base Class for all Avatar Model managers. 
    /// </summary>
    public class AvatarModelManager : MonoBehaviour
    {
        #region Private Variables
        /// <summary>
        /// AvatarWidget attached to the parent object.
        /// </summary>
        protected AvatarWidget m_AvatarWidgetAttached;
        #endregion

        #region OnEnable / OnDisable / OnApplicationQuit / Awake

        /// <summary>
        /// OnEnable of the AvatarModelManager, mood and avatar state change functions are registered to catch the changes on them.
        /// </summary>
        protected virtual void OnEnable()
        {
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, OnChangedMood);
            EventManager.Instance.RegisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_STATE_FINISH, OnChangedBehavior);
        }

        /// <summary>
        /// OnDisable of the AvatarModelManager, the registered functions are unregistered to clean-up.
        /// </summary>
        protected virtual void OnDisable()
        {
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_MOOD_FINISH, OnChangedMood);
            EventManager.Instance.UnregisterEventReceiver(Constants.Event.ON_CHANGE_AVATAR_STATE_FINISH, OnChangedBehavior);
        }

        /// <summary>
        /// On Awake of the AvatarModelManager, the AvatarWidget is found and assigned to be used for future purposes. 
        /// </summary>
        protected virtual void Awake()
        {
            m_AvatarWidgetAttached = this.transform.GetComponentInParent<AvatarWidget>();
            if (m_AvatarWidgetAttached == null)
            {
                Log.Error("AvatarModelManager", "There is no Avatar Widget on any parent.");
                this.enabled = false;
            }
        }

        #endregion

        #region Changing Mood / Avatar State

        /// <summary>
        /// On Mood Change of the avatar, this function gets called. 
        /// </summary>
        /// <param name="args">If there 2 paremeteres we consider 1st paremeter is AvatarWidget object, otherwise we are using initial AvatarWidget object's values to call other functions</param>
		private void OnChangedMood(System.Object[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
				ChangedMood(m_AvatarWidgetAttached.MoodColor, m_AvatarWidgetAttached.MoodTimeModifier);
            }
            else if (args.Length == 2)
            {
                AvatarWidget avatarWidget = args[0] as AvatarWidget;
                if (avatarWidget != null)
                {
                    if (avatarWidget == m_AvatarWidgetAttached)
                    {
						ChangedMood(m_AvatarWidgetAttached.MoodColor, m_AvatarWidgetAttached.MoodTimeModifier);
                    }
                    else
                    {
                        //do nothing because the Avatar object is not our attached avatar.
                    }
                }
                else
                {
                    Log.Error("AvatarModelManager", "OnChangeMood the argument is not AvatarWidget object");
                }
            }
            else
            {
                Log.Error("AvatarModelManager", "OnChangeMood the argument length is undefined {0}", args.Length);
            }
        }

        /// <summary>
        /// On Behavior Change of the avater, this function gets called
        /// </summary>
        /// <param name="args">If there 2 paremeteres we consider 1st paremeter is AvatarWidget object, otherwise we are using initial AvatarWidget object's values to call other functions</param>
        private void OnChangedBehavior(System.Object[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
				ChangedBehavior(m_AvatarWidgetAttached.BehaviourColor, m_AvatarWidgetAttached.BehaviorTimeModifier);
            }
            else if (args.Length == 2)
            {
                AvatarWidget avatarWidget = args[0] as AvatarWidget;
                if (avatarWidget != null)
                {
                    if (avatarWidget == m_AvatarWidgetAttached)
                    {
						ChangedBehavior(m_AvatarWidgetAttached.BehaviourColor, m_AvatarWidgetAttached.BehaviorTimeModifier);
                    }
                    else
                    {
                        //do nothing because the Avatar object is not our attached avatar.
                    }
                }
                else
                {
                    Log.Error("AvatarModelManager", "OnChangeBehavior the argument is not AvatarWidget object");
                }
            }
            else
            {
                Log.Error("AvatarModelManager", "OnChangedBehavior the argument length is undefined {0}", args.Length);
            }
        }

        /// <summary>
        /// On Behavior Change, this function gets called. 
        /// </summary>
        /// <param name="colorToChange">Color to be used in animations of Avatar Model</param>
        /// <param name="speedModifier">Speed modifier to be used in animations of Avatar Model</param>
        public virtual void ChangedBehavior(Color colorToChange, float speedModifier) { }

		/// <summary>
		/// After Mood Change, this function gets called.
		/// </summary>
		/// <param name="colorToChange">Color to change.</param>
		/// <param name="speedModifier">Speed modifier.</param>
		public virtual void ChangedMood(Color colorToChange, float speedModifier) { }

        #endregion
    }

}
