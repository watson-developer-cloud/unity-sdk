using UnityEngine;
using System.Collections;
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
        public virtual void OnChangedMood(System.Object[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                ChangeToColor(m_AvatarWidgetAttached.MoodColor, m_AvatarWidgetAttached.MoodSpeedModifier);
            }
            else if (args.Length == 2)
            {
                AvatarWidget avatarWidget = args[0] as AvatarWidget;
                if (avatarWidget != null)
                {
                    if (avatarWidget == m_AvatarWidgetAttached)
                    {
                        ChangeToColor(m_AvatarWidgetAttached.MoodColor, m_AvatarWidgetAttached.MoodSpeedModifier);
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
        public virtual void OnChangedBehavior(System.Object[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                ChangeToColor(m_AvatarWidgetAttached.BehaviourColor, m_AvatarWidgetAttached.MoodSpeedModifier);
            }
            else if (args.Length == 2)
            {
                AvatarWidget avatarWidget = args[0] as AvatarWidget;
                if (avatarWidget != null)
                {
                    if (avatarWidget == m_AvatarWidgetAttached)
                    {
                        ChangeToColor(m_AvatarWidgetAttached.BehaviourColor, m_AvatarWidgetAttached.MoodSpeedModifier);
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
        /// On Mood change or Behavior Change, this function gets called. 
        /// </summary>
        /// <param name="colorToChange">Color to be used in animations of Avatar Model</param>
        /// <param name="speedModifier">Speed modifier to be used in animations of Avatar Model</param>
        public virtual void ChangeToColor(Color colorToChange, float speedModifier) { }

        #endregion
    }

}
